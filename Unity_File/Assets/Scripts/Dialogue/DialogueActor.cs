using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActor : MonoBehaviour, GameEventListener
{
    [Tooltip("Which actor is this?")]
    public DialogueManager.ACTOR actor;

    [Tooltip("The transforms used to turn the neck.")]
    public List<Transform> neckJoint;

    [Tooltip("The start animation event.")]
    public MultimodelAnimationManager.ANIMATION_EVENTS startPosition;

    [Tooltip("How quickly can they turn their neck?")]
    private float turnSpeed = 1f;

    [Tooltip("Where do they face when they attack?")]
    public Transform attackDirection;

    // Are they attacking?
    private bool attacking = false;

    // Used to get direct access to actor gameobjects
    private static Dictionary<DialogueManager.ACTOR, GameObject> actorDict;

    // Which way are they currently facing?
    private Vector3 lookDirection;

    // Where would they like to face?
    private Vector3 desiredDirection;

    // Which way are they currently facing their body?
    private Vector3 bodyDirection;

    // Where would they like to position their bodies?
    private Vector3 desiredBodyDirection;

    // What's our default up axis?
    private Vector3 upAxis;

    private bool lookAlongAxis = true;

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent) {
        if (gameEvent == GameManager.GAME_EVENTS.ERVIN_AND_CHAD_LEAVE)
            lookAlongAxis = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Init vars
        if (actorDict == null)
            actorDict = new Dictionary<DialogueManager.ACTOR, GameObject>();
        foreach (Transform neckj in neckJoint)
            if (neckj.gameObject.activeSelf) {
                lookDirection = neckj.up;// Weird values for weird models
                desiredDirection = lookDirection;
                upAxis = -neckj.right;// Weird values for weird models
                desiredBodyDirection = transform.forward;
                bodyDirection = desiredBodyDirection;
            }

        if (GetComponentInChildren<MultimodelAnimationManager>())
            GetComponentInChildren<MultimodelAnimationManager>().triggerAnimationEvent(startPosition);

        // Register to class
        if (!actorDict.ContainsKey(actor))
            actorDict.Add(actor, gameObject);
    }

    public static GameObject getActor(DialogueManager.ACTOR actor)
    {
        if (actor == DialogueManager.ACTOR.Player)
            return GameObject.FindGameObjectWithTag("Player");
        else
            return actorDict[actor];
    }

    public void faceAttackDirection()
    {
        attacking = true;
        lookat(attackDirection);
    }

    public void releaseAttackDirection()
    {
        attacking = false;
    }

    public static void allLookAtSpeaker(DialogueManager.ACTOR speaker) {
        foreach (DialogueManager.ACTOR actor in actorDict.Keys)
            if (actor != speaker)
                getActor(actor).GetComponent<DialogueActor>().lookat(getActor(speaker).transform);
    }

    public static void allLookAtExcept(DialogueManager.ACTOR lookTarget, DialogueManager.ACTOR exception) {
        foreach (DialogueManager.ACTOR actor in actorDict.Keys)
            if (actor != exception)
                getActor(actor).GetComponent<DialogueActor>().lookat(getActor(lookTarget).transform);
    }

    public static void resetActors() {
        actorDict = new Dictionary<DialogueManager.ACTOR, GameObject>();
    }

    public void lookat(Transform other) {
        Transform lookTransform = other;
        if (attacking)
            lookTransform = attackDirection;
        desiredDirection = (lookTransform.position - transform.position).normalized;
        lookWithBody(lookTransform);
    }

    public void lookWithBody(Transform other)
    {
        desiredBodyDirection = Vector3.ProjectOnPlane((other.position - transform.position), Vector3.up).normalized;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (desiredBodyDirection != bodyDirection)
        {
            bodyDirection = Vector3.RotateTowards(bodyDirection, desiredBodyDirection, turnSpeed * Time.deltaTime, 0f);
        }
        transform.LookAt(transform.position + bodyDirection, Vector3.up);

        if (lookAlongAxis) { 
            // Face the correct way
            if (desiredDirection != lookDirection) {
            
                lookDirection = Vector3.RotateTowards(lookDirection, desiredDirection, turnSpeed * Time.deltaTime, 0f);
            }
            foreach(Transform neckj in neckJoint)
                if (neckj.gameObject.activeSelf)
                    neckj.LookAt(-Vector3.Cross(upAxis, lookDirection) + neckj.position, lookDirection);// Weird values for weird models
        }

    }
}
