using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugOutputCanvas : MonoBehaviour
{
    // Singleton pattern
    private static DebugOutputCanvas singleton;

    // Component imports
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton pattern
        if (!singleton)
            singleton = this;

        // Component imports
        text = GetComponent<Text>();
    }

    /// <summary>
    /// Sets the debug canvas output text to the provided text.
    /// </summary>
    /// <param name="text">The provided text</param>
    public static void OutputText(string text) {
        singleton.text.text = text;
    }
}
