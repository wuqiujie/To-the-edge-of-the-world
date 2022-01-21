using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour, GameEventListener
{
    private const float MIN_MINION_SPAWN_INTERVAL = 2f;

    [Tooltip("What's the ideal position to attack the player boat?")]
    public Transform relativeAttackPosition;

    [Tooltip("How quickly do we traverse the water")]
    public float moveSpeed = 10f;

    [Tooltip("What's the minimum move speed")]
    public float minMoveSpeed = 3f;

    [Tooltip("How many minions does this ship have?")]
    public int nMinions = 10;

    [Tooltip("Areas where the minions can spawn")]
    public List<Transform> minionSpawnPositions;

    [Tooltip("The minion prefab")]
    public GameObject minion;

    private bool attack = false;
    private bool inAttackRange = false;

    private float initialDistance;

    private Vector3 attackPos;
    private static int nEnemyShips = 0;
    private bool attacking = false;
    private static bool spotted = false;


    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent)
    {

        // Start approaching when the boat is freed
        if (gameEvent == GameManager.GAME_EVENTS.BOAT_FREED)
        {
            GameManager.TriggerGameEvent(GameManager.GAME_EVENTS.START_ENEMY_PIRATE_APPROACH);
        }

        if (gameEvent == GameManager.GAME_EVENTS.START_ENEMY_PIRATE_APPROACH)
            attack = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Init vars
        attackPos = relativeAttackPosition.position;
        initialDistance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(attackPos.x, attackPos.z));
        nEnemyShips += 1;

        GameManager.RegisterEventListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (attack)
        {
            if (!inAttackRange)
            {
                // Move towards enemy
                // Get everything in 2D to ignore waves
                Vector2 pos = new Vector2(transform.position.x, transform.position.z);
                Vector2 attPos = new Vector2(attackPos.x, attackPos.z);
                float remainingDistance = (pos - attPos).magnitude;

                // Check if we've been spotted
                if (remainingDistance / initialDistance < 0.03f && !spotted)
                {
                    spotted = true;
                    GameManager.TriggerGameEvent(GameManager.GAME_EVENTS.ENEMY_PIRATES_VISIBLE);
                    SoundManager.TriggerAudioEvent(SoundManager.SOUND_EVENT.SCENE_1_BATTLE_START);
                }

                // Lerp the speed for a slower finish
                float currentSpeed = Mathf.Max(minMoveSpeed, moveSpeed * remainingDistance / initialDistance);
                if (remainingDistance > currentSpeed * Time.deltaTime)
                {
                    Vector2 moveDir = (attPos - pos).normalized * currentSpeed * Time.deltaTime;
                    transform.position += new Vector3(moveDir.x, 0, moveDir.y);
                }
                else
                {
                    transform.position = new Vector3(attackPos.x, transform.position.y, attackPos.z);
                    inAttackRange = true;
                }
            }else if (!attacking){
                // Spawn minions to attack the player
                attacking = true;
                StartCoroutine(spawnEnemies());
            }
        }
    }

    private IEnumerator spawnEnemies() {

        while (nMinions > 0) {
            foreach (Transform trans in minionSpawnPositions)
                if (EnemyLandSpot.getNearestFree(trans.position)!=null) {
                    if (nMinions > 0) {
                        Instantiate(minion, trans.position, trans.rotation, trans);
                        nMinions -= 1;
                        if (nMinions <= 0)
                            break;
                        yield return new WaitForSeconds(MIN_MINION_SPAWN_INTERVAL);
                    }  
                } else
                    yield return new WaitForSeconds(MIN_MINION_SPAWN_INTERVAL);
        }

        // Ship is out of enemies
        StartCoroutine(Sink());
    }

    private const float SINK_TIME = 30f;
    private const float SINK_HEIGHT = 40f;
    private IEnumerator Sink() {
        float sinkTime = 0;
        GetComponent<Buoyancy>().enabled = false;

        while (sinkTime < SINK_TIME) {
            sinkTime += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y - SINK_HEIGHT * Time.deltaTime / SINK_TIME, transform.position.z);

            yield return new WaitForEndOfFrame();
        }

        nEnemyShips -= 1;
        if (nEnemyShips == 0)
            GameManager.TriggerGameEvent(GameManager.GAME_EVENTS.ALL_ENEMIES_DEFEATED);
    }
}
