using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingRequests : MonoBehaviour {


    private Color[] colors = new Color[4] { Color.white, Color.red, Color.green, Color.blue };

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Blackout() {
        // send a blackout command to the lighting controller to turn all lights off
        DMXController.Lighting.Blackout();
    }

    public void SetLighting() {
        // send a command to the lighting controller to turn the set lights on
        DMXController.Lighting.UseCue("Tour", 3);
    }

    public void GuestLights(Color color) {
        // send a command to the lighting controller to turn the guest lights on
        DMXController.Lighting.TurnOn("caveRGB", color, 0, 255);
    }

    public void GuestLights(int i)
    {
        // send a command to the lighting controller to turn the guest lights on
        DMXController.Lighting.TurnOn("caveRGB", colors[i], 0, 255);
    }

}
