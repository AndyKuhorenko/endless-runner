using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowZombie : Enemy
{
    [SerializeField] float speed = 1f;

    void Update()
    {
        float newPosZ = transform.position.z - speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, trackHeight, newPosZ);
    }
}
