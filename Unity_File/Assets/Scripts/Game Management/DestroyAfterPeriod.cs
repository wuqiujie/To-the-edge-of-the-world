using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterPeriod : MonoBehaviour
{
    [Tooltip("Lifetime")]
    public float lifetime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(dissapear());
    }

    private IEnumerator dissapear() {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
