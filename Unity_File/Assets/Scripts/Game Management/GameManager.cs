using System.Collections;
using UnityEngine;
using System.Collections.Generic;



public enum GameState
{
    Scene1,
    Scene2,
    Scene3,
    Scene4,
    Scene5
}
public class GameManager : MonoBehaviour
{
    static GameManager s_Instance;
    public static GameManager Instance => s_Instance;

    [Tooltip("Gameobjects to enable after a 0.5 second delay")]
    public List<GameObject> enableObjects;
    public List<System.Action> actions;

    // EventListeners
    private static List<GameEventListener> gameEventListeners = new List<GameEventListener>();

    public enum GAME_EVENTS {CTHULU_RISE, SPACEBAR_PRESSED, CTHULU_GONE, LIGHTHOUSE_SPOTTED, FLEMMING_MAD_AT_CREW_0, LOWER_CTHULU, NEXT_SCENE, RAISE_PLATFORM, LOWER_PLATFORM, BOAT_FREED, START_ENEMY_PIRATE_APPROACH, ALL_ENEMIES_DEFEATED,
    FLEMMING_ASKS_NAME, CREW_INTRODUCTIONS, TO_THE_EDGE_CHANT, ENEMY_PIRATES_VISIBLE, FLEMMING_COMMANDS_ATTACK, FLEMMING_CHANTS_AFTER_VICTORY,
    ERVIN_ANGRY, SELECT_PLAYER_RESPONSE, YES_RESPONSE, NO_RESPONSE, FLEMMING_ASKS_CHAD, ERVIN_AND_CHAD_LEAVE, FLEMMING_STARTS_CONFESSION, EDGE_OF_THE_WORLD_REVEALED, PLAYER_CHEERS_UP_CAPTAIN, FADE_TO_BLACK, EDGE_OF_WORD_HITABLE};

    private enum PLAYER_RESPONSE { YES, NO};
    private static PLAYER_RESPONSE response;

    public static void RegisterEventListener(GameEventListener eventListener)
    {
        gameEventListeners.Add(eventListener);
    }

    public static void TriggerGameEvent(GAME_EVENTS gameEvent){
        // Inform the event listeners
        for (int i = gameEventListeners.Count - 1; i >= 0; i--)
        {
            // TODO: Clean up unused event listeners
            gameEventListeners[i].TriggerGameEvent(gameEvent);
        }

        if (gameEvent == GAME_EVENTS.SELECT_PLAYER_RESPONSE)
            if (response == PLAYER_RESPONSE.YES)
                TriggerGameEvent(GAME_EVENTS.YES_RESPONSE);
            else
                TriggerGameEvent(GAME_EVENTS.NO_RESPONSE);
    }

    private void Start() {
        // Enable all the enable objects in time
        Invoke("enableEnableObjects", 0.5f);
        CreateList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpacePressed();
        }else if (Input.GetKeyDown(KeyCode.Y))
        {
            response = PLAYER_RESPONSE.YES;
            DebugOutputCanvas.OutputText("Player Response: YES");
        }else if (Input.GetKeyDown(KeyCode.N))
        {
            response = PLAYER_RESPONSE.NO;
            DebugOutputCanvas.OutputText("Player Response: NO");
        }
    }
    private void enableEnableObjects()
    {
        foreach (GameObject obj in enableObjects)
            obj.SetActive(true);
    }
    public int currentActionNum = 0;
    void CreateList()
    {
        actions = new List<System.Action>();
        actions.Add(action1);
        actions.Add(action2);
    }
    
    private void SpacePressed() {

        TriggerGameEvent(GAME_EVENTS.SPACEBAR_PRESSED);

        SceneSpacebarActions.TriggerNextSpacebarAction();

    }


    public void ChangeScene()
    {
        if (actions[currentActionNum] != null)
        { 
        actions[currentActionNum]();
        currentActionNum++;
        }
    }
    public void action1()
    {
    }
    public void action2()
    {
    }
    
}
