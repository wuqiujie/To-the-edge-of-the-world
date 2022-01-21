using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phidgets;
using Phidgets.Events;

public class FloorController : MonoBehaviour {

    // the motion floor uses four air bladders, in the corners of the floor, to raise it which are enumerated as follows:
    // 2 - front left		3 - front right
    // 0 - back left		1 - back right

    // Constants
    public const float MAX_VOLTAGE = 10; 

    private Analog motionFloor;

    // Singleton pattern
    public static FloorController singleton;


    // Use this for initialization
    void Start () {
        // attach the phidget that controls the floor
        motionFloor = new Analog();
        motionFloor.open();
        motionFloor.waitForAttachment(1000);
        // lower the floor at start - just in case a previous program left it up
        resetFloor();

        // Attach to singleton
        if (!singleton)
            singleton = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDisable() {
        // drop the floor
        Debug.Log("resetting floor in OnDisable");
        resetFloor();
        motionFloor.close();    // if you don't close the phidget then this phidget hangs
    }

    public void enable() {
        // the air bladders have to be enabled before they can be used
        for (int i = 0; i < 4; i++) {
            motionFloor.outputs[i].Enabled = true;
        }
    }

    public void disable() {
        // when done using the floor, and after it has been lowered, disable the bladders
        for (int i = 0; i < 4; i++) {
            motionFloor.outputs[i].Enabled = false;
        }
    }

    public void resetFloor() {
        // lowers the floor *** for safty, should be called at the start and end of all games ***
        enable();
        lowerFloor();
        disable();
    }

    public float getVoltage(int index) {
        // return the current voltage level of the provided air bladder
        return ((float)motionFloor.outputs[index].Voltage);
    }

    public void lowerFloor() {
        // lower the floor by setting all bladders voltages to zero - should be used at the start and end of games
        for (int i = 0; i < 4; i++) {
            motionFloor.outputs[i].Voltage = 0.0f;
        }
    }

    public void moveOne(int index, float voltage) {
        // set the voltage (translates to floor height) of the provided bladder to the provided voltage
        // voltage = 0.0F is fully lowered
        // voltage = 5.0F is raised halfway
        // voltage = 10.0F is raised fully
        motionFloor.outputs[index].Voltage = voltage;
    }

    public void moveAll(float voltage) {
        // for all four bladders set the provided voltage (translates to height) in a range of 0.0F - 10.0F
        for (int i = 0; i < 4; i++) {
            motionFloor.outputs[i].Voltage = voltage;
        }
    }

    public void raiseFront(float voltage) {
        // raise the bladders on the front side of the floor to the provided voltage and fully lower the bladders on the back side
        motionFloor.outputs[2].Voltage = voltage;
        motionFloor.outputs[3].Voltage = voltage;
        motionFloor.outputs[0].Voltage = 0.0F;
        motionFloor.outputs[1].Voltage = 0.0F;
    }

    public void raiseBack(float voltage) {
        // raise the bladders on the back side of the floor to the provided voltage and fully lower the bladders on the front side
        motionFloor.outputs[0].Voltage = voltage;
        motionFloor.outputs[1].Voltage = voltage;
        motionFloor.outputs[2].Voltage = 0.0F;
        motionFloor.outputs[3].Voltage = 0.0F;
    }

    public void raiseRight(float voltage) {
        // raise the bladders on the right side of the floor to the provided voltage and fully lower the bladders on the left side
        motionFloor.outputs[1].Voltage = voltage;
        motionFloor.outputs[3].Voltage = voltage;
        motionFloor.outputs[0].Voltage = 0.0F;
        motionFloor.outputs[2].Voltage = 0.0F;
    }

    public void raiseLeft(float voltage) {
        // raise the bladders on the left side of the floor to the provided voltage and fully lower the bladders on the right side
        motionFloor.outputs[0].Voltage = voltage;
        motionFloor.outputs[2].Voltage = voltage;
        motionFloor.outputs[1].Voltage = 0.0F;
        motionFloor.outputs[3].Voltage = 0.0F;
    }

    public void zeroVoltage() {
        // backwards compatability
        lowerFloor();
    }

    public void move(int index, float voltage) {
        // backwards compatability
        moveOne(index, voltage);
    }

}
