using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyMinion : MonoBehaviour, Attackable
{
    [Tooltip("Enable to kill manually.")]
    public bool kill = false;

    public enum ATTACK_STATE { WAITING_TO_JUMP, JUMPING, ATTACKING, DEAD}
    private ATTACK_STATE attackState;

    private const float JUMP_TIME = 1f;
    private float timeSinceJumpStart = 0f;

    private EnemyLandSpot targetLandSpot;

    // Component imports
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // init vars
        attackState = ATTACK_STATE.WAITING_TO_JUMP;

        // component imporsts
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (attackState)
        {
            case ATTACK_STATE.WAITING_TO_JUMP:
                targetLandSpot = EnemyLandSpot.getNearestFree(transform.position);
                if (targetLandSpot != null)
                {
                    targetLandSpot.fillSpot();
                    StartCoroutine(JumpTowardsLandSpot());
                    attackState = ATTACK_STATE.JUMPING;
                }
                break;
            case ATTACK_STATE.ATTACKING:
                // Face Player
                transform.LookAt(transform.position + Vector3.ProjectOnPlane(GameObject.FindGameObjectWithTag("Player").transform.position - transform.position, Vector3.up));
                break;
        }

        if (kill)
        {
            kill = false;
            onHit();
        }
    }


    private const float JUMP_BUILDUP_TIME = 3f;
    private IEnumerator JumpTowardsLandSpot()
    {
        animator.SetTrigger("Jump");
        yield return new WaitForSeconds(JUMP_BUILDUP_TIME);
        transform.parent = null;
        timeSinceJumpStart = 0;
        Vector3 startPos = transform.position;

        // Parabolic trajectory
        Vector3 horizontalDistance = Vector3.ProjectOnPlane(targetLandSpot.transform.position, Vector3.up) - Vector3.ProjectOnPlane(transform.position, Vector3.up);
        float h = startPos.y - targetLandSpot.transform.position.y;
        float b = 2-(h/2);
        while (timeSinceJumpStart < JUMP_TIME) {
            timeSinceJumpStart += Time.deltaTime;
            
            float dx = timeSinceJumpStart/JUMP_TIME; // 0 to 1
            float dy = -Mathf.Pow(2*dx, 2)+b*(2*dx)+h; // start height to 0
            transform.position = startPos + horizontalDistance * dx;
            transform.position = new Vector3(transform.position.x, targetLandSpot.transform.position.y + dy, transform.position.z);

            yield return new WaitForEndOfFrame();
        }

        if (attackState == ATTACK_STATE.JUMPING) {
            transform.parent = targetLandSpot.transform;
            transform.localPosition = Vector3.zero;
            animator.SetBool("Landed", true);
            attackState = ATTACK_STATE.ATTACKING;
        }
    }

    private const float DIE_TIME = 3f;
    private IEnumerator Die() {
        attackState = ATTACK_STATE.DEAD;
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(DIE_TIME);
        if (targetLandSpot != null) {
            targetLandSpot.spotEmptied();
            Destroy(gameObject);
        }
    }

    public void onHit() {
        SoundManager.TriggerAudioEvent(SoundManager.SOUND_EVENT.HIT_HUMAN);
        attackState = ATTACK_STATE.DEAD;
        StartCoroutine(Die());
    }
}
