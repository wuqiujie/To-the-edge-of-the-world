using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reference: https://medium.com/@michel.brisis/off-axis-projection-in-unity-1572d826541e#:~:text=%20Off-axis%20projection%20in%20Unity%20%201%20The,plane.%20This...%204%20Next%20time.%20%20More%20
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class OffAxisProjector : MonoBehaviour
{
    [Tooltip("The projection plane we're looking at")]
    public ProjectionPlane lookAt;

    // Component imports
    private Camera cam;

    // Screen corner vectors
    private Vector3 bl;
    private Vector3 br;
    private Vector3 tl;
    private Vector3 tr;

    // Start is called before the first frame update
    void Start()
    {
        // Component imports
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Ensure projection plane is up to date
        lookAt.projectionPoints();

        // Ensure camera is looking directly towards the center
        //transform.LookAt(lookAt.transform.position);

        // Get vectors from eye to projection screen corners
        bl = lookAt.BL - transform.position;
        br = lookAt.BR - transform.position;
        tl = lookAt.TL - transform.position;
        tr = lookAt.TR - transform.position;

        Vector3 viewDirection = transform.position + bl + br + tl + tr;

        // Get distance from eye to projection plane to clamp near plane (so nothing between is visible)
        float d = -Vector3.Dot(bl, lookAt.normal);
        cam.nearClipPlane = d;

        // Construct camera frustum
        Matrix4x4 P = Matrix4x4.Frustum(
            Vector3.Dot(lookAt.right, bl),
            Vector3.Dot(lookAt.right, br),
            Vector3.Dot(lookAt.up, bl),
            Vector3.Dot(lookAt.up, tl),
            cam.nearClipPlane,
            cam.farClipPlane);

        // Translate to eye position
        Matrix4x4 T = Matrix4x4.Translate(-transform.position);
        Matrix4x4 R = Matrix4x4.Rotate(Quaternion.Inverse(transform.rotation)*lookAt.transform.rotation);

        // Assign projection matrix values
        cam.worldToCameraMatrix = lookAt.projectionPoints() * R * T;
        cam.projectionMatrix = P;


    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, tr);
        Gizmos.DrawRay(transform.position, tl);
        Gizmos.DrawRay(transform.position, br);
        Gizmos.DrawRay(transform.position, bl);

    }
}
