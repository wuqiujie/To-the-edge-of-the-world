using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Slash : MonoBehaviour
{
    [Tooltip("Are we currently moving?")]
    public bool released = false;

    private GameObject slashPrefab;

    private const float LIFETIME = 3f;

    // Component imports
    private LineRenderer lineRenderer;

    private List<Vector3> forwards;
    private List<BoxCollider> edgeColliders;

    private bool init = false;

    public void setSlashPrefab(GameObject prefab) {
        slashPrefab = prefab;
    }

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    void Init() {
        if (!init) {
            // Component imports
            lineRenderer = GetComponent<LineRenderer>();

            // Init vars
            forwards = new List<Vector3>();
            edgeColliders = new List<BoxCollider>();
        }
        init = true;
    }

    public void setOrigin(Vector3 origin) {
        transform.position = origin;
    }


    public void updateSlash(Vector3 pos, Vector3 forward) {
        if (!init)
            Init();

        lineRenderer.positionCount += 1;
        lineRenderer.SetPosition(lineRenderer.positionCount-1, pos);
        forwards.Add(forward);

        // Add edge collider
        if (lineRenderer.positionCount >= 2) {
            BoxCollider boxCol = (new GameObject("Edge Collider")).AddComponent<BoxCollider>();
            boxCol.gameObject.AddComponent<SlashCollider>();
            boxCol.transform.parent = transform;
            boxCol.isTrigger = true;
            // Place in center of last two points
            boxCol.transform.position = (lineRenderer.GetPosition(lineRenderer.positionCount-2) + lineRenderer.GetPosition(lineRenderer.positionCount-1))/2f;
            Vector3 edgeVec = lineRenderer.GetPosition(lineRenderer.positionCount-1) - lineRenderer.GetPosition(lineRenderer.positionCount-2);
            boxCol.transform.forward = edgeVec.normalized;
            boxCol.center = Vector3.zero;
            boxCol.size = new Vector3(1, 1, edgeVec.magnitude);
            edgeColliders.Add(boxCol);
        }
    }

    public void release() {
        // determine average forward
        Vector3 average = Vector3.zero;
        foreach (Vector3 dir in forwards)
            average += dir;

        released = true;

        // Destroy after lifetime
        Invoke("dissipate", LIFETIME);
    }

    public void onChildTriggerEnter(Collider other) {
        Attackable attackable = other.GetComponent<Attackable>();
        if (attackable != null) {
            Instantiate(slashPrefab, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            attackable.onHit();
            dissipate();
        }
    }

    private void dissipate() {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (released) {
            // Fly through space
            for (int i = 0; i < lineRenderer.positionCount; i++) {
                Vector3 pointPos = lineRenderer.GetPosition(i) + forwards[i].normalized * Time.deltaTime * Sword.SLASH_SPEED;
                lineRenderer.SetPosition(i, pointPos);

                // Update colliders
                if (i>=1) {
                    edgeColliders[i-1].transform.position = (lineRenderer.GetPosition(i) + lineRenderer.GetPosition(i-1))/2f;
                    Vector3 edgeVec = lineRenderer.GetPosition(i) - lineRenderer.GetPosition(i-1);
                    edgeColliders[i-1].transform.forward = edgeVec.normalized;
                    edgeColliders[i-1].center = Vector3.zero;
                    edgeColliders[i-1].size = new Vector3(1, 1, edgeVec.magnitude);
                }
            }
        }
    }
}
