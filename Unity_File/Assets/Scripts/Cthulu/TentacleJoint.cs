using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleJoint : MonoBehaviour, Attackable
{
    [Tooltip("What's our root?")]
    public Tentacle tentacle;

    public void onHit() {
        tentacle.KillTentacle();
    }
}
