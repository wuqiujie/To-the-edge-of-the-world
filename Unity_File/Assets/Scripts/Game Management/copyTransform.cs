using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copyTransform : MonoBehaviour
{
    [Tooltip("The transform which we copy")]
    public Transform transformToCopy;

    // Update is called once per frame
    void Update()
    {
        transform.position = transformToCopy.position;
        transform.rotation = transformToCopy.rotation;
    }
}
