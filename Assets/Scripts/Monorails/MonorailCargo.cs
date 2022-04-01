using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonorailCargo : MonoBehaviour
{
    [SerializeField] GameObject[] envPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = envPrefabs[Random.Range(0, envPrefabs.Length)];

        GameObject cargo = Instantiate(prefab, new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.rotation, transform);

        cargo.transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
