using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveObjectBinding : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Bind our transform to the most recent one
        //Debug.Log(CaveTransform.mostRecent.name);
        transform.SetPositionAndRotation(CaveTransform.mostRecent.transform.position, CaveTransform.mostRecent.transform.rotation);
    }
}
