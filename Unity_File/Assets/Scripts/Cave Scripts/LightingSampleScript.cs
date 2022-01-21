using UnityEngine;
using System.Collections;

public class LightingSampleScript : MonoBehaviour
{
		private float lightStartTime = 0f;
		private float lightFireRate = .1f;
		Color lerpedColor;
		public bool readInput = true;
		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (!readInput) {
						//this sets the Wall lights to gradually change from red to green
						if (Time.time > lightStartTime + lightFireRate) {
								float lerpDuration = 5f;
								float lerp = Mathf.PingPong (Time.time, lerpDuration) / lerpDuration;
								
//								Color32 from1 = new Color32 (255f, 0f, 128f);
//								Color32 to1 = new Color32 (0f, 128f, 0f);
								Color from = new Color (0f / 255f, 0f / 255f, 128f / 255f);
								Color to = new Color (0f / 255f, 128f / 255f, 0f / 255f);
								lerpedColor = Color.Lerp (from, to, lerp);
								DMXController.Lighting.TurnOn ("Walls", lerpedColor, 0, 255);
								lightStartTime = Time.time;
						}		
				} else {
						//reads inputs to change the lighting
						if (Input.GetKeyDown (KeyCode.Space)) {
								DMXController.Lighting.Blackout ();
						}
						if (Input.GetKeyDown (KeyCode.R)) {
								DMXController.Lighting.TurnOn ("Walls", Color.red, 0, 255);
						}
						if (Input.GetKeyDown (KeyCode.B)) {
								DMXController.Lighting.TurnOn ("ThemeRGB", Color.blue, 0, 255);
						}
						if (Input.GetKeyDown (KeyCode.G)) {
								DMXController.Lighting.TurnOn ("CaveRGB", Color.green, 0, 255);
						}
				}

		}
}
