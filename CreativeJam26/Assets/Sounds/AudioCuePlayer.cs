using UnityEngine;

// Add to a runtime-created UI Button and call Play from its On Click event.
public class AudioCuePlayer : MonoBehaviour
{
    [SerializeField] private AudioCue cue = AudioCue.UISelect;

    public void Play()
    {
        AudioManager.Play(cue);
    }
}
