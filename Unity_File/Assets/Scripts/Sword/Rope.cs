using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour, GameEventListener
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.RegisterEventListener(this);
    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent) {
        if (gameEvent == GameManager.GAME_EVENTS.BOAT_FREED) {
            SoundManager.TriggerAudioEvent(SoundManager.SOUND_EVENT.START_SHANTY);
            Destroy(gameObject);
        }
    }
}
