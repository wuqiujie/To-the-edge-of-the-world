using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using System;

[RequireComponent(typeof(Dropdown))]
public class SetTrackerDropdown : MonoBehaviour
{
    [Tooltip("The tracker we set values for.")]
    public SteamVR_TrackedObject tracker;

    // Component imports
    private Dropdown dropdown;

    private List<SteamVR_TrackedObject.EIndex> indexValues;

    // Start is called before the first frame update
    void Start()
    {
        // Component imports
        dropdown = GetComponent<Dropdown>();

        // Init vars
        indexValues = new List<SteamVR_TrackedObject.EIndex>();

        // Fill in dropdown with values
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (SteamVR_TrackedObject.EIndex index in Enum.GetValues(typeof(SteamVR_TrackedObject.EIndex))) {
            options.Add(new Dropdown.OptionData(index.ToString()));
            indexValues.Add(index);
        }
        dropdown.options = options;

        selectOption();
    }

    public void selectOption() {
        tracker.index = indexValues[dropdown.value];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
