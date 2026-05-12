using EditorAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace WhwahAudio
{
    [System.Serializable, CreateAssetMenu(fileName = "New Sequential Clip", menuName = "Audio/Sequential Audio Clip")]
    public class AudioClipSequential : AudioClipWrapper
    {
        public override AudioClip Clip() => GetClip();

        [SerializeField, Range(0, 1)] float skipChance = 0;
        [SerializeField] List<AudioClipWrapper> clipList;

        int index = 0;

        public override void OnValidate()
        {
            base.OnValidate();

            if (clipList.Contains(this))
            {
                Debug.LogError("Circular reference detected! A clip list must not contain a reference to itself.");
                clipList.Remove(this);
            }
        }

        AudioClipWrapper GetClip()
        {
            AudioClipWrapper clip = clipList[index++];
            index = skipChance > Random.Range(0, 1) ? index + 1 : index;
            index = index >= clipList.Count ? 0 : index;
            return clip;
        }

        public void ResetSequence()
        {
            index = 0;
        }

        [Button] void DebugTestClip() => Debug.Log(GetClip().name);
    }
}
