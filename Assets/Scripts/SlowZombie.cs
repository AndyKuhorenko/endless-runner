using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowZombie : Enemy
{
    [SerializeField] float speed = 1f;
    [SerializeField] Animator animator;

    public void PlayDeathAnimation()
    {
        animator.SetBool("isDead", true);

        GetComponent<AudioSource>().Stop();
        GetComponent<CapsuleCollider>().enabled = false;

        speed = 0;

        Destroy(gameObject, 10f);
    }

    void Update()
    {
        float newPosZ = transform.position.z - speed * Time.deltaTime;

        transform.position = new Vector3(transform.position.x, trackHeight, newPosZ);
    }
}
