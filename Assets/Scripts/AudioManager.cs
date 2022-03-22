using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource playerJump;
    [SerializeField] AudioSource zombieHit;
    [SerializeField] AudioSource obstacleHit;

    [SerializeField] AudioSource menuMusic;
    [SerializeField] AudioSource gameMusic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayJump()
    {
        playerJump.Play();
    }

    public void PlayZombieHit()
    {
        zombieHit.Play();
    }
    
    public void PlayObstacleHit()
    {
        obstacleHit.Play();
    }

    public void PlayMenu()
    {
        gameMusic.Pause();
        menuMusic.Play();
    }

    public void PlayGame()
    {
        menuMusic.Stop();
        gameMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
