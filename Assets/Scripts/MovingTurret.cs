using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTurret : MonoBehaviour
{
    private int rotationSpeed;
    private int maxRotaionAngle = 170;
    private bool isRotated = false;

    void Start()
    {
        rotationSpeed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localEulerAngles.y >= maxRotaionAngle)
        {
            isRotated = true;
        }
        else if (transform.localEulerAngles.y <= 10)
        {
            isRotated = false;
        }

        transform.Rotate(transform.rotation.x, isRotated ? -rotationSpeed / 5 : rotationSpeed / 5, 0);
    }
}
