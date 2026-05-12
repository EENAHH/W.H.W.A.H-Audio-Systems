using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace WhwahAudio
{
    public abstract class AudioClipWrapper : ScriptableObject
    {
        public static implicit operator AudioClip(AudioClipWrapper resource) { return resource.Clip(); }

        public abstract AudioClip Clip();

        [Header("Base Settings:")]
        public AudioMixerGroup mixerGroup;
        public bool _loop = false;
        public bool _dontDestroyOnLoad = false;
        [Range(0, 1)] public float _volume = 1;
        [Range(0, 5)] public float _pitch = 1;
        [Range(0, 1)] public float _3DBalance = 1;
        [Range(0, 360)] public float _3DSpreadAngle = 0;
        [Range(0, 5)] public float _dopplerEffect = 1;
        [Line(GUIColor.Gray)]

        [Header("Rolloff Settings:")]
        [SerializeField, HorizontalGroup(nameof(_minDistance), nameof(_maxDistance))]
        public float _minDistance = 1;
        [HideProperty] public float _maxDistance = 500;
        public AudioRolloffMode _rolloffMode = AudioRolloffMode.Logarithmic;
        [ShowField(nameof(_rolloffMode), AudioRolloffMode.Custom)] public AnimationCurve _rolloffCurve = new(new Keyframe[] { new(0, 1, 0, -573.3928f, 0, 0.0017296326f), new(1, 0, -0.008833871f, 0, 1, 0) });
        virtual public List<AudioModifierFactory> modifiers => nullModifiers;
        private List<AudioModifierFactory> nullModifiers = new();

        new public string name => Clip().name;
        public float length => Clip().length;
        public bool ambisonic => Clip().ambisonic;
        public int frequency => Clip().frequency;
        public int samples => Clip().samples;
        public int channels => Clip().channels;
        public bool preloadAudioData => Clip().preloadAudioData;
        public AudioDataLoadState loadState => Clip().loadState;
        public AudioClipLoadType loadType => Clip().loadType;

        public bool LoadAudioData() => Clip().LoadAudioData();
        public bool UnloadAudioData() => Clip().UnloadAudioData();
        public bool SetData(float[] data, int offsetSamples) => Clip().SetData(data, offsetSamples);

        public Action onEdited;
        public virtual void OnValidate()
        {
            if (_rolloffMode == AudioRolloffMode.Custom)
                _minDistance = 0;

            onEdited?.Invoke();
        }
    }
}