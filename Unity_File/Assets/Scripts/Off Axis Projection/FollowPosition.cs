using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{

    [Tooltip("The thing whose position we follow")]
    public Transform followObject;

    // Update is called once per frame
    void Update()
    {
        transform.position = followObject.position;
    }
}
