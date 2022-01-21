using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        GetComponentInParent<Slash>().onChildTriggerEnter(other);
    }
}
