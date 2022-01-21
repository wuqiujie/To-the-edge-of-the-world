using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveTransform : MonoBehaviour
{
    public static CaveTransform mostRecent;

    // Start is called before the first frame update
    void Start()
    {
        mostRecent = this;
    }
}
