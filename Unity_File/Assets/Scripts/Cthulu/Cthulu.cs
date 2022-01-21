using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cthulu : MonoBehaviour, GameEventListener
{
    [Tooltip("A container for all our messy parts.")]
    public GameObject cthuluParent;

    [Tooltip("How long does it take Cthulu to reach the top?")]
    public float RiseTime = 8f;

    // Initial y of the parent
    private float parentInitY;

    // Start is called before the first frame update
    void Start()
    {
        // Register as a listener
        GameManager.RegisterEventListener(this);

        // Init vars
        parentInitY = cthuluParent.transform.localPosition.y;
    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent)
    {
        switch (gameEvent)
        {
            case GameManager.GAME_EVENTS.CTHULU_RISE:
                StartCoroutine(Rise());
                SoundManager.TriggerAudioEvent(SoundManager.SOUND_EVENT.START_CTHULU_BATTLE);
                break;
            case GameManager.GAME_EVENTS.LOWER_CTHULU:
                StartCoroutine(Lower());
                SoundManager.TriggerInterruptionEvent(SoundManager.SOUND_INTERUPTION_TYPE.END_CTHULU_BATTLE);
                break;
        }
    }

    private IEnumerator Rise()
    {
        cthuluParent.SetActive(true);

        while (cthuluParent.transform.localPosition.y < 0)
        {
            cthuluParent.transform.localPosition += Vector3.up * Time.deltaTime * Mathf.Abs(parentInitY) / RiseTime;
            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator Lower()
    {

        while (cthuluParent.transform.localPosition.y > parentInitY)
        {
            cthuluParent.transform.localPosition -= Vector3.up * Time.deltaTime * Mathf.Abs(parentInitY) / (5f);// 5 Second Retreat
            yield return new WaitForEndOfFrame();
        }

        cthuluParent.SetActive(false);
        GameManager.TriggerGameEvent(GameManager.GAME_EVENTS.CTHULU_GONE);

    }
}
