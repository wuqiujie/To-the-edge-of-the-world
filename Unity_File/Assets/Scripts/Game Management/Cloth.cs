using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloth : MonoBehaviour, Attackable, GameEventListener
{
    [Tooltip("Manually hits the cloth, for debugging")]
    public bool manualTrigger;

    private bool isDestroyable = false;

    public void onHit()
    {
        if (isDestroyable)
        {
            GameManager.TriggerGameEvent(GameManager.GAME_EVENTS.EDGE_OF_THE_WORLD_REVEALED);
            Destroy(gameObject);
        }
    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent)
    {
        // Only become destructable when flemming begins his confession
        if (gameEvent == GameManager.GAME_EVENTS.EDGE_OF_WORD_HITABLE)
            isDestroyable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.RegisterEventListener(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (manualTrigger)
        {
            manualTrigger = false;
            onHit();
        }
    }
}
