using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reference: https://medium.com/@michel.brisis/off-axis-projection-in-unity-1572d826541e#:~:text=%20Off-axis%20projection%20in%20Unity%20%201%20The,plane.%20This...%204%20Next%20time.%20%20More%20
/// </summary>

[ExecuteInEditMode]
public class ProjectionPlane : MonoBehaviour
{
    [HideInInspector]
    // 4 Corners
    public Vector3 BL, BR, TR, TL;

    [HideInInspector]
    // 3 Direction axis
    public Vector3 right, up, normal;

    // Returns a matrix containing all the info a camera needs for off-axis projection
    public Matrix4x4 projectionPoints() {
        Matrix4x4 pPoints = Matrix4x4.zero;

        // Get direction vectors
        right = (BR - BL).normalized;
        up = (TL - BL).normalized;
        normal = -Vector3.Cross(right, up).normalized;

        // Fill matrix
        pPoints[0, 0] = right.x;
        pPoints[0, 1] = right.y;
        pPoints[0, 2] = right.z;

        pPoints[1, 0] = up.x;
        pPoints[1, 1] = up.y;
        pPoints[1, 2] = up.z;

        pPoints[2, 0] = normal.x;
        pPoints[2, 1] = normal.y;
        pPoints[2, 2] = normal.z;

        pPoints[3, 3] = 1.0f;

        return pPoints;
    }

    private void Update() {
        // Get the four corners
        BL = transform.TransformPoint(-Vector3.right * 5 - Vector3.up * 5);
        BR = transform.TransformPoint(Vector3.right * 5 - Vector3.up * 5);
        TR = transform.TransformPoint(Vector3.right * 5 + Vector3.up * 5);
        TL = transform.TransformPoint(-Vector3.right * 5 + Vector3.up * 5);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(BL, 0.2f);
        Gizmos.DrawSphere(BR, 0.2f);
        Gizmos.DrawSphere(TR, 0.2f);
        Gizmos.DrawSphere(TL, 0.2f);
    }
}
