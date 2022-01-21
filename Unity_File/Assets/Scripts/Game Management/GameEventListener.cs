using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameEventListener
{
    /// <summary>
    /// Reacts to a triggered game event
    /// </summary>
    /// <param name="gameEvent">The game event</param>
    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent);
}
