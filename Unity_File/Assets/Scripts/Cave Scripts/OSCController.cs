using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCController : MonoBehaviour
{
	public string remoteIp = "255.255.255.255";	// "127.0.0.1" = LOCAL "255.255.255.255" = BROADCAST
	public int sendToPort = 8000;
	public int listenerPort = 15308;

	string[] addrs;         // what "/name" message addresses to listen for
	GameObject[] notifyGO;  // what object to notify - the object must have a ReceivedOSCmessage(data:String) function

	private Osc oscHandler;
	private bool sendNeeded;	// Scooping issue - handler has no access to game objects
	private int sendToIndex;
	private string data;

	void OnDisable() {
		Debug.Log("closing OSC UDP socket in OnDisable");
		oscHandler.Cancel();
		oscHandler = null;
	}

	void Start() {
		sendNeeded = false;
		sendToIndex = 0;
		data = "";

		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init(remoteIp, sendToPort, listenerPort);

		oscHandler = GetComponent<Osc>();
		oscHandler.init(udp);

		// setup handlers
		if (addrs != null) {
			foreach (var addr in addrs) {
				oscHandler.SetAddressHandler(addr, RecMessage);
			}
		}

	}

	void Update() {

		// send a message to the notify object that data has been received
		if (sendNeeded) {
			notifyGO[sendToIndex].SendMessage("ReceivedOSCmessage", data);
			sendNeeded = false;
		}
	}

	void SendOSCMessage(string data) {
		// data is a string with the addr followed by the message parms seperated by " "'s
		// example: "/test1 TRUE 23 0.501 bla" 
		Debug.Log("sending: " + data);
		OscMessage oscM = Osc.StringToOscMessage(data);
		oscHandler.Send(oscM);
	}

	void RecMessage(OscMessage m) {

		//Debug.Log("--------------> OSC message received: ("+m+")");
		Debug.Log("--------------> OSC message received > " + Osc.OscMessageToString(m));

		// need the addr index to tell what object needs to be notified
		int i = -1;
		for (var a = 0; a < addrs.Length; a++) {
			if (addrs[a] == m.Address) i = a;
		}

		// save the index and the data, notify Update that a message needs to be sent out
		sendNeeded = true;
		sendToIndex = i;
		data = Osc.OscMessageToString(m);

		// >>>> TO DO: Messages could come in so fast that data could be lost - use lists
	}

}
