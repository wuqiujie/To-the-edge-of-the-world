using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Tooltip("The renderer whose values we're using.")]
    public MeshRenderer waterRenderer;

    [Tooltip("The height map of the first waves texture.")]
    public Texture2D waveHeightMap1;

    [Tooltip("The height map of the second waves texture.")]
    public Texture2D waveHeightMap2;

    [Tooltip("What's the distance between the forward samples.")]
    public float forwardSampleDistance = 12.5f;
    
    [Tooltip("What's the distance between the right/left samples.")]
    public float rightSampleDistance = 12.5f;

    [Tooltip("How quickly do we move to desired look vectors?")]
    public float rotateSpeed = 1f;

    /// <summary>
    /// TODO: FIX THIS HACKY SOLUTION LATER
    /// </summary>
    [Tooltip("Hacky vertical offset solution to buoyancy positoin problems")]
    public float verticalOffsetCorrection;

    // The positions of the forward samples
    private Vector3[] samplePositions;

    // Start is called before the first frame update
    void Start()
    {
        // Init vars
        samplePositions = new Vector3[4];
    }

    private void Update()
    {
        // Ensure the shader's time is synced with our own
        waterRenderer.material.SetFloat("_TimeCustom", Time.time);

        // Get the height at the four key locations
        samplePositions[0] = sampleAtRelativePoint(transform.forward, forwardSampleDistance);
        samplePositions[1] = sampleAtRelativePoint(-transform.forward, forwardSampleDistance);
        samplePositions[2] = sampleAtRelativePoint(transform.right, rightSampleDistance);
        samplePositions[3] = sampleAtRelativePoint(-transform.right, rightSampleDistance);

        // Part of the hacky solution
        for (int i = 0; i < 4; i++)
            samplePositions[i] += Vector3.down * verticalOffsetCorrection;

        // Place us at the right height (average of the four)
        float average = 0;
        for (int i = 0; i < 4; i++)
            average += samplePositions[i].y / 4f;
        transform.position = new Vector3(transform.position.x, average, transform.position.z);

        // Angle us forward and right correctly
        Vector3 forwardVector = samplePositions[0] - samplePositions[1];
        Vector3 rightVector = samplePositions[2] - samplePositions[3];
        Vector3 smoothedForward = Vector3.Lerp(transform.forward, forwardVector.normalized, Time.deltaTime * rotateSpeed);
        Vector3 smoothedRight = Vector3.Lerp(transform.right, rightVector.normalized, Time.deltaTime * rotateSpeed);
        transform.LookAt(transform.position + smoothedForward, Vector3.Cross(smoothedForward, smoothedRight));
    }

    /// <summary>
    /// Gets the position of a height sample at a point relative to the transform
    /// </summary>
    /// <param name="sampleDirection">The direction from the transform's origin to recieve the point</param>
    /// <param name="sampleDistance">How far in that direction do we go to sample a point?</param>
    /// <returns>An exact position of the wave in that direction</returns>
    private Vector3 sampleAtRelativePoint(Vector3 sampleDirection, float sampleDistance)
    {
        Vector3 samplePos = (new Vector3(sampleDirection.x, 0, sampleDirection.z)).normalized * sampleDistance;
        float desiredY = heightFunctionYCoord(transform.position + samplePos);
        return( new Vector3(transform.position.x + samplePos.x, desiredY, transform.position.z + samplePos.z));
    }

    // Recreate height function from water shader
    private float heightFunction(Vector2 uv, Texture2D tex, float waveHeight)
    {
        return (tex.GetPixelBilinear(uv.x,uv.y)).r * waveHeight;
    }

    private Vector2 heightFunctionUV(Vector3 worldPos, Vector4 direction, float speed, float scale)
    {
        Vector3 uvXZ = (worldPos + Time.time * 3 * (new Vector3(direction.x, direction.y, direction.z)) * speed);
        Vector2 uv = new Vector2(uvXZ.x, uvXZ.z);
        uv /= scale;
        uv = new Vector2(uv.x % 1, uv.y % 1);
        return uv;
    }

    private float heightFunction(Vector3 worldPos, float _WaveHeight, float _WaveHeight2)
    {
        Vector4 _FlowDirection = waterRenderer.material.GetVector("_FlowDirection");
        //Vector4 _FlowDirection2 = waterRenderer.material.GetVector("_FlowDirection2");
        float _FlowSpeed = waterRenderer.material.GetFloat("_FlowSpeed");
        //float _FlowSpeed2 = waterRenderer.material.GetFloat("_FlowSpeed2");
        float _WorldSpaceTilingScale = waterRenderer.material.GetFloat("_WorldSpaceTilingScale");
        //float _WorldSpaceTilingScale2 = waterRenderer.material.GetFloat("_WorldSpaceTilingScale2");


        Vector2 uv = heightFunctionUV(worldPos, _FlowDirection, _FlowSpeed, _WorldSpaceTilingScale);
        //Vector2 uv2 = heightFunctionUV(worldPos, _FlowDirection2, _FlowSpeed2, _WorldSpaceTilingScale2);

        return heightFunction(uv, waveHeightMap1, _WaveHeight);// + heightFunction(uv2, waveHeightMap2, _WaveHeight2);
    }

    /// <summary>
    /// Gets the y coordinate of our water given world space coordinates
    /// </summary>
    /// <param name="worldPos">The world space coordinates we're testing for</param>
    /// <returns>The corresponding y value</returns>
    private float heightFunctionYCoord(Vector3 worldPos)
    {
        float _WaveHeight = waterRenderer.material.GetFloat("_WaveHeight");
        float _WaveHeight2 = waterRenderer.material.GetFloat("_WaveHeight2");

        return (waterRenderer.transform.position.y + waterRenderer.transform.lossyScale.y*(heightFunction(worldPos, _WaveHeight, _WaveHeight2) - ((_WaveHeight + _WaveHeight2) / 2)));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (samplePositions != null && samplePositions.Length>=4)
            for (int i = 0; i < 4; i++)
                Gizmos.DrawSphere(samplePositions[i], 1f);
    }
}
