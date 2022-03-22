using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastZombie : MonoBehaviour
{
    [SerializeField] float speed = 3f;

    private const float trackHeight = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float newPosZ = transform.position.z - speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, trackHeight, newPosZ);
    }
}
