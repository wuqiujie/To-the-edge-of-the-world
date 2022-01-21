using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpacebarActions : MonoBehaviour
{
    // The most recently instantiated instance
    private static SceneSpacebarActions mostRecent;

    [Tooltip("The list of actions spacebar does in this scene.")]
    public List<GameManager.GAME_EVENTS> spacebarEvents;

    private int currentSpacebarEvent = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Update the most recently instantiated instance
        mostRecent = this;
    }

    public static void TriggerNextSpacebarAction()
    {
        if (mostRecent.currentSpacebarEvent < mostRecent.spacebarEvents.Count)
        {
            mostRecent.currentSpacebarEvent++;
            GameManager.TriggerGameEvent(mostRecent.spacebarEvents[mostRecent.currentSpacebarEvent-1]);
        }
    }
}
