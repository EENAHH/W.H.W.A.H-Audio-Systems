using UnityEngine;
using UnityEngine.Audio;

namespace WhwahAudio
{
    public class AudioPlayer 
    {
        private AudioClipWrapper _clip;
        private AudioEmitter _emitter;
        private AudioMixerGroup _mixerGroup;
        private Vector3 _position = Vector3.zero;
        private Transform _parent = null;
        private bool _loop = false;
        private bool _dontDestroyOnLoad = false;
        private AudioEmitter fadeTrack = null;
        private float crossfadeDuration = 2;

        private AudioPlayer() { } //constructor blocker

        //override builder. Use these to override parameters from the audio resource
        public AudioPlayer WithMixerGroup(AudioMixerGroup mixerGroup) { _mixerGroup = mixerGroup; return this; }
        public AudioPlayer WithLocalPosition(Vector3 position) { _position = position; return this; }
        public AudioPlayer WithParent(Transform parent) { _parent = parent; return this; }
        public AudioPlayer WithDontDestroyOnLoad(bool dontDestroyOnLoad = true) { _dontDestroyOnLoad = dontDestroyOnLoad; return this; }
        public AudioPlayer WithCrossFade(AudioEmitter oldTrack, float duration = 2) { fadeTrack = oldTrack; crossfadeDuration = duration; return this; }

        public AudioEmitter Play()
        {
            // Setup
            _emitter = new GameObject().AddComponent<AudioEmitter>();

            // Overridden
            _emitter.transform.parent = _parent;
            _emitter.transform.localPosition = _position;

            _emitter.Initialize(_clip);

            // Overrides
            _emitter.outputAudioMixerGroup = _mixerGroup;
            _emitter.loop = _loop;

            // Play
            _emitter.Play();

            // Crossfade
            if (fadeTrack)
            {
                fadeTrack.FadeAudio(-1, 0, crossfadeDuration, returnOnComplete: true);
                _emitter.FadeAudio(0, _emitter.volume, crossfadeDuration);
            }
        
            // Do not move to dndol if a child
            if(_dontDestroyOnLoad && _parent == null)
                GameObject.DontDestroyOnLoad(_emitter);

            return _emitter;
        }


        //////    Singleton    //////
        private static AudioPlayer _inst;
        public static AudioPlayer Create(AudioClipWrapper clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("The audio resource sent was null. Check your audio assignments");
                return null;
            }

            if (_inst == null)
                _inst = new AudioPlayer();

            _inst.Reset(clip);
            return _inst;
        }

        //reset defaults
        void Reset()
        {
            _clip = null;
            _mixerGroup = default;
            _emitter = null;
            _position = Vector3.zero;
            _parent = null;
            _loop = false;
            _dontDestroyOnLoad = false;
            fadeTrack = null;
            crossfadeDuration = 2;
        }

        //reset to clip
        void Reset(AudioClipWrapper clip)
        {
            _clip = clip;
            _mixerGroup = clip.mixerGroup ? clip.mixerGroup : default;
            _emitter = null;
            _position = Vector3.zero;
            _parent = null;
            _loop = clip._loop;
            _dontDestroyOnLoad = clip._dontDestroyOnLoad;
            fadeTrack = null;
            crossfadeDuration = 2;
        }
    }

}
