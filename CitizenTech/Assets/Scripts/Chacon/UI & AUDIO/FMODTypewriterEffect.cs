
using UnityEngine;
using FMODUnity;
using PixelCrushers.DialogueSystem;

public class FMODTypewriterEffect : UnityUITypewriterEffect
{
    [SerializeField] private EventReference dialogueBlip;

    protected override void PlayCharacterAudio(char c)
    {
        if (char.IsWhiteSpace(c))
            return;

        RuntimeManager.PlayOneShot(dialogueBlip);
    }
}
