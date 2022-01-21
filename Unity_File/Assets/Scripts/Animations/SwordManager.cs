using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManager : MonoBehaviour
{
    [Tooltip("Left Sword")]
    public GameObject leftSword;

    [Tooltip("Left Sword Holster Transform")]
    public Transform leftSwordHolster;

    [Tooltip("Left Sword Hand Transform")]
    public Transform leftSwordHand;

    [Tooltip("Right Sword")]
    public GameObject rightSword;

    [Tooltip("Right Sword Holster Transform")]
    public Transform rightSwordHolster;

    [Tooltip("Right Sword Hand Transform")]
    public Transform rightSwordHand;

    public enum SWORD { LEFT, RIGHT}

    // Start is called before the first frame update
    void Start()
    {
        HolsterSword(SWORD.LEFT);
        HolsterSword(SWORD.RIGHT);
    }

    public void HolsterAfterDelay(SWORD hand, float delay)
    {
        StartCoroutine(triggerAfterDelay(true, hand, delay));
    }

    private IEnumerator triggerAfterDelay(bool holster, SWORD hand, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (holster)
            HolsterSword(hand);
        else
            GrabSword(hand);
    }

    public void GrabAfterDelay(SWORD hand, float delay)
    {
        StartCoroutine(triggerAfterDelay(false, hand, delay));
    }

    public void GrabSword(SWORD hand)
    {
        if (hand == SWORD.LEFT)
        {
            if (leftSword != null)
            {
                leftSword.transform.parent = leftSwordHand;
                leftSword.transform.localPosition = Vector3.zero;
                leftSword.transform.localRotation = Quaternion.identity;
            }
        }else if (hand == SWORD.RIGHT)
        {
            if (rightSword != null)
            {
                rightSword.transform.parent = rightSwordHand;
                rightSword.transform.localPosition = Vector3.zero;
                rightSword.transform.localRotation = Quaternion.identity;
            }
        }
    }

    public void HolsterSword(SWORD hand)
    {
        if (hand == SWORD.LEFT)
        {
            if (leftSword != null)
            {
                leftSword.transform.parent = leftSwordHolster;
                leftSword.transform.localPosition = Vector3.zero;
                leftSword.transform.localRotation = Quaternion.identity; 
            }
        }
        else if (hand == SWORD.RIGHT)
        {
            if (rightSword != null)
            {
                rightSword.transform.parent = rightSwordHolster;
                rightSword.transform.localPosition = Vector3.zero;
                rightSword.transform.localRotation = Quaternion.identity;
            }
        }
        
    }
}
