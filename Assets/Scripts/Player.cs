using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float gravity = 14.8f;
    private const float maxRotation = 30f;
    private const float offsetPosX = 1f;

    private CharacterController controller;
    private GameObject body;

    [SerializeField] private Animator animator;

    [SerializeField] private float runningSpeed = 6f;

    [SerializeField] private UI ui;

    [SerializeField] private TileManager tileManager;

    [SerializeField] private AudioManager audioManager;

    private float changeTrackSpeed = 3f;
    private float verticalVelocity = 0f;
    private const float jumpVelocity = 4f;
    private const float jumpHeight = 0.5f;
    private Vector3 moveVector;

    private float rotation = 0f;

    private bool isJumping = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    private float score = 0f;
    private float lastUpdatedScore = 0f;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        body = GameObject.FindGameObjectWithTag("PlayerBody");
    }

    void Update()
    {
        ProcessMoving();
        UpdateScore();
    }

    private void ProcessMoving()
    {
        moveVector = Vector3.zero;

        if (isJumping)
        {
            ProcessJumping();
        }
        else
        {
            SetVerticalVelocity();
        }


        HandleInputs();

        ProcessRotating();

        moveVector.z = runningSpeed;

        moveVector.y = verticalVelocity;

        controller.Move(moveVector * Time.deltaTime);
    }

    private void UpdateScore()
    {
        score = transform.position.z * 2;

        ui.SetScoresTextNumber(score);

        if (lastUpdatedScore > 1600) return;

        if (score - lastUpdatedScore > 400)
        {
            runningSpeed += 1f;
            changeTrackSpeed += 0.25f;
            lastUpdatedScore += 400;

            tileManager.IncreaseObstacleSpawnChance();
        }

    }

    private void ProcessJumping()
    {
        animator.SetBool("isJumping", isJumping);

        audioManager.PlayJump();

        verticalVelocity = jumpVelocity;

        if (transform.position.y > jumpHeight)
        {
            isJumping = false;
        }
    }

    private void ProcessRotating()
    {
        if (isMovingLeft)
        {
            RotateToLeft();
        }
        else if (isMovingRight)
        {
            RotateToRight();
        }
        else
        {
            RotateToCenter();
        }
    }


    private void SetVerticalVelocity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f;
            animator.SetBool("isJumping", isJumping);
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    private void HandleInputs()
    {
        float vectorX = Input.GetAxisRaw("Horizontal");

        isMovingLeft = vectorX < 0;
        isMovingRight = vectorX > 0;

        if (transform.position.x >= offsetPosX && isMovingRight)
        {
            isMovingRight = false;

            moveVector.x = 0;
        }
        else if (transform.position.x <= -offsetPosX && isMovingLeft)
        {
            isMovingLeft = false;

            moveVector.x = 0;
        }
        else
        {
            moveVector.x = vectorX * changeTrackSpeed;
        }


        if (controller.isGrounded && !isJumping)
        {
            if (Input.GetKeyDown("w") || Input.GetKeyDown("space") || Input.GetKeyDown("up"))
            {
                isJumping = true;
            }
        }
        

    }

    private void RotateToLeft()
    {
        float movingSpeed = changeTrackSpeed * Time.deltaTime;

        rotation -= movingSpeed * 80;

        if (rotation <= -maxRotation)
        {
            rotation = -maxRotation;
        }

        body.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void RotateToRight()
    {
        float movingSpeed = changeTrackSpeed * Time.deltaTime;

        rotation += movingSpeed * 80;

        if (rotation >= maxRotation)
        {
            rotation = maxRotation;
        }

        body.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void RotateToCenter()
    {
        float movingSpeed = changeTrackSpeed * Time.deltaTime;

        if (rotation < 0f)
        {
            rotation += movingSpeed * 100;

            if (rotation >= 0f) rotation = 0f;
        }
        else if (rotation > 0f)
        {
            rotation -= movingSpeed * 100;

            if (rotation <= 0f) rotation = 0f;
        }
        else
        {
            rotation = 0f;
        }

        body.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    public void AddRunningSpeed(float addedSpeed)
    {
        runningSpeed += addedSpeed;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.tag == "ObstaclePrefab")
        {
            if (UI.currentState != GameState.Fail)
            {
                ui.SetFailGameState();
                audioManager.PlayObstacleHit();
            }
        }
        else if (hit.collider.tag == "Enemy")
        {
            AudioSource zombieSound = hit.collider.gameObject.GetComponentInChildren<AudioSource>();

            zombieSound.Stop();

            if (UI.currentState != GameState.Fail)
            {
                ui.SetFailGameState();
                audioManager.PlayZombieHit();
            }
        }
    }

    public float GetPosZ()
    {
        return transform.position.z;
    }
}
