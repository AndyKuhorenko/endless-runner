using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private float rocketSpeed;
    void Start()
    {
        int random = Random.Range(1, 3);

        rocketSpeed = random / 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + rocketSpeed, transform.position.z);
    }
}
