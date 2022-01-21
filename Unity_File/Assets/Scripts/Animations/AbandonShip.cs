using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbandonShip : MonoBehaviour, GameEventListener
{
    private const float JUMP_TIME = 3f;
    private float timeSinceJumpStart = 0f;

    public Transform targetLandSpot;

    private static List<AbandonShip> instances;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.RegisterEventListener(this);

        if (instances == null)
            instances = new List<AbandonShip>();
        instances.Add(this);
    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent)
    {
        /*if (gameEvent == GameManager.GAME_EVENTS.ERVIN_AND_CHAD_LEAVE)
        {
            StartCoroutine(Jump());
        }*/
    }

    public static void AllLeave()
    {
        foreach (AbandonShip instance in instances)
            instance.Leave();
    }

    public void Leave()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator Jump()
    {
        transform.parent = null;
        timeSinceJumpStart = 0;
        Vector3 startPos = transform.position;

        // Parabolic trajectory
        Vector3 horizontalDistance = Vector3.ProjectOnPlane(targetLandSpot.transform.position, Vector3.up) - Vector3.ProjectOnPlane(transform.position, Vector3.up);
        float h = startPos.y - targetLandSpot.transform.position.y;
        float b = 2 - (h / 2);
        while (timeSinceJumpStart < JUMP_TIME)
        {
            timeSinceJumpStart += Time.deltaTime;

            float dx = timeSinceJumpStart / JUMP_TIME; // 0 to 1
            float dy = -Mathf.Pow(2 * dx, 2) + b * (2 * dx) + h; // start height to 0
            transform.position = startPos + horizontalDistance * dx;
            transform.position = new Vector3(transform.position.x, targetLandSpot.transform.position.y + dy, transform.position.z);

            yield return new WaitForEndOfFrame();
        }
    }
}
