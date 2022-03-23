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

    private float touchX;
    private float touchY;

    private bool hasWeapon = false;
    private const int shootRange = 15;
    private int ammo = 100;
    private bool canShoot = true;
    private float timeBetweenShots = 0.2f;

    private bool godMode = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        body = GameObject.FindGameObjectWithTag("PlayerBody");
        SetCurrentAmmo();
    }

    void Update()
    {
        ProcessMoving();
        UpdateScore();
    }

    private void ProcessRaycast()
    {
        RaycastHit hit;

        Vector3 shotOrigin = new Vector3(body.transform.position.x, body.transform.position.y + 0.6f, body.transform.position.z);

        if (Physics.Raycast(shotOrigin, body.transform.forward, out hit, shootRange))
        {
            Debug.DrawLine(shotOrigin, hit.point, Color.red, 20f, false);
            Debug.Log(hit.transform.gameObject.name);
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
        HandleFireInputs();

        float vectorX = Input.GetAxisRaw("Horizontal");

        isMovingLeft = vectorX < 0;
        isMovingRight = vectorX > 0;

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
                        vectorX = 1;
                        isMovingRight = true;
                    }
                    else if ((touch.position.x + 40) < touchX)
                    {
                        vectorX = -1;
                        isMovingLeft = true;
                    }
                    break;
                case TouchPhase.Ended:
                    vectorX = 0;
                    isMovingLeft = false;
                    isMovingRight = false;
                    break;
                case TouchPhase.Canceled:
                    vectorX = 0;
                    isMovingLeft = false;
                    isMovingRight = false;
                    break;
                default:
                    break;
            }
        }

        // Stop moving if in max right or left position on road
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

    private void HandleFireInputs()
    {
        if (Input.GetMouseButtonDown(0) && canShoot && UI.currentState == GameState.Active)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        if (ammo > 0)
        {
            ProcessRaycast();

            audioManager.PlayShot();

            ammo--;
            SetCurrentAmmo();
        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
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

    public void PickWeapon()
    {
        if (!hasWeapon)
        {
            hasWeapon = true;
        }

        ammo++;

        print(ammo);
    }

    private void SetCurrentAmmo()
    {
        ui.SetAmmoCount(ammo);
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

    public float GetPosZ()
    {
        return transform.position.z;
    }
}
