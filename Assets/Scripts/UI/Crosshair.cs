using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public void Move(Vector3 aimPos)
    {
        transform.position = new Vector3(transform.position.x + aimPos.x, transform.position.y + aimPos.y, 0);
    }
}
