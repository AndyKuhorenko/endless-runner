using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;


public class Player : MonoBehaviour
{
    private enum Track
    {
        Left = -1,
        Middle,
        Right,
    };

    private Track currentTrack = Track.Middle;

    private const float gravity = 14.8f;
    private const float maxRotation = 30f;
    private const float offsetPosX = 1f;

    private CharacterController controller;
    private GameObject body;

    [SerializeField] private bool godMode = true;

    [SerializeField] private Animator animator;

    [SerializeField] private float runningSpeed = 6f;

    [SerializeField] private  UI ui;

    [SerializeField] private TileManager tileManager;

    [SerializeField] private AudioManager audioManager;

    [SerializeField] private PlayerWeapon playerWeapon;

    private float changeTrackSpeed = 3f;
    private float verticalVelocity = 0f;
    private const float jumpVelocity = 4f;
    private const float jumpHeight = 0.5f;
    private Vector3 moveVector;

    private float rotation = 0f;

    private bool isJumping = false;
    private bool isFalling = false;

    private bool isMovingLeft = false;

    private bool isMovingRight = false;

    private float score = 0f;
    private float lastUpdatedScore = 0f;

    private float touchX;
    private float touchY;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        body = GameObject.FindGameObjectWithTag("PlayerBody");
    }

    void Update()
    {
        ProcessMoving();
        UpdateScore();

        if (isFalling)
        {
            FallInCrater();
        }
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

        // Stop moving
        if (transform.position.x >= offsetPosX && isMovingRight)
        {
            isMovingRight = false;

            currentTrack = Track.Right;

            moveVector.x = 0;
        }
        else if (transform.position.x <= -offsetPosX && isMovingLeft)
        {
            isMovingLeft = false;

            currentTrack = Track.Left;

            moveVector.x = 0;
        }
        else
        {
            if (isMovingLeft)
            {
                moveVector.x = -changeTrackSpeed;

                if (currentTrack == Track.Middle && transform.position.x < 0)
                {
                    isMovingLeft = false;

                    moveVector.x = 0;
                }
            }

            if (isMovingRight)
            {
                moveVector.x = changeTrackSpeed;

                if (currentTrack == Track.Middle && transform.position.x > 0)
                {
                    isMovingRight = false;

                    moveVector.x = 0;
                }
            }
        }

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
            runningSpeed += 0.5f;
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
        if (GetLeftInputKeys())
        {
            isMovingLeft = true;
            isMovingRight = false;

            switch (currentTrack)
            {
                case Track.Middle:
                    currentTrack = Track.Left;
                    break;
                case Track.Right:

                    // If character crossed the middle track stop at middle else return to left track
                    if (transform.position.x > 0)
                    {
                        currentTrack = Track.Middle;
                    }
                    else currentTrack = Track.Left;

                    break;
            }
        }
        
        if (GetRightInputKeys())
        {
            isMovingRight = true;
            isMovingLeft = false;

            switch (currentTrack)
            {
                case Track.Middle:
                    currentTrack = Track.Right;
                    break;
                case Track.Left:

                    // If character crossed the middle track stop at middle else return to right track
                    if (transform.position.x < 0)
                    {
                        currentTrack = Track.Middle;
                    }
                    else currentTrack = Track.Right;

                    break;
            }
        }

        // Mobile inputs
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchX = touch.position.x;
                    touchY = touch.position.y;
                    break;
                case TouchPhase.Moved:
                    if (touch.position.y - 200 > touchY)
                    {
                        if (controller.isGrounded && !isJumping) isJumping = true;
                    }
                    else if ((touch.position.x - 40) > touchX)
                    {
                        isMovingRight = true;
                    }
                    else if ((touch.position.x + 40) < touchX)
                    {
                        isMovingLeft = true;
                    }
                    break;
                case TouchPhase.Ended:
                    isMovingLeft = false;
                    isMovingRight = false;
                    break;
                case TouchPhase.Canceled:
                    isMovingLeft = false;
                    isMovingRight = false;
                    break;
                default:
                    break;
            }
        }

        if (controller.isGrounded && !isJumping)
        {
            if ((Input.GetKeyDown("w") || Input.GetKeyDown("space") || Input.GetKeyDown("up")) && !isFalling)
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

    private bool GetRightInputKeys()
    {
        return (Input.GetKeyDown("d") || Input.GetKeyDown("right")) && !isFalling;
    }

    private bool GetLeftInputKeys()
    {
        return (Input.GetKeyDown("a") || Input.GetKeyDown("left")) && !isFalling;
    }

    public void AddRunningSpeed(float addedSpeed)
    {
        runningSpeed += addedSpeed;
    }

    public void PickWeapon()
    {
        playerWeapon.PickWeapon();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (godMode) return;

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

    private void FallInCrater()
    {
        runningSpeed = 0f;
        body.transform.position = body.transform.position + Vector3.down * 2 * Time.deltaTime;

        animator.SetBool("isJumping", true);

        playerWeapon.DropWeapon();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (godMode) return;

        if (other.name.Contains("crater"))
        {
            isFalling = true;

            StartCoroutine(SetFailGameState());
        }
    }

    IEnumerator SetFailGameState()
    {
        yield return new WaitForSecondsRealtime(2.5f);

        if (UI.currentState != GameState.Fail) ui.SetFailGameState();
    }

    public float GetPosZ()
    {
        return transform.position.z;
    }
}
