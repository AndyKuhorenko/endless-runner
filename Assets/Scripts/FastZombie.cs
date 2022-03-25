using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastZombie : Enemy
{
    [SerializeField] float speed = 3f;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem deathParticles;

    public void PlayDeathAnimation(Vector3 hitPos)
    {
        deathParticles.gameObject.transform.position = hitPos;
        deathParticles.Play();

        animator.speed = Random.Range(1, 1.5f);
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
