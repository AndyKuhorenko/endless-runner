using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;


public class Player : MonoBehaviour
{
    private const float gravity = 14.8f;
    private const float maxRotation = 30f;
    private const float offsetPosX = 1f;

    private CharacterController controller;
    private GameObject body;

    [SerializeField] private bool godMode = true;

    [SerializeField] private Animator animator;

    [SerializeField] private float runningSpeed = 6f;

    [SerializeField] private int ammo = 0;

    [SerializeField] private UI ui;

    [SerializeField] private TileManager tileManager;

    [SerializeField] private AudioManager audioManager;

    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject ammoShell;
    [SerializeField] private ParticleSystem muzzleFlash;
    private RigBuilder weaponRigBuilder;

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
    private bool canShoot = true;
    private float timeBetweenShots = 0.5f;

    private int killedEnemies = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        weaponRigBuilder = GetComponentInChildren<RigBuilder>();
        body = GameObject.FindGameObjectWithTag("PlayerBody");

        SetCurrentAmmo();
    }

    void Update()
    {
        ProcessMoving();
        UpdateScore();
        MoveCrosshair();
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

            GameObject gunShell = Instantiate(ammoShell, weapon.transform.position, Quaternion.Euler(90, 0, 0));

            gunShell.GetComponent<Rigidbody>().AddForce(new Vector3(0.02f, 0.003f, 0.002f));

            Destroy(gunShell, 1.5f);

            muzzleFlash.Play();

            ammo--;
            SetCurrentAmmo();
        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    private void MoveCrosshair()
    {
        Vector3 weaponPos = weapon.transform.position;
        Vector3 aimPosForward = weapon.transform.forward;
        Vector3 aimPos = new Vector3(aimPosForward.x, aimPosForward.y, aimPosForward.z);

        Vector3 hit = weaponPos + aimPos;
        Debug.Log(aimPos);
        Debug.Log(hit.x);
        if (Input.GetMouseButton(1))
        {
            Debug.DrawLine(weaponPos, hit, Color.red, 50f, false);

        }

        ui.MoveCrosshair(aimPos);
    }

    private void ProcessRaycast()
    {

        Vector3 shotOrigin = weapon.transform.position;

        float rayThickness = 0.6f;

        // int layerMask = 1 << 2;

        // Collide with every layer except the second
        // layerMask = ~layerMask;

        int layerMask = 1 << 7; // Only with 7th layer

        RaycastHit[] hits = Physics.SphereCastAll(shotOrigin, rayThickness, weapon.transform.forward, shootRange, layerMask);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            Debug.Log(hit);
            Debug.DrawLine(shotOrigin, hit.point, Color.red, 20f, false);

            SlowZombie slowZombie = hit.transform.GetComponent<SlowZombie>();
            FastZombie fastZombie = hit.transform.GetComponent<FastZombie>();

            if (slowZombie)
            {
                slowZombie.PlayDeathAnimation(hit.point);
                killedEnemies++;
            }
            else if (fastZombie)
            {
                fastZombie.PlayDeathAnimation(hit.point);
                killedEnemies++;
            }

            SetCurrentKills();

            Debug.Log(hit.transform.gameObject.name);
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

    public void PickWeapon()
    {
        if (!hasWeapon)
        {
            hasWeapon = true;

            weaponRigBuilder.enabled = true;

            weapon.gameObject.SetActive(true);
        }

        ammo++;

        SetCurrentAmmo();
    }

    private void SetCurrentAmmo()
    {
        ui.SetAmmoCount(ammo);
    }

    private void SetCurrentKills()
    {
        ui.SetKillsText(killedEnemies);
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
