using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhwahAudio
{
    public enum AudioModifierFactoryType { PositionRandom, Pitch, PitchRandom, MinDistance, MaxDistance, Volume, LowPass, HighPass, Chorus, Loop, }

    [System.Serializable]
    public abstract class AudioModifierFactory
    {
        public abstract AudioModifier CreateModifier();

        // Factory Dispatch. Easy for UI useages
        public static AudioModifierFactory CreateFactory(AudioModifierFactoryType type)
        {
            switch (type)
            {
                case AudioModifierFactoryType.PositionRandom:
                    return new AudioRandomPositionFactory();
                case AudioModifierFactoryType.Pitch:
                    return new AudioPitchFactory();
                case AudioModifierFactoryType.PitchRandom:
                    return new AudioPitchRandomFactory();
                case AudioModifierFactoryType.MinDistance:
                    return new AudioMinDistanceFactory();
                case AudioModifierFactoryType.MaxDistance:
                    return new AudioMaxDistanceFactory();
                case AudioModifierFactoryType.Volume:
                    return new AudioVolumeFactory();
                case AudioModifierFactoryType.LowPass:
                    return new AudioLowPassFactory();
                case AudioModifierFactoryType.HighPass:
                    return new AudioHighPassFactory();
                case AudioModifierFactoryType.Chorus:
                    return new AudioChorusFactory();
                case AudioModifierFactoryType.Loop:
                    return new AudioLoopFactory();
                default:
                    return null;
            }
        }
        public static AudioModifierFactory CreateFactory(string assemblyTypeRef)
        {
            var type = Type.GetType(assemblyTypeRef);
            var ctors = type.GetConstructors();
            return ctors[0].Invoke(new object[] { }) as AudioModifierFactory;

        }
    }
#pragma warning disable CS0414

    // -----------  LOOP  -----------
    [System.Serializable]
    public class AudioLoopFactory : AudioModifierFactory
    {
        [SerializeField, HideProperty] string displayName = "Loop Modifier";
        [SerializeField, Rename("Looped Sections")] public List<LoopedAudioSection> loops = new();
        public override AudioModifier CreateModifier()
        {
            return new AudioModifierLoop(this);
        }

    }

    [System.Serializable]
    public class LoopedAudioSection
    {
        public string loopKey = "Loop_01";
        [SerializeField, HorizontalGroup(nameof(start), nameof(end))]
        private EditorAttributes.Void loopValues;

        [HideProperty] public float start;
        [HideProperty] public float end;
    }

    // -----------  PITCH  -----------

    [System.Serializable]
    public class AudioRandomPositionFactory : AudioModifierFactory
    {
        AudioModifierRandomPosition modifier;
        [SerializeField, HideProperty] string displayName = "Random Position Modifier";

        public Vector3 GetRandomOffset() => new Vector3(UnityEngine.Random.Range(-randomOffset.x, randomOffset.x), UnityEngine.Random.Range(-randomOffset.y, randomOffset.y), UnityEngine.Random.Range(-randomOffset.z, randomOffset.z));
        public Vector3 randomOffset;

        public override AudioModifier CreateModifier()
        {
            if (modifier != null) return modifier;
            modifier = new(this);
            return modifier;
        }

    }

    // -----------  PITCH  -----------

    [System.Serializable]
    public class AudioPitchFactory : AudioModifierFactory
    {
        AudioModifierDynamicPitch modifier;
        [SerializeField, HideProperty] string displayName = "Pitch Modifier";

        [Header("Dynamic Values")]
        [SerializeField, MinMaxSlider(0, 5)] public Vector2 pitchRange = new Vector2(0.8f, 1.2f);

        public override AudioModifier CreateModifier()
        {
            if (modifier != null) return modifier;
            modifier = new(this);
            return modifier;
        }
    }

    [System.Serializable]
    public class AudioPitchRandomFactory : AudioModifierFactory
    {
        AudioModifierRandomPitch modifier;
        [SerializeField, HideProperty] string displayName = "Pitch Random";

        [Header("Random Range")]
        [SerializeField, MinMaxSlider(0, 5)] public Vector2 pitchRange = new Vector2(0.95f, 1.05f);
        public override AudioModifier CreateModifier()
        {
            if (modifier != null) return modifier;
            modifier = new(this);
            return modifier;
        }
    }

    // -----------  RANGE  -----------

    [System.Serializable]
    public class AudioMinDistanceFactory : AudioModifierFactory
    {
        AudioModifierDynamicMinDistance modifier;
        [SerializeField, HideProperty] string displayName = "Min Distance Modifier";

        [Header("Dynamic Values")]
        [SerializeField] public Vector2 minDistanceRange = new Vector2(1, 5);
        public override AudioModifier CreateModifier()
        {
            if (modifier != null) return modifier;
            modifier = new(this);
            return modifier;
        }
    }

    [System.Serializable]
    public class AudioMaxDistanceFactory : AudioModifierFactory
    {
        AudioModifierDynamicMaxDistance modifier;
        [SerializeField, HideProperty] string displayName = "Min Distance Modifier";

        [Header("Dynamic Values")]
        [SerializeField] public Vector2 maxDistanceRange = new Vector2(1, 5);
        public override AudioModifier CreateModifier()
        {
            if (modifier != null) return modifier;
            modifier = new(this);
            return modifier;
        }
    }

    // -----------  VOLUME  -----------

    [System.Serializable]
    public class AudioVolumeFactory : AudioModifierFactory
    {
        AudioModifierDynamicVolume modifier;
        [SerializeField, HideProperty] string displayName = "Volume Modifier";

        [Header("Dynamic Values")]
        [SerializeField] public Vector2 volumeRange = new Vector2(0, 1);
        public override AudioModifier CreateModifier()
        {
            if (modifier != null) return modifier;
            modifier = new(this);
            return modifier;
        }
    }

    [System.Serializable]
    public class AudioLowPassFactory : AudioModifierFactory
    {
        [SerializeField, HideProperty] public string displayName = "Low Pass Modifier";

        [Header("Start Values")]
        [SerializeField, Range(10, 22000)] public float cutoff = 5000;
        [SerializeField, Range(1, 10)] public float resonance = 1;

        [ToggleGroup("Dynamic Values", drawInBox: true, nameof(rangeCutoff), nameof(rangeResonance), nameof(effectCurve))]
        [HideProperty] public bool useDynamicRange;
        [HideProperty, MinMaxSlider(10, 22000)] public Vector2 rangeCutoff = new Vector2(5000, 7000);
        [HideProperty, MinMaxSlider(1, 10)] public Vector2 rangeResonance = new Vector2(1, 2);
        [HideProperty] public AnimationCurve effectCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 1f), new Keyframe(1, 1, 1f, 0) });

        public override AudioModifier CreateModifier()
        {
            return new AudioModifierLowPass(this);
        }
    }

    [System.Serializable]
    public class AudioHighPassFactory : AudioModifierFactory
    {
        [SerializeField, HideProperty] string displayName = "High Pass Modifier";

        [Header("Start Values")]
        [SerializeField, Range(10, 22000)] public float cutoff = 5000;
        [SerializeField, Range(1, 10)] public float resonance = 1;

        [ToggleGroup("Dynamic Values", drawInBox: true, nameof(rangeCutoff), nameof(rangeResonance), nameof(effectCurve))]
        [HideProperty] public bool useDynamicRange;
        [HideProperty, MinMaxSlider(10, 22000)] public Vector2 rangeCutoff = new Vector2(5000, 7000);
        [HideProperty, MinMaxSlider(1, 10)] public Vector2 rangeResonance = new Vector2(1, 2);
        [HideProperty] public AnimationCurve effectCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 1f), new Keyframe(1, 1, 1f, 0) });

        public override AudioModifier CreateModifier()
        {
            return new AudioModifierHighPass(this);
        }
    }

    [System.Serializable]
    public class AudioChorusFactory : AudioModifierFactory
    {
        [SerializeField, HideProperty] string displayName = "Chorus Modifier";

        [Header("Start Values")]
        [SerializeField, Range(0, 1)] public float dryMix = 0.5f;
        [SerializeField, Range(0, 1)] public float wetMix1 = 0.5f;
        [SerializeField, Range(0, 1)] public float wetMix2 = 0.5f;
        [SerializeField, Range(0, 1)] public float wetMix3 = 0.5f;
        [SerializeField, Range(0.1f, 100)] public float delay = 40;
        [SerializeField, Range(0, 20)] public float rate = 0.8f;
        [SerializeField, Range(0, 1)] public float depth = 0.03f;

        [ToggleGroup("Dynamic Values", drawInBox: true, nameof(depthRange), nameof(effectCurve))]
        [HideProperty] public bool useDynamicRange;
        [HideProperty, MinMaxSlider(0, 1)] public Vector2 depthRange = new Vector2(0.5f, 1);
        [HideProperty] public AnimationCurve effectCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, 1f), new Keyframe(1, 1, 1f, 0) });

        public override AudioModifier CreateModifier()
        {
            return new AudioModifierChorus(this);
        }
    }

#pragma warning restore CS0414
}