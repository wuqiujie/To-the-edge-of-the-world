using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    [Tooltip("The audio events triggered on awake")]
    public List<SoundManager.SOUND_EVENT> awakeEvents;

    [Tooltip("The audio interruptions triggered on awake")]
    public List<SoundManager.SOUND_INTERUPTION_TYPE> awakeInterruptions;

    // Start is called before the first frame update
    void Start()
    {
        // Trigger events and interruptions
        foreach (SoundManager.SOUND_INTERUPTION_TYPE interruption in awakeInterruptions)
            SoundManager.TriggerInterruptionEvent(interruption);
        foreach (SoundManager.SOUND_EVENT soundEvent in awakeEvents)
            SoundManager.TriggerAudioEvent(soundEvent);
        
    }
}
