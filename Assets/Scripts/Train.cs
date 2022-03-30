using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    private int trainSpeed = 6;

    // Start is called before the first frame update
    void Start()
    {
        trainSpeed = Random.Range(5, 10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + Vector3.left * trainSpeed * Time.deltaTime;
    }
}
