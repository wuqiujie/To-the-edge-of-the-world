using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [Tooltip("The origin of the sword transform coordinates")]
    public Transform swordSpaceOrigin;

    [Tooltip("Spawn distance of the slash")]
    public float slashStartDistance = 5f;

    [Tooltip("Minimum speed for a slash to be counted")]
    public float slashMinSpeed = 30f;

    [Tooltip("How many times a second we update the slash")]
    public float slashDetail = 10f;

    [Tooltip("The slash prefab")]
    public GameObject slashPrefab;

    [Tooltip("The slash effect prefab")]
    public GameObject slashEffect;

    // How quickly they move through the air
    public static float SLASH_SPEED = 100f;

    // The last slash update
    private float lastSlashUpdate = 0f;

    //[Tooltip("Minimum velocity for slash to draw")]
    //public float slashMinAngularVelocity = 5f;

    // The position of the slash at the last measurement
    private Vector3 lastSlashPos = new Vector3();

    // Are we currently drawing a slash
    private bool slashing = false;

    // Where did the slash start?
    private Vector3 slashStartPos;

    // The current slash prefab
    private Slash currentSlash;

    // Start is called before the first frame update
    void Start()
    {
        // Init vars
        lastSlashPos = swordSpaceOrigin.InverseTransformPoint(transform.position + transform.forward * slashStartDistance);
    }

    // Update is called once per frame
    void Update()
    {
        // Determine current speed of slash
        Vector3 currentSlashPos = swordSpaceOrigin.InverseTransformPoint(transform.position + transform.forward * slashStartDistance);
        float speed = (currentSlashPos - lastSlashPos).magnitude / Time.deltaTime;

        if (speed > slashMinSpeed && !slashing) {
            startSlash();
        }else if (speed < slashMinSpeed && slashing) {
            endSlash();
        }

        // Update vars
        lastSlashPos = currentSlashPos;

        // Rendering
        if (slashing) {
            lastSlashUpdate += Time.deltaTime;
            if (lastSlashUpdate > 1f/slashDetail) {
                currentSlash.updateSlash(swordSpaceOrigin.TransformPoint(lastSlashPos), transform.forward);
                lastSlashUpdate = 0f;
            }
        }
    }

    private void startSlash() {
        slashing = true;
        currentSlash = Instantiate(slashPrefab).GetComponent<Slash>();
        currentSlash.setSlashPrefab(slashPrefab);
        currentSlash.setOrigin(swordSpaceOrigin.TransformPoint(lastSlashPos));
        currentSlash.updateSlash(swordSpaceOrigin.TransformPoint(lastSlashPos), transform.forward);
    }
    
    private void endSlash() {
        slashing = false;
        currentSlash.release();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
    }
}
