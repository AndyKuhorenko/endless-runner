using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private CharacterController controller;
    private GameObject body;

    private enum Track {
        Left = -1,
        Middle,
        Right,
    };

    [SerializeField] private float runningSpeed = 1f;
    private Track currentTrack = Track.Middle;
    private float offsetPosX = 1f;
    private float changeTrackSpeed = 3f;

    private float maxRotation = 30f;
    private float rotation = 0f;
    private bool isRotationRising = false;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        body = GameObject.FindGameObjectWithTag("PlayerBody");
    }

    // Update is called once per frame
    void Update()
    {
        ProcessMoving();
    }

    private void ProcessMoving()
    {
        HandleInputs();

        if (isMovingLeft)
        {
            MoveLeft();
        }
        else if (isMovingRight)
        {
            MoveRight();
        }

        MoveForward();
    }

    private void HandleInputs()
    {
        print(isMovingLeft);
        if ((Input.GetKeyDown("a") || Input.GetKeyDown("left")) && !isMovingLeft)
        {
            isMovingLeft = true;

            if (isMovingRight && currentTrack == Track.Middle)
            {
                currentTrack = Track.Right;
            }

            isMovingRight = false;
            isRotationRising = true;
        }
        
        if ((Input.GetKeyDown("d") || Input.GetKeyDown("right")) && !isMovingRight)
        {
            isMovingRight = true;

            if (isMovingLeft && currentTrack == Track.Middle)
            {
                currentTrack = Track.Left;
            }

            isMovingLeft = false;
            isRotationRising = true;
        }
    }

    private void MoveForward()
    {
        controller.Move(Vector3.forward * runningSpeed * Time.deltaTime);

        if (!isMovingLeft && !isMovingRight)
        {
            body.transform.rotation = Quaternion.identity;

            rotation = 0f;
        }
    }

    private void MoveLeft()
    {
        float movingSpeed = changeTrackSpeed * Time.deltaTime;

        switch (currentTrack)
        {
            case Track.Left:
                MoveLeftFromMiddleTrack(movingSpeed);
                break;
            case Track.Middle:
                MoveLeftFromMiddleTrack(movingSpeed);
                break;
            case Track.Right:
                MoveLeftFromRightTrack(movingSpeed);
                break;

        }
    }
    
    private void MoveRight()
    {
        float movingSpeed = changeTrackSpeed * Time.deltaTime;

        switch (currentTrack)
        {
            case Track.Left:
                MoveRightFromLeftTrack(movingSpeed);
                break;
            case Track.Middle:
                MoveRightFromMiddleTrack(movingSpeed);
                break;
            case Track.Right:
                MoveRightFromMiddleTrack(movingSpeed);
                break;

        }
    }

    private void MoveLeftFromMiddleTrack(float movingSpeed)
    {
        controller.Move(Vector3.right * -movingSpeed);

        RotateLeft(movingSpeed);

        if (transform.position.x <= (float)Track.Left)
        {
            transform.position = new Vector3((float)Track.Left, transform.position.y, transform.position.z);

            currentTrack = Track.Left;

            isMovingLeft = false;
        }
    }

    private void MoveLeftFromRightTrack(float movingSpeed)
    {
        controller.Move(Vector3.right * -movingSpeed);

        RotateLeft(movingSpeed);

        if (transform.position.x <= (float)Track.Middle)
        {
            transform.position = new Vector3((float)Track.Middle, transform.position.y, transform.position.z);

            currentTrack = Track.Middle;

            isMovingLeft = false;
        }
    }

    private void MoveRightFromLeftTrack(float movingSpeed)
    {
        controller.Move(Vector3.right * movingSpeed);

        RotateRight(movingSpeed);

        if (transform.position.x >= (float)Track.Middle)
        {
            transform.position = new Vector3((float)Track.Middle, transform.position.y, transform.position.z);

            currentTrack = Track.Middle;

            isMovingRight = false;
        }
    }

    private void MoveRightFromMiddleTrack(float movingSpeed)
    {
        controller.Move(Vector3.right * movingSpeed);

        RotateRight(movingSpeed);

        if (transform.position.x >= (float)Track.Right)
        {
            transform.position = new Vector3((float)Track.Right, transform.position.y, transform.position.z);

            currentTrack = Track.Right;

            isMovingRight = false;
        }
    }

    private void RotateLeft(float movingSpeed)
    {
        if (isRotationRising)
        {
            rotation -= movingSpeed * 50;

            if (rotation <= -maxRotation) isRotationRising = false;
        }
        else
        {
            rotation += movingSpeed * 50;
        }

        body.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }
    
    private void RotateRight(float movingSpeed)
    {
        if (isRotationRising)
        {
            rotation += movingSpeed * 50;

            if (rotation >= maxRotation) isRotationRising = false;
        }
        else
        {
            rotation -= movingSpeed * 50;
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
