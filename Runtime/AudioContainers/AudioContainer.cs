using UnityEngine;
using EditorAttributes;
using System.Collections.Generic;

namespace WhwahAudio
{
    [System.Serializable, CreateAssetMenu(fileName = "New Clip", menuName = "Audio/Audio Container")]
    public class AudioContainer : AudioClipWrapper
    {
        // Base Clip
        [Line(GUIColor.Gray)]
        [Header("Base Clip:")]
        [SerializeField, Required] private AudioClip baseClip;
        public override AudioClip Clip() => baseClip;
        [Line(GUIColor.Gray)]

        // Modifiers
        [Header("Audio Modifiers:")]
        [SerializeField, Tooltip("Searches the unity project for 'AudioModifierFactory' classes. If you have made custom modifiers they will show up here.")]
        bool addByAssemblyReference;

        [HideField(nameof(addByAssemblyReference)), InlineButton(nameof(AddFactory), "Add Modifier")]
        public AudioModifierFactoryType typeToAdd;

        [ShowField(nameof(addByAssemblyReference)), InlineButton(nameof(AddCustomFactory), "Add Custom"), TypeDropdown(baseTypeFilter: typeof(AudioModifierFactory))]
        public string typePath;

        public void AddFactory() => modifiers.Add(AudioModifierFactory.CreateFactory(typeToAdd));
        public void AddCustomFactory() => modifiers.Add(AudioModifierFactory.CreateFactory(typePath));

        [Tooltip("Modifiers apply effects to audio emitters over time. " +
            "\n\nMost run off of a strength multiplier which is set in code. " +
            "\n\nThe minimum range is used when the strength is 0 and the maximum range is used when strength is 1. " +
            "\n\nThe modfiers which use strength are currently (Pitch, Range, & Volume). " +
            "\n\nLoop is special & must be told to move to the next section in code")]
        [SerializeReference] public List<AudioModifierFactory> Modifiers;
        public override List<AudioModifierFactory> modifiers => Modifiers;
    }
}