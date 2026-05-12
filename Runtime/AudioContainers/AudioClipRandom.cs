using EditorAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhwahAudio
{
    [System.Serializable, CreateAssetMenu(fileName = "New Random Clip", menuName = "Audio/Randomized Audio Clip")]
    public class AudioClipRandom : AudioClipWrapper
    {
        public override AudioClip Clip()
        {
            return Time.frameCount == lastFrame ? frameClip : GetRandomClip();
        }

        [SerializeField] List<WeightedClip> clipList;
    
        //Store current frame. If the frame is the same then return the same audio clip.
        private AudioClip frameClip;
        private int lastFrame;

        [System.Serializable] public class WeightedClip 
        {
            [Tooltip("The amount of ballots this clip has to win the random selection")]
            [SerializeField] public int weight = 1;
            public AudioClipWrapper clip;

            WeightedClip()
            {
                weight = 1;
            }
        }

        public override void OnValidate()
        {
            base.OnValidate();

            foreach (var item in clipList)
            {
                item.weight = Mathf.Clamp(item.weight, 1, int.MaxValue);
            }

            var dupe = clipList.Find(x => x.clip == this);
            if (dupe != null)
            {
                Debug.LogError("Circular reference detected! A clip list must not contain a reference to itself.");
                clipList.Remove(dupe);
            }
        }

        AudioClipWrapper GetRandomClip()
        {
            lastFrame = Time.frameCount;

            int totalWeight = 0;
            int index = 0;

            clipList.ForEach(clip => { totalWeight += clip.weight; });
            int rand = Random.Range(1, totalWeight + 1);

            foreach (WeightedClip weighted in clipList)
            {
                if(weighted.clip == null)
                    continue;

                index += weighted.weight;
                if (index >= rand)
                {
                    frameClip = weighted.clip;
                    return weighted.clip;
                }
            }

            Debug.LogWarning("Audio Clip Weight Error");
            frameClip = clipList.Last().clip;
            return clipList.Last().clip;
        }

        [Button] void DebugTestClip() => Debug.Log(GetRandomClip().name);
    }
}
