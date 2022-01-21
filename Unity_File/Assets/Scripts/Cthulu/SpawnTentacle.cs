using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTentacle : MonoBehaviour, GameEventListener
{
    [Tooltip("Tentacle prefab")]
    public GameObject TentaclePrefab;

    private static bool attacking = false;

    private const float MIN_SPAWN_TIME = 2f;

    private const float MAX_SPAWN_TIME = 5f;

    private const float INITIAL_SPAWN_TIME = 15f;

    private float lastSpawnTime = 0f;


    private float nextSpawnGoalTime;

    private bool childAlive = false;

    private void Start()
    {
        GameManager.RegisterEventListener(this);

        // Init vars
        nextSpawnGoalTime = Mathf.Lerp(MIN_SPAWN_TIME, MAX_SPAWN_TIME, Random.value);
        lastSpawnTime = -INITIAL_SPAWN_TIME;

    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent)
    {
        if (gameEvent == GameManager.GAME_EVENTS.CTHULU_RISE)
            attacking = true;
        else if (gameEvent == GameManager.GAME_EVENTS.LOWER_CTHULU) 
            attacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (attacking)
        {
            lastSpawnTime += Time.deltaTime;

            if (lastSpawnTime > nextSpawnGoalTime && !childAlive)
            {
                spawnTentacle();
                lastSpawnTime = 0f;
                nextSpawnGoalTime = Mathf.Lerp(MIN_SPAWN_TIME, MAX_SPAWN_TIME, Random.value);
            }
        }
    }

    private void spawnTentacle()
    {
        GameObject tentacle = Instantiate(TentaclePrefab, transform.position, transform.rotation);
        tentacle.GetComponent<Tentacle>().setAttackArea(transform.parent);
        tentacle.GetComponent<Tentacle>().setSpawner(this);
        childAlive = true;
    }

    public void tentacleDied()
    {
        childAlive = false;
    }
}
