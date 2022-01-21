using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MultimodelAnimationManager : MonoBehaviour
{
    [Tooltip("The objects which categorize into different models")]
    public List<modelObjects> modelObjs;

    [Tooltip("How long after the FIGHT command does the character grab the left sword?")]
    public float leftSwordGrabTime = 3f;

    [Tooltip("How long after the FIGHT command does the character grab the right sword?")]
    public float rightSwordGrabTime = 3f;

    [Tooltip("How long after the END_FIGHT command does the character put away the left sword?")]
    public float leftSwordPutAwayTime = 3f;

    [Tooltip("How long after the END_FIGHT command does the character put away the right sword?")]
    public float rightSwordPutAwayTime = 3f;

    [Tooltip("How long after the END_FIGHT command does the character return to the idle animation?")]
    public float backToIDLETime = 3f;

    [Tooltip("Used for debugging, set an animation event and hit the bool to trigger")]
    public ANIMATION_EVENTS debugTriggerEventType;

    [Tooltip("Used for debugging, set an animation event and hit the bool to trigger the event above")]
    public bool debugTriggerEvent = false;

    [System.Serializable]
    public class modelObjects
    {
        public List<GameObject> objectsInModel;
    }

    public enum ANIMATION_EVENTS { FIGHT, STAND, END_FIGHT, CHEER, END_CHEER, SIT, SING, END_SING, ANGRY, END_ANGRY}
    private const ANIMATION_EVENTS DEFAULT_STATE = ANIMATION_EVENTS.STAND;

    private Dictionary<ANIMATION_EVENTS, int> modelMap;

    private SwordManager swordManager;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Init vars
        modelMap = new Dictionary<ANIMATION_EVENTS, int>();
        modelMap.Add(ANIMATION_EVENTS.FIGHT, 0);
        modelMap.Add(ANIMATION_EVENTS.STAND, 1 % modelObjs.Count);
        modelMap.Add(ANIMATION_EVENTS.SIT, 0);
        modelMap.Add(ANIMATION_EVENTS.END_FIGHT, 0);
        modelMap.Add(ANIMATION_EVENTS.CHEER, 0);
        modelMap.Add(ANIMATION_EVENTS.END_CHEER, 1 % modelObjs.Count);
        modelMap.Add(ANIMATION_EVENTS.SING, 0);
        modelMap.Add(ANIMATION_EVENTS.END_SING, 0);
        modelMap.Add(ANIMATION_EVENTS.ANGRY, 0);
        modelMap.Add(ANIMATION_EVENTS.END_ANGRY, 0);

        // Disable all models except the default
        enableCorrectModel(DEFAULT_STATE);

        animator = GetComponent<Animator>();
        swordManager = GetComponent<SwordManager>();
        
    }

    private void Update()
    {
        if (debugTriggerEvent)
        {
            triggerAnimationEvent(debugTriggerEventType);
            debugTriggerEvent = false;
        }
    }

    private void enableCorrectModel(ANIMATION_EVENTS animEvent)
    {
        for (int i = 0; i < modelObjs.Count; i++)
        {
            if (i == modelMap[animEvent])
            {
                foreach (GameObject gameObject in modelObjs[i].objectsInModel)
                    gameObject.SetActive(true);
            }
            else
            {
                foreach (GameObject gameObject in modelObjs[i].objectsInModel)
                    gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator triggerEventAfterDelay(ANIMATION_EVENTS animEvent, float delay)
    {
        yield return new WaitForSeconds(delay);
        triggerAnimationEvent(animEvent);
    }

    public void triggerAnimationEvent(ANIMATION_EVENTS animEvent)
    {
        enableCorrectModel(animEvent);
        switch (animEvent)
        {
            case ANIMATION_EVENTS.FIGHT:
                animator.SetTrigger("Fight");
                swordManager.GrabAfterDelay(SwordManager.SWORD.LEFT, leftSwordGrabTime);
                swordManager.GrabAfterDelay(SwordManager.SWORD.RIGHT, rightSwordGrabTime);
                GetComponentInParent<DialogueActor>().faceAttackDirection();
                break;
            case ANIMATION_EVENTS.CHEER:
                animator.SetTrigger("Cheer");
                break;
            case ANIMATION_EVENTS.END_CHEER:
                animator.SetTrigger("End Cheer");
                break;
            case ANIMATION_EVENTS.END_FIGHT:
                animator.SetTrigger("End Fight");
                GetComponentInParent<DialogueActor>().releaseAttackDirection();
                swordManager.HolsterAfterDelay(SwordManager.SWORD.LEFT, leftSwordPutAwayTime);
                swordManager.HolsterAfterDelay(SwordManager.SWORD.RIGHT, rightSwordPutAwayTime);
                StartCoroutine(triggerEventAfterDelay(ANIMATION_EVENTS.STAND, backToIDLETime));
                break;
            case ANIMATION_EVENTS.SIT:
                animator.SetTrigger("Sit");
                break;
            case ANIMATION_EVENTS.STAND:
                animator.SetTrigger("Stand");
                break;
            case ANIMATION_EVENTS.SING:
                animator.SetTrigger("Sing");
                break;
            case ANIMATION_EVENTS.END_SING:
                animator.SetTrigger("End Sing");
                break;
            case ANIMATION_EVENTS.ANGRY:
                animator.SetTrigger("Angry");
                break;
            case ANIMATION_EVENTS.END_ANGRY:
                animator.SetTrigger("End Angry");
                break;
        }
    }
}
