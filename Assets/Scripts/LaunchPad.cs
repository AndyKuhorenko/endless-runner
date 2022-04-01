using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField] GameObject[] launchPrefabs;

    private float launchPadHeight = 1.1f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = launchPrefabs[Random.Range(0, launchPrefabs.Length)];

        Instantiate(prefab, new Vector3(transform.position.x, transform.position.y + launchPadHeight, transform.position.z), Quaternion.identity, transform);
    }
}
