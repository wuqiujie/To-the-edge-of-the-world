using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BindToCaveFloor : MonoBehaviour, GameEventListener
{
    // Minimum time between floor updates
    private const float FLOOR_UPDATE_INTERVAL = 0.1f;

    [Tooltip("The medium height of the platform off the floor in meters")]
    public float MediumHeightOffGround = 0.29f;

    [Tooltip("How wide is the platform in meters")]
    public float PlatformWidth = 1.83f;

    [Tooltip("How long is the platform in meters")]
    public float PlatformLength = 1.85f;

    // Are we currently updating the floor?
    private static bool currentlyBound = false;

    // Used to keep track of the corners and their extensions
    private Vector3[] corners;
    private float[] cornerLengths;

    private void Awake()
    {
        // Init vars
        corners = new Vector3[4];
        cornerLengths = new float[4];

    }

    public void TriggerGameEvent(GameManager.GAME_EVENTS gameEvent) {
        switch (gameEvent) {
            case GameManager.GAME_EVENTS.RAISE_PLATFORM:
                bind();
                break;
            case GameManager.GAME_EVENTS.LOWER_PLATFORM:
                unBind();
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FloorController.singleton.resetFloor();

        // Register us to the game manager
        GameManager.RegisterEventListener(this);
    }

    /// <summary>
    /// Begins synchronizing us to the cave floor
    /// </summary>
    private void bind() { 
        currentlyBound = true;

        FloorController.singleton.enable();

        // Start the coroutine which updates the floor's position
        StartCoroutine(updateFloorPosition());
    }

    /// <summary>
    /// Lowers the floor and halts our synchronization
    /// </summary>
    private void unBind()
    {
        currentlyBound = false;
        StartCoroutine(waitThenLower());
    }

    /// <summary>
    /// Lowers sthe floor after a time period to ensure all other commands have been called
    /// </summary>
    private IEnumerator waitThenLower()
    {
        yield return new WaitForSeconds(FLOOR_UPDATE_INTERVAL * 1.5f);
        FloorController.singleton.resetFloor();

    }

    private IEnumerator updateFloorPosition()
    {
        while (currentlyBound)
        {
            yield return new WaitForSeconds(FLOOR_UPDATE_INTERVAL);

            // Set each edge to the height determined by this object
            for (int i = 0; i < 4; i++)
                FloorController.singleton.moveOne(i, FloorController.MAX_VOLTAGE * (cornerLengths[i] / (2*MediumHeightOffGround)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Set corner values
        // 2 - front left		3 - front right
        // 0 - back left		1 - back right
        corners[0] = transform.TransformPoint((-Vector3.forward * PlatformLength - Vector3.right * PlatformWidth)*0.5f);
        corners[1] = transform.TransformPoint((-Vector3.forward * PlatformLength + Vector3.right * PlatformWidth) * 0.5f);
        corners[2] = transform.TransformPoint((Vector3.forward * PlatformLength - Vector3.right * PlatformWidth) * 0.5f);
        corners[3] = transform.TransformPoint((Vector3.forward * PlatformLength + Vector3.right * PlatformWidth) * 0.5f);

        // Determine length of each corner piston
        if (cornerLengths.Length < 4)
            cornerLengths = new float[4];
        for (int i = 0; i < cornerLengths.Length; i++)
        {
            cornerLengths[i] = corners[i].y - transform.position.y + MediumHeightOffGround;
            if (cornerLengths[i] < 0)
            {
                // Don't bother correcting rotation, just getting the height right is good enough for our purposes
                corners[i] = new Vector3(corners[i].x, corners[i].y - cornerLengths[i], corners[i].z);
                cornerLengths[i] = 0;
            }else if (cornerLengths[i] > MediumHeightOffGround * 2)
            {
                // Don't bother correcting rotation, just getting the height right is good enough for our purposes
                corners[i] = new Vector3(corners[i].x, corners[i].y - cornerLengths[i] + MediumHeightOffGround * 2, corners[i].z);
                cornerLengths[i] = MediumHeightOffGround * 2;
            }
        }

    }

    private void OnDrawGizmos()
    {

        // Draw the corners and the lines
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.identity;
        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 corner = corners[i];
            Gizmos.DrawSphere(corner, 0.05f);
            if (cornerLengths.Length >= i)
                Gizmos.DrawLine(corner, corner - Vector3.up * cornerLengths[i]);
        }

        // Draw the ideal platform
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(PlatformLength, 0.05f, PlatformWidth));

        // Draw the actual platform
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.identity;
        for (int i = 0; i < corners.Length; i++)
            for (int j = i+1; j < corners.Length; j++)
                Gizmos.DrawLine(corners[i], corners[j]);
    }
}
