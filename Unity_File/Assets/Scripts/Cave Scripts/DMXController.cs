using UnityEngine;
using System.Collections;

public class DMXController : MonoBehaviour
{
	private static DMXController _instance;
	private static GameObject osc; 

	public static DMXController Lighting {
			get {
					if (_instance == null)
							_instance = GameObject.FindObjectOfType<DMXController> ();
					return _instance;
			}
	}

	void Awake ()
	{
		osc = GameObject.Find("OSCMain");
	}

	void Start ()
	{
		//Blackout ();
		Invoke ("Blackout", 1);	// need to give the networking time to connect
	}

	void OnDisable ()
	{

	}

	void Update () 
	{
		// if the esacepe key is pressed then player is exiting - quick, send a message to the lighting manager 
		if (Input.GetKeyDown (KeyCode.Escape)) {
			UseShow ("theme");	// start up the theme lighting again
		}
	}
		
	public void Blackout ()
	{
		osc.SendMessage("SendOSCMessage", "/lighting operations blackout");
	}
	
	public void AllOn ()
	{
		osc.SendMessage("SendOSCMessage", "/lighting operations allOn");
	}
	
	public void TurnOn (string groupName, int red, int green, int  blue, int amber, int dimmer)
	{
		osc.SendMessage("SendOSCMessage", "/lighting color " + groupName + " "+red+" "+green+" "+blue+" "+amber+" "+dimmer);
	}
	
	public void TurnOn (string groupName, Color32 thisColor, int amber, int dimmer)
	{
		osc.SendMessage("SendOSCMessage", "/lighting color " + groupName + " "+thisColor.r+" "+thisColor.g+" "+thisColor.b+" "+amber+" "+dimmer);
	}
	
	public void TurnOn (string groupName, Color thisColor, int amber, int dimmer)
	{
		osc.SendMessage("SendOSCMessage", "/lighting color " + groupName + " "+thisColor.r*255+" "+thisColor.g*255+" "+thisColor.b*255+" "+amber+" "+dimmer);
	}

	public void TurnOff (string groupName)
	{
		osc.SendMessage("SendOSCMessage", "/lighting color " + groupName + " 0 0 0 0 0");
	}



/*------------------The methods below have not been tested--------------------------*/

    public void MoveVulture(int pan, int tilt, int finePan, int fineTilt)
    {
		osc.SendMessage("SendOSCMessage", "/lighting move vulture "+pan+" "+tilt+" "+finePan+" "+fineTilt);
	}

    /*thisColor is an integer from 0-255 with ranges that cover about 5 Colors. */

    public void TurnOnWaterLight (int thisColor, int rotation, int dimmer)
	{
		osc.SendMessage("SendOSCMessage", "/lighting color h20 "+thisColor+" "+rotation+" 0 0 "+dimmer);
	}

	public void UseCue (string cueName, int cueNumber)
	{
		osc.SendMessage("SendOSCMessage", "/lighting cue " + cueName + " " + cueNumber);
	}

	public void UseShow (string showName)
	{
		osc.SendMessage("SendOSCMessage", "/lighting show " + showName);
	}
}
