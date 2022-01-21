using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandObjects : MonoBehaviour, GameEventListener
{
    [Tooltip("How quickly do land objects move?")]
    public float moveSpeed = 5f;

    [Tooltip("What directio ndo land objects move?")]
    public Vector3 moveDir = Vector3.right;

    private static bool moving = false;

    void Start()
    {
        GameManager.RegisterEventListener(this);
    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent)
    {
        if (gameEvent == GameManager.GAME_EVENTS.BOAT_FREED)
            moving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).position += moveDir.normalized * moveSpeed * Time.deltaTime;
    }
}
