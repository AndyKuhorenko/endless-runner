using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float gravity = 9.8f;
    private const float maxRotation = 30f;
    private const float offsetPosX = 1f;

    private CharacterController controller;
    private GameObject body;

    private enum Track {
        Left = -1,
        Middle,
        Right,
    };

    [SerializeField] private float runningSpeed = 2f;

    private float changeTrackSpeed = 2f;
    private float verticalVelocity = 0f;
    private Vector3 moveVector; 

    private float rotation = 0f;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        body = GameObject.FindGameObjectWithTag("PlayerBody");
    }

    void Update()
    {
        ProcessMoving();
    }

    private void ProcessMoving()
    {
        moveVector = Vector3.zero;

        SetVerticalVelocity();

        HandleInputs();

        ProcessRotating();

        moveVector.z = runningSpeed;

        moveVector.y = verticalVelocity;

        controller.Move(moveVector * Time.deltaTime);
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
    }

    private void RotateToLeft()
    {
        float movingSpeed = runningSpeed * Time.deltaTime;

        rotation -= movingSpeed * 40;

        if (rotation <= -maxRotation)
        {
            rotation = -maxRotation;
        }

        body.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void RotateToRight()
    {
        float movingSpeed = runningSpeed * Time.deltaTime;

        rotation += movingSpeed * 40;

        if (rotation >= maxRotation)
        {
            rotation = maxRotation;
        }

        body.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void RotateToCenter()
    {
        float movingSpeed = runningSpeed * Time.deltaTime;

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

    public float GetPosZ()
    {
        return transform.position.z;
    }
}
