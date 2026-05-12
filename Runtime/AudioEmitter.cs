using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Unity.VisualScripting;
using System;

namespace WhwahAudio
{
    public class AudioEmitter : MonoBehaviour
    {
        //public
        [SerializeField] AudioClipWrapper audioResource;
        [SerializeReference] List<AudioModifier> activeModifiers = new();

        public static implicit operator AudioSource(AudioEmitter emitter) { return emitter.audioSource; }

        public bool InPool => inPool;
        public Action onPlay, onPause, onStop, onDestroy;

        //private
        private AudioSource audioSource;
        private Coroutine activeFade;
        private float audioModifierStrength;
        private bool inPool = true;

        /*
        [Header("Extra 3D Sound Settings")]
        [Tooltip("This makes the audio source not take into account the volume of the audio listener.")]
        [SerializeField] bool IgnoreListenerVolume = false;
        [Tooltip("Allows AudioSource to play even though AudioListener.pause is set to true. This is useful for the menu element sounds or background music in pause menus.")]
        [SerializeField] bool IgnoreListenerPause = false;
        [Tooltip("Determines if the spatializer effect is inserted before or after the effect filters.")]
        [SerializeField] bool SpatializePostEffects = false;
        [Tooltip("Whether the Audio Source should be updated in the fixed or dynamic update.")]
        [SerializeField] AudioVelocityUpdateMode VelocityUpdateMode = AudioVelocityUpdateMode.Auto;
        */

        //linked
        #region AudioSource Links
        public AudioSource Source => audioSource;
        public float volume { get => audioSource.volume; set => audioSource.volume = value; }
        public float spatialBlend { get => audioSource.spatialBlend; set => audioSource.spatialBlend = value; }
        public float pitch { get => audioSource.pitch; set => audioSource.pitch = value; }
        public float time { get => audioSource.time; set => audioSource.time = value; }
        public int timeSamples { get => audioSource.timeSamples; set => audioSource.timeSamples = value; }
        public AudioClipWrapper clip
        {
            get => audioResource;
            set => SetClip(value);
        }
        public AudioMixerGroup outputAudioMixerGroup { get => audioSource.outputAudioMixerGroup; set => audioSource.outputAudioMixerGroup = value; }
        public bool isPlaying => audioSource.isPlaying;
        public bool isVirtual => audioSource.isVirtual;
        public bool loop { get => audioSource.loop; set => audioSource.loop = value; }
        public bool ignoreListenerVolume { get => audioSource.ignoreListenerVolume; set => audioSource.ignoreListenerVolume = value; }
        public bool playOnAwake { get => audioSource.playOnAwake; set => audioSource.playOnAwake = value; }
        public bool ignoreListnerPause { get => audioSource.playOnAwake; set => audioSource.playOnAwake = value; }
        public AudioVelocityUpdateMode velocityUpdateMode { get => audioSource.velocityUpdateMode; set => audioSource.velocityUpdateMode = value; }
        public float panStereo { get => audioSource.panStereo; set => audioSource.panStereo = value; }
        public bool spatilize { get => audioSource.spatialize; set => audioSource.spatialize = value; }
        public bool spacializePostEffects { get => audioSource.spatializePostEffects; set => audioSource.spatializePostEffects = value; }
        public float reverbZoneMix { get => audioSource.reverbZoneMix; set => audioSource.reverbZoneMix = value; }
        public bool bypassEffects { get => audioSource.bypassEffects; set => audioSource.bypassEffects = value; }
        public bool bypassListnerEffects { get => audioSource.bypassListenerEffects; set => audioSource.bypassListenerEffects = value; }
        public bool bypassReverbZones { get => audioSource.bypassReverbZones; set => audioSource.bypassReverbZones = value; }
        public float dopplerLevel { get => audioSource.dopplerLevel; set => audioSource.dopplerLevel = value; }
        public float spread { get => audioSource.spread; set => audioSource.spread = value; }
        public int priority { get => audioSource.priority; set => audioSource.priority = value; }
        public bool mute { get => audioSource.mute; set => audioSource.mute = value; }
        public float minDistance { get => audioSource.minDistance; set => audioSource.minDistance = value; }
        public float maxDistance { get => audioSource.maxDistance; set => audioSource.maxDistance = value; }
        public AudioRolloffMode rolloffMode { get => audioSource.rolloffMode; set => audioSource.rolloffMode = value; }
        #endregion

