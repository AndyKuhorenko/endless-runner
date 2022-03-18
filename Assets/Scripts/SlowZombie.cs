using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowZombie : MonoBehaviour
{
    [SerializeField] float speed = 1f;

    private const float trackHeight = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // transform.position = new Vector3(0, 0.1f, 10);
    }

    // Update is called once per frame
    void Update()
    {
        float newPosZ = transform.position.z - speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, trackHeight, newPosZ);
    }
}
