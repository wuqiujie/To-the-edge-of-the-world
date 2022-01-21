using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLandSpot : MonoBehaviour
{
    // List of all land spots in the scene
    private static List<EnemyLandSpot> allSpots;

    private bool free = true;

    // Start is called before the first frame update
    void Start()
    {
        // Init vars
        if (allSpots == null)
            allSpots = new List<EnemyLandSpot>();
        allSpots.Add(this);
    }

    public static EnemyLandSpot getNearestFree(Vector3 position)
    {
        EnemyLandSpot nearest = null;

        foreach (EnemyLandSpot landSpot in allSpots)
        {
            if (landSpot.isFree())
            {
                if (nearest == null)
                    nearest = landSpot;
                else if (Vector3.Distance(position, landSpot.transform.position) < Vector3.Distance(position, nearest.transform.position))
                    nearest = landSpot;
            }
        }

        return nearest;
    }

    public void fillSpot() { free = false; }

    public void spotEmptied() { free = true; }

    public bool isFree() { return free; }

    // Update is called once per frame
    void Update()
    {
        
    }
}