        // BASE //
        private void Update()
        {
            foreach (var mod in activeModifiers)
            {
                mod.ApplyEffect(this, audioModifierStrength);
            }
        }

        // CREATE DESTROY //
        void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.Stop();
        }

        public void Initialize(AudioClipWrapper clip)
        {
            SetClip(clip);
            gameObject.name = "sfx_" + audioSource.clip.name;
            audioResource.onEdited += MatchSource;
        }

        void SetClip(AudioClipWrapper clip)
        {
            audioResource = clip;
            audioSource.clip = clip;
            MatchSource();

            if (!clip._loop)
                Destroy(gameObject, audioSource.clip.length);
        }
        void MatchSource()
        {
            outputAudioMixerGroup = audioResource.mixerGroup;
            volume = audioResource._volume;
            pitch = audioResource._pitch;
            minDistance = audioResource._minDistance;
            maxDistance = audioResource._maxDistance;
            spatialBlend = audioResource._3DBalance;
            spread = audioResource._3DSpreadAngle;
            loop = audioResource._loop;
            dopplerLevel = audioResource._dopplerEffect;

            rolloffMode = audioResource._rolloffMode;
            if (rolloffMode == AudioRolloffMode.Custom)
                Source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audioResource._rolloffCurve);

            activeModifiers.ForEach(hotMod => hotMod.Destroy());
            activeModifiers.Clear();

            clip.modifiers?.ForEach(coldMod => activeModifiers.Add(coldMod.CreateModifier()));
            activeModifiers.ForEach(hotMod => hotMod.Initialize(this));
        }

        private void OnDestroy()
        {
            if (audioSource != null)
                Destroy(audioSource.gameObject);

            audioResource.onEdited -= MatchSource;
            onDestroy?.Invoke();
        }

        // PLAY - PAUSE //
        public void Play()
        {
            audioSource.enabled = true;
            audioSource.Play();
            onPlay?.Invoke();
        }

        public void Stop()
        {
            audioSource.Stop();
            onStop?.Invoke();
        }

        public void Pause()
        {
            audioSource.Pause();
            onPause?.Invoke();
        }

        // MOD CONTROL //
        public Type GetModifier<Type>()
        {
            return activeModifiers.Find(m => m is Type).ConvertTo<Type>();
        }

        public bool TryGetModifier<Type>(out Type modifier)
        {
            modifier = GetModifier<Type>();
            return modifier != null;
        }

        public void SetModiferStrength(float strength)
        {
            audioModifierStrength = Mathf.Clamp01(strength);
        }


        // OTHER //
        public void FadeAudio(float from, float to, float fadeTime = 0.5f, bool pauseOnComplete = false, bool returnOnComplete = false)
        {
            if (activeFade != null)
                StopCoroutine(activeFade);

            activeFade = StartCoroutine(RunFadeAudio(from, to, fadeTime, pauseOnComplete = false, returnOnComplete = false));
        }

        private IEnumerator RunFadeAudio(float from, float to, float fadeTime = 0.5f, bool pauseOnComplete = false, bool returnOnComplete = false)
        {
            fadeTime = Mathf.Clamp(fadeTime, 0.02f, Mathf.Infinity);
            from = from < 0 ? audioSource.volume : Mathf.Clamp01(from);
            to = Mathf.Clamp01(to);
            float timer = 0;
            audioSource.volume = from;

            while (audioSource.volume != to)
            {
                yield return null;
                timer += Time.deltaTime;
                if (audioSource == null) yield break;
                audioSource.volume = Mathf.Lerp(from, to, Mathf.InverseLerp(0, fadeTime, timer));
            }

            if (pauseOnComplete) Pause();
            if (returnOnComplete) Destroy(gameObject, 0);
        }
    }
}