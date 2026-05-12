using EditorAttributes;
using UnityEngine;

namespace WhwahAudio
{
    public class AudioSpawnPoint : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] bool playOnEnable = true;
        [SerializeField] float fadeIn = 0;
        [SerializeField, PropertyDropdown] AudioClipWrapper audioResource;

        public AudioEmitter _Emitter => emitter;
        private AudioEmitter emitter;

        [SerializeField, DrawHandle(GUIColor.Cyan, Space.Self), HideProperty] private float minDistance;
        [SerializeField, DrawHandle(GUIColor.Cyan, Space.Self), HideProperty] private float maxDistance;

        private void OnDrawGizmosSelected()
        {
            minDistance = audioResource._minDistance;
            maxDistance = audioResource._maxDistance;
            //audioResource.OnValidate();

            if (emitter)
                Debug.DrawLine(transform.position, emitter.transform.position, Color.cyan);
        }

        private void OnValidate()
        {
            audioResource._minDistance = minDistance;
            audioResource._maxDistance = maxDistance;

            if (_Emitter)
            {
                _Emitter.Source.minDistance = minDistance;
                _Emitter.Source.maxDistance = maxDistance;
            }
        }

        private void OnEnable()
        {
            if (!playOnEnable)
                return;

            PlayAudioClip();
        }

        [Button]
        public void PlayAudioClip()
        {
            if (!Application.isPlaying)
                return;

            if (audioResource == null)
            {
                Debug.LogAssertion(name + " has no audioResource assigned");
                return;
            }

            if (audioResource._loop)
            {
                if (emitter)
                    emitter.FadeAudio(-1, 0, fadeIn, returnOnComplete: true);
                emitter = AudioPlayer.Create(audioResource).WithParent(transform).Play();
            }
            else
                emitter = AudioPlayer.Create(audioResource).WithParent(transform).Play();

            if (emitter && fadeIn > 0)
                emitter.FadeAudio(0, 1, fadeIn);
        }
    }
}