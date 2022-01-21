using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    // This should describe every possible event in game which could trigger a sound. This will be continually grown by the programmers, sound people need not worry.
    public enum SOUND_EVENT { GAME_START, SCENE_1_START, SCENE_3_START, LIGHTING_STRIKE, DIALOGUE, SCENE_2_START,
        SCENE_1_BATTLE_START, HIT_SEA_MONSTER, HIT_HUMAN, END_CREDITS, START_SHANTY, START_CTHULU_BATTLE
    };

    // Ways in which audio can be interrupted
    public enum SOUND_INTERUPTION_TYPE { SAME_SOUND, ANY_SOUND, SONG_PLAYED, NEW_SCENE_LOADED, END_CTHULU_BATTLE }

    [Tooltip("The list of all sounds in the game.")]
    public List<SoundContainer> sounds;

    // Singleton pattern
    private static SoundManager singleton;

    // The audiosources currently playing
    private List<SoundPlayer> currentSources;

    // This class describes sound objects for audio people.
    [System.Serializable]
    public class SoundContainer
    {
        [Tooltip("The clip of the sound itself.")]
        public AudioClip sound;
        [Tooltip("The event which triggers the sound (ask Faris if the event you want isn't here.)")]
        public SOUND_EVENT triggerEvent;
        [Tooltip("How do you interrupt the sound?")]
        public List<SOUND_INTERUPTION_TYPE> interuptions;
        [Tooltip("Does the sound loop?")]
        public bool loop = false;
        [Tooltip("How loud does the sound play?")]
        [Range(0, 1)]
        public float volume = 0.5f;
        public enum AUDIO_SPACE { _2D, _3D }
        [Tooltip("Do you want it to sound like it's coming from a source or just play in general 2D space?")]
        public AUDIO_SPACE audioSpace;
        [Tooltip("Which object does this sound come from?")]
        public Transform soundObject;
        [Tooltip("What interruption events does this sound trigger?")]
        public List<SOUND_INTERUPTION_TYPE> triggerInterruptions;
    }

    // This class is attached to the objects which produce our audio to keep track of if they need to be interrupted
    public class SoundPlayer : MonoBehaviour
    {
        public SOUND_EVENT triggerEvent;
        public AudioSource source;
        public List<SOUND_INTERUPTION_TYPE> interuptionTypes;
    }

    // Start is called before the first frame update
    void Start() {
        // Init vars
        currentSources = new List<SoundPlayer>();

        // Singleton pattern
        if (!singleton) {
            singleton = this;

            // Ensure the ambient sounds are played and managed
            TriggerAudioEvent(SOUND_EVENT.GAME_START);
        } else
            Debug.LogError("There should only be one SoundContainer in the scene.");
    }

    // Halts playback of a sound
    private static void StopSound(SoundPlayer player) {
        if (singleton.currentSources.Contains(player))
            singleton.currentSources.Remove(player);
        if (player)
            Destroy(player.gameObject);
    }

    // Halts appropriate sources according to a soundEvent
    private static void InterruptSources(SOUND_EVENT soundEvent) {
        for (int i = singleton.currentSources.Count - 1; i >= 0; i--) {
            SoundPlayer player = singleton.currentSources[i];
            bool stop = false;
            foreach (SOUND_INTERUPTION_TYPE interuption in player.interuptionTypes) {
                switch (interuption) {
                    case SOUND_INTERUPTION_TYPE.SAME_SOUND:
                        if (soundEvent == player.triggerEvent)
                            stop = true;
                        break;
                    case SOUND_INTERUPTION_TYPE.ANY_SOUND:
                        stop = true;
                        break;
                }
                if (stop)
                    break;
            }
            if (stop)
                StopSound(player);
        }

        // Triggers the song start interruption
        if (Enum.GetName(typeof(SOUND_EVENT), soundEvent).Split('_')[0] == "M")
            TriggerInterruptionEvent(SOUND_INTERUPTION_TYPE.SONG_PLAYED);
    }

    public static void TriggerInterruptionEvent(SOUND_INTERUPTION_TYPE interruption) {
        Debug.Log("Interruption: " + interruption.ToString());
        for (int i = singleton.currentSources.Count - 1; i >= 0; i--) {
            SoundPlayer player = singleton.currentSources[i];
            if (player.interuptionTypes.Contains(interruption))
                StopSound(player);
        }
    }

    public static void TriggerAudioEventAfterDelay(SOUND_EVENT soundEvent, float delay) {
        singleton.StartCoroutine(singleton.triggerEventAfterDelay(soundEvent, delay));
    }

    private IEnumerator triggerEventAfterDelay(SOUND_EVENT soundEvent, float delay) {
        yield return new WaitForSeconds(delay);
        TriggerAudioEvent(soundEvent);
    }

    // Play the appropriate sounds of a trigger event
    private static void PlayEventSounds(SOUND_EVENT soundEvent, bool positionProvided, Vector3 position, AudioClip clip = null) {
        // First trigger all interruptions
        foreach (SoundContainer container in singleton.sounds) {
            if (container.triggerEvent == soundEvent) {
                foreach (SOUND_INTERUPTION_TYPE interruptionEvent in container.triggerInterruptions)
                    TriggerInterruptionEvent(interruptionEvent);
            }
        }

        // Then play all sounds
        foreach (SoundContainer container in singleton.sounds) {
            if (container.triggerEvent == soundEvent) {
                if (container.sound)
                    Debug.Log("Playing: "+ container.sound.name);

                // Create gameobject and transfer properties
                GameObject soundObject = new GameObject("Sound player");
                SoundPlayer player = soundObject.AddComponent<SoundPlayer>();
                player.source = soundObject.AddComponent<AudioSource>();
                player.triggerEvent = soundEvent;
                player.interuptionTypes = container.interuptions;
                player.source.playOnAwake = false;
                if (clip == null)
                    player.source.clip = container.sound;
                else
                    player.source.clip = clip;
                player.source.volume = container.volume;
                player.source.loop = container.loop;
                player.source.spatialBlend = (container.audioSpace == SoundContainer.AUDIO_SPACE._2D ? 0 : 1);
                if (container.soundObject) {
                    player.transform.parent = container.soundObject;
                    player.transform.localPosition = Vector3.zero;
                } else if (positionProvided && container.audioSpace == SoundContainer.AUDIO_SPACE._3D)
                    player.transform.position = position;
                player.source.Play();
                singleton.currentSources.Add(player);
            }
        }
    }

    // Ensures the sound plays as intended
    public static void TriggerAudioEvent(SOUND_EVENT soundEvent) {
        // Halt interruptable sounds
        InterruptSources(soundEvent);

        // Play sound
        PlayEventSounds(soundEvent, false, Vector3.zero);
    }

    // Ensures the sound plays as intended
    public static void TriggerAudioEvent(SOUND_EVENT soundEvent, Vector3 position) {
        // Halt interruptable sounds
        InterruptSources(soundEvent);

        // Play sound
        PlayEventSounds(soundEvent, true, position);
    }

    // Ensures the sound plays as intended
    public static void TriggerAudioEvent(SOUND_EVENT soundEvent, Vector3 position, AudioClip clip)
    {
        // Halt interruptable sounds
        InterruptSources(soundEvent);

        // Play sound
        PlayEventSounds(soundEvent, true, position, clip);
    }

    private void Update() {
        // Check if any sources have stopped, and if so delete them
        for (int i = currentSources.Count - 1; i >= 0; i--) {
            SoundPlayer player = currentSources[i];
            if (!player.source.isPlaying) {
                StopSound(player);
            }
        }
    }
}