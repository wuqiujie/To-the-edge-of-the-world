using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour, GameEventListener
{
    public enum ACTOR { Flemming, Ervin, Chad, Player, Cthulu}

    // The most recently created dialogueManager
    private static DialogueManager mostRecent;

    [System.Serializable]
    public class CharacterAnimationEvent
    {
        [Tooltip("The character impacted by this event.")]
        public ACTOR character;

        [Tooltip("The animation event.")]
        public MultimodelAnimationManager.ANIMATION_EVENTS animEvent;
    }

    [System.Serializable]
    public class DialogueLine
    {
        [Tooltip("Who's saying the line?")]
        public ACTOR character;

        [Tooltip("The line audio file")]
        public AudioClip line;

        [Tooltip("Events which trigger at the start of a dialogue line")]
        public List<GameManager.GAME_EVENTS> triggerEventAtStart;

        [Tooltip("Events which trigger at the end of a dialogue line")]
        public List<GameManager.GAME_EVENTS> triggerEventAtEnd;

        [Tooltip("How long do we wait before continuning to the next line?")]
        public float nextLinePause = 0.5f;

        [Tooltip("Which character is the actor looking at while delivering their line?")]
        public ACTOR speakerLookAtCharacter;

        [Tooltip("Are the characters looking at the speaker?")]
        public bool othersLookingAtSpeaker = true;

        [Tooltip("Othwerwise, what are they looking at?")]
        public ACTOR everyoneLookAtCharacter;

        [Tooltip("Animation events triggered by the start of this line.")]
        public List<CharacterAnimationEvent> triggerAnimationEventsAtStart;

        [Tooltip("Animation events triggered by the end of this line.")]
        public List<CharacterAnimationEvent> triggerAnimationEventsAtEnd;
    }

    [System.Serializable]
    public class DialogueSequence
    {
        [Tooltip("A series of dialogue lines which play together in order.")]
        public List<DialogueLine> sequence;

        [Tooltip("The event which triggers this sequence.")]
        public GameManager.GAME_EVENTS triggeredBy;
    }

    [Tooltip("The list of dialogue sequences. A sequence is a series of dialogue lines which play in order.")]
    public List<DialogueSequence> Sequences;

    /// <summary>
    /// Gets the dialogue manager in the current scene
    /// </summary>
    /// <returns></returns>
    public static DialogueManager getInCurrentScene()
    {
        return mostRecent;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Update most recent manager
        mostRecent = this;

        // Register our existence to the game manager
        GameManager.RegisterEventListener(this);
    }

    private IEnumerator sayLine(DialogueSequence sequence, int line)
    {
        // Say the line
        DialogueLine dLine = sequence.sequence[line];
        SoundManager.TriggerAudioEvent(SoundManager.SOUND_EVENT.DIALOGUE, DialogueActor.getActor(dLine.character).transform.position, dLine.line);

        // Trigger the correct events at the start
        foreach (CharacterAnimationEvent cAnim in sequence.sequence[line].triggerAnimationEventsAtStart)
            DialogueActor.getActor(cAnim.character).GetComponentInChildren<MultimodelAnimationManager>().triggerAnimationEvent(cAnim.animEvent);

        // Everyone look at the person talking
        if (dLine.othersLookingAtSpeaker)
            DialogueActor.allLookAtSpeaker(dLine.character);
        else
            DialogueActor.allLookAtExcept(dLine.everyoneLookAtCharacter, dLine.character);

        // Person talking looks at whoever specified
        Transform lookTarget;
        if (dLine.speakerLookAtCharacter == ACTOR.Player)
            lookTarget = GameObject.FindGameObjectWithTag("Player").transform;
        else
            lookTarget = DialogueActor.getActor(dLine.speakerLookAtCharacter).transform;
        DialogueActor.getActor(dLine.character).GetComponent<DialogueActor>().lookat(lookTarget);

        // Trigger any simultaneous events
        foreach (GameManager.GAME_EVENTS gEvent in dLine.triggerEventAtStart)
            GameManager.TriggerGameEvent(gEvent);

        // Wait until the end of the line
        yield return new WaitForSeconds(dLine.line.length);

        // Trigger any following events
        foreach (GameManager.GAME_EVENTS gEvent in dLine.triggerEventAtEnd)
            GameManager.TriggerGameEvent(gEvent);

        // Trigger the correct events at the end
        foreach (CharacterAnimationEvent cAnim in sequence.sequence[line].triggerAnimationEventsAtEnd)
            DialogueActor.getActor(cAnim.character).GetComponentInChildren<MultimodelAnimationManager>().triggerAnimationEvent(cAnim.animEvent);

        // Check if this is the last line
        if (line >= sequence.sequence.Count - 1)
            yield return false;
        else {
            // Wait the appropriate period then say the next line
            yield return new WaitForSeconds(dLine.nextLinePause);
            StartCoroutine(sayLine(sequence, line + 1));
        }
    }

    private void StartSequence(DialogueSequence sequence)
    {
        StartCoroutine(sayLine(sequence, 0));
    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent)
    {
        foreach(DialogueSequence sequence in Sequences)
        {
            if (sequence.triggeredBy == gameEvent)
            {
                StartSequence(sequence);
            }
        }
    }
}
