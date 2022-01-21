using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour, GameEventListener
{
    // How long does it take a tentacle to get to its attack zone?
    private const float TRAVEL_TIME = 10f;
    private float moveSpeed;

    enum ACTION_STATE { TRAVELLING, ATTACKING, DYING}
    private ACTION_STATE actionState = ACTION_STATE.TRAVELLING;
    private Transform attackArea;
    private SpawnTentacle spawner;
    private float originalY;



    public void setSpawner(SpawnTentacle daddy)
    {
        spawner = daddy;
    }

    public void setAttackArea(Transform attackArea)
    {
        this.attackArea = attackArea;
        moveSpeed = (transform.position - attackArea.transform.position).magnitude / TRAVEL_TIME;
        transform.rotation = attackArea.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;

        GameManager.RegisterEventListener(this);
    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent) {
        if (gameEvent == GameManager.GAME_EVENTS.LOWER_CTHULU)
            KillTentacle();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dying) {
            switch (actionState) {
                case ACTION_STATE.TRAVELLING:
                    if ((attackArea.position - transform.position).magnitude > moveSpeed * Time.deltaTime)
                        transform.position += (attackArea.position - transform.position).normalized * moveSpeed * Time.deltaTime;
                    else {
                        transform.position = attackArea.transform.position;
                        actionState = ACTION_STATE.ATTACKING;
                    }
                    break;
                case ACTION_STATE.ATTACKING:
                    ContractTentacle(Mathf.Cos(Time.time) * 20 * Time.deltaTime);
                    break;
            }
        }

    }

    private bool dying = false;
    public void KillTentacle() {
        actionState = ACTION_STATE.DYING;

        // Check if we're already dying
        if (dying)
            return;
        dying = true;


        StartCoroutine(Die());

    }

    private IEnumerator Die()
    {
        SoundManager.TriggerAudioEvent(SoundManager.SOUND_EVENT.HIT_SEA_MONSTER);
        spawner.tentacleDied();

        while (transform.position.y > originalY) {
            transform.position -= Vector3.up * Time.deltaTime * 8f;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }


    private void ContractTentacle(float amount, Transform trans)
    {
        trans.Rotate(trans.up, amount);
        if (trans.childCount > 0)
            ContractTentacle(amount, trans.GetChild(0));
    }

    private void ContractTentacle(float amount)
    {
        ContractTentacle(amount, transform);
    }
}
