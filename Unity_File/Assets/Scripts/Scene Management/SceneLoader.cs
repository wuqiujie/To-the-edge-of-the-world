using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour, GameEventListener
{
    [Tooltip("All the scenes in the game.")]
    public List<string> scenes;

    [Tooltip("The renderers which fade to white for a scene transition.")]
    public List<Image> fadeToWhiteImages;

    [Tooltip("How long does it take for a scene transition?")]
    public float sceneTransitionTime = 3;

    [Tooltip("The end credits PNG")]
    public Texture2D endCredits;

    // The index of the current scene
    private static int sceneIndex = 0;

    // Singleton pattern
    private static SceneLoader singleton;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton pattern
        if (!singleton)
        {
            singleton = this;

            // Register as game event listener (listen for the next scene event)
            GameManager.RegisterEventListener(this);
        }
    }

    // Check if we've hit a nextscene event
    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent){
        if (gameEvent == GameManager.GAME_EVENTS.NEXT_SCENE)
            nextScene();
        else if (gameEvent == GameManager.GAME_EVENTS.FADE_TO_BLACK)
            StartCoroutine(FadeToCredits());
        else if (gameEvent == GameManager.GAME_EVENTS.ERVIN_AND_CHAD_LEAVE)
            StartCoroutine(chadAndErvinLeaveFade());
    }

    public static int getSceneIndex() {
        return sceneIndex;
    }

    public static void nextScene()
    {
        DialogueActor.resetActors();//TODO: Make this cleaner
        singleton.StartCoroutine(singleton.FadeToNextScene());
    }

    // Things are misnamed here because I copy-pasted, suck it up there isn't much time
    public IEnumerator chadAndErvinLeaveFade()
    {
        // How white is the scene currently?
        float whiteAmount = 0;

        // Enable the fade renderers
        foreach (Image img in fadeToWhiteImages)
        {
            img.enabled = true;
            img.color = new Color(0, 0, 0, whiteAmount);
        }

        // Wait until we reach white
        while (whiteAmount < 1)
        {
            whiteAmount += Time.deltaTime * 2 / sceneTransitionTime;
            whiteAmount = Mathf.Min(whiteAmount, 1);
            foreach (Image img in fadeToWhiteImages)
                img.color = new Color(0, 0, 0, whiteAmount);
            yield return new WaitForEndOfFrame();
        }

        // Once we've hit white, transition to the next scene
        AbandonShip.AllLeave();

        // Start fading back down
        while (whiteAmount > 0)
        {
            whiteAmount -= Time.deltaTime * 2 / sceneTransitionTime;
            whiteAmount = Mathf.Max(whiteAmount, 0);
            foreach (Image img in fadeToWhiteImages)
                img.color = new Color(0, 0, 0, whiteAmount);
            yield return new WaitForEndOfFrame();
        }

        // Disable the fade renderers
        foreach (Image img in fadeToWhiteImages)
            img.enabled = false;

        // So all code paths return a value lol
        yield return new WaitForEndOfFrame();
    }

    // Things are misnamed here because I copy-pasted, suck it up there isn't much time
    public IEnumerator FadeToCredits()
    {
        // Trigger end sounds
        SoundManager.TriggerAudioEvent(SoundManager.SOUND_EVENT.END_CREDITS);

        // How white is the scene currently?
        float whiteAmount = 0;

        // Enable the fade renderers
        foreach (Image img in fadeToWhiteImages)
        {
            img.enabled = true;
            img.material.mainTexture = endCredits;
            img.color = new Color(1, 1, 1, whiteAmount);
        }

        // Wait until we reach white
        while (whiteAmount < 1)
        {
            whiteAmount += Time.deltaTime * 2 / sceneTransitionTime;
            whiteAmount = Mathf.Min(whiteAmount, 1);
            foreach (Image img in fadeToWhiteImages)
                img.color = new Color(1, 1, 1, whiteAmount);
            yield return new WaitForEndOfFrame();
        }

        // So all code paths return a value lol
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator FadeToNextScene() {
        // How white is the scene currently?
        float whiteAmount = 0;

        // Enable the fade renderers
        foreach (Image img in fadeToWhiteImages) {
            img.enabled = true;
            img.color = new Color(0, 0, 0, whiteAmount);
            for(int i = 0; i < img.transform.childCount; i++)
                img.transform.GetChild(i).gameObject.SetActive(true);
        }

        // Wait until we reach white
        while (whiteAmount < 1) {
            whiteAmount += Time.deltaTime * 2 / sceneTransitionTime;
            whiteAmount = Mathf.Min(whiteAmount, 1);
            foreach (Image img in fadeToWhiteImages)
                img.color = new Color(0, 0, 0, whiteAmount);
            yield return new WaitForEndOfFrame();
        }

        // Once we've hit white, transition to the next scene
        sceneIndex++;
        sceneIndex = sceneIndex % scenes.Count;
        SceneManager.LoadScene(scenes[sceneIndex]);

        // Start fading back down
        while (whiteAmount > 0) {
            whiteAmount -= Time.deltaTime * 2 / sceneTransitionTime;
            whiteAmount = Mathf.Max(whiteAmount, 0);
            foreach (Image img in fadeToWhiteImages)
                img.color = new Color(0, 0, 0, whiteAmount);
            yield return new WaitForEndOfFrame();
        }

        // Disable the fade renderers
        foreach (Image img in fadeToWhiteImages)
        {
            img.enabled = false;
            for (int i = 0; i < img.transform.childCount; i++)
                img.transform.GetChild(i).gameObject.SetActive(false);
        }

        // So all code paths return a value lol
        yield return new WaitForEndOfFrame();
    }
}
