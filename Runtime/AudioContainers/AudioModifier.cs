using UnityEngine;

namespace WhwahAudio
{
    public abstract class AudioModifier
    {
        public virtual void Initialize(AudioEmitter emitter) { }
        public virtual void ApplyEffect(AudioEmitter emitter, float strength) { }
        public virtual void Destroy() { }
    }


    // -----------  LOOP  -----------
    public class AudioModifierLoop : AudioModifier
    {
        private AudioLoopFactory factory;
        private int index = 0;

        public AudioModifierLoop(AudioLoopFactory factory) { this.factory = factory; }

        //observes it's audio emitter for looped threshold. Is called each update by the emitter from its list of modifiers.
        public override void ApplyEffect(AudioEmitter emitter, float strength)
        {
            if (factory.loops == null || index >= factory.loops.Count)
                return;

            if (emitter.time >= factory.loops[index].end)
                emitter.time = factory.loops[index].start;
        }

        public void SetLoopByKeyName(string key) => index = factory.loops.FindIndex(loop => loop.loopKey == key);
        public void SetLoopIndex(int index) => this.index = Mathf.Clamp(index, 0, factory.loops.Count); //intentional. If index is 1 more than count then loops have ended 
        public void NextLoop() => Mathf.Clamp(++index, 0, factory.loops.Count);
    }


    // -----------  POSITION  -----------
    public class AudioModifierRandomPosition : AudioModifier
    {
        private AudioRandomPositionFactory factory;

        public AudioModifierRandomPosition(AudioRandomPositionFactory factory) { this.factory = factory; }

        public override void Initialize(AudioEmitter emitter)
        {
            emitter.Source.transform.localPosition += factory.GetRandomOffset();
        }
    }

    // -----------  PITCH  -----------
    public class AudioModifierDynamicPitch : AudioModifier
    {
        private AudioPitchFactory factory;

        public AudioModifierDynamicPitch(AudioPitchFactory factory) { this.factory = factory; }

        public override void ApplyEffect(AudioEmitter emitter, float strength)
        {
            emitter.pitch = Mathf.Lerp(factory.pitchRange.x, factory.pitchRange.y, strength);
        }
    }

    public class AudioModifierRandomPitch : AudioModifier
    {
        private AudioPitchRandomFactory factory;

        public AudioModifierRandomPitch(AudioPitchRandomFactory factory) { this.factory = factory; }

        public override void Initialize(AudioEmitter emitter)
        {
            emitter.pitch = Mathf.Lerp(factory.pitchRange.x, factory.pitchRange.y, Random.value);
        }
    }

    // -----------  RANGE  -----------
    public class AudioModifierDynamicMinDistance : AudioModifier
    {
        AudioMinDistanceFactory factory;

        public AudioModifierDynamicMinDistance(AudioMinDistanceFactory factory) { this.factory = factory; }

        public override void ApplyEffect(AudioEmitter emitter, float strength)
        {
            emitter.minDistance = Mathf.Lerp(factory.minDistanceRange.x, factory.minDistanceRange.y, strength);
        }
    }

    public class AudioModifierDynamicMaxDistance : AudioModifier
    {
        AudioMaxDistanceFactory factory;

        public AudioModifierDynamicMaxDistance(AudioMaxDistanceFactory factory) { this.factory = factory; }

        public override void ApplyEffect(AudioEmitter emitter, float strength)
        {
            emitter.maxDistance = Mathf.Lerp(factory.maxDistanceRange.x, factory.maxDistanceRange.y, strength);
        }
    }

    // -----------  VOLUME  -----------
    public class AudioModifierDynamicVolume : AudioModifier
    {
        AudioVolumeFactory factory;

        public AudioModifierDynamicVolume(AudioVolumeFactory factory) { this.factory = factory; }

        public override void ApplyEffect(AudioEmitter emitter, float strength)
        {
            emitter.volume = Mathf.Lerp(factory.volumeRange.x, factory.volumeRange.y, strength);
        }
    }


    // -----------  LOW / HIGH PASS -----------
    public class AudioModifierLowPass : AudioModifier
    {
        private AudioLowPassFactory factory;
        private AudioLowPassFilter lowPass;

        public AudioModifierLowPass(AudioLowPassFactory factory) { this.factory = factory; }

        public override void ApplyEffect(AudioEmitter emitter, float strength)
        {
            if (!factory.useDynamicRange)
                return;

            lowPass.cutoffFrequency = Mathf.Lerp(factory.rangeCutoff.x, factory.rangeCutoff.y, strength);
            lowPass.lowpassResonanceQ = Mathf.Lerp(factory.rangeResonance.x, factory.rangeResonance.y, strength);
        }

        public override void Initialize(AudioEmitter emitter)
        {
            if(!emitter.Source.gameObject.TryGetComponent(out lowPass))
                lowPass = emitter.Source.gameObject.AddComponent<AudioLowPassFilter>();
            lowPass.cutoffFrequency = factory.cutoff;
            lowPass.lowpassResonanceQ = factory.resonance;
        }
    }


    public class AudioModifierHighPass : AudioModifier
    {
        private AudioHighPassFactory factory;
        private AudioHighPassFilter highPass;

        public AudioModifierHighPass(AudioHighPassFactory factory) { this.factory = factory; }

        public override void ApplyEffect(AudioEmitter emitter, float strength)
        {
            if (!factory.useDynamicRange)
                return;

            highPass.cutoffFrequency = Mathf.Lerp(factory.rangeCutoff.x, factory.rangeCutoff.y, strength);
            highPass.highpassResonanceQ = Mathf.Lerp(factory.rangeResonance.x, factory.rangeResonance.y, strength);
        }

        public override void Initialize(AudioEmitter emitter)
        {
            if(!emitter.Source.gameObject.TryGetComponent(out highPass))
                highPass = emitter.Source.gameObject.GetComponent<AudioHighPassFilter>();

            highPass.cutoffFrequency = factory.cutoff;
            highPass.highpassResonanceQ = factory.resonance;
        }
    }

    // -----------  CHORUS -----------
    public class AudioModifierChorus : AudioModifier
    {
        private AudioChorusFactory factory;
        private AudioChorusFilter chorus;

        public AudioModifierChorus(AudioChorusFactory factory) { this.factory = factory; }

        public override void ApplyEffect(AudioEmitter emitter, float strength)
        {
            if (factory.useDynamicRange)
                chorus.depth = Mathf.Lerp(factory.depthRange.x, factory.depthRange.y, factory.effectCurve.Evaluate(strength));
        }

        public override void Initialize(AudioEmitter emitter)
        {
            if(!emitter.Source.gameObject.TryGetComponent(out chorus))
                chorus = emitter.Source.gameObject.GetComponent<AudioChorusFilter>();

            chorus.dryMix = factory.dryMix;
            chorus.wetMix1 = factory.wetMix1;
            chorus.wetMix2 = factory.wetMix2;
            chorus.wetMix3 = factory.wetMix3;
            chorus.delay = factory.delay;
            chorus.rate = factory.rate;
            chorus.depth = factory.depth;
        }
    }
}