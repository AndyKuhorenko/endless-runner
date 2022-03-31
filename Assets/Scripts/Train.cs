using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Train : MonoBehaviour
{
    CinemachineDollyCart[] railCarriages;

    // Start is called before the first frame update
    void Start()
    {
        float trainSpeed = Random.Range(6, 10);
        railCarriages = GetComponentsInChildren<CinemachineDollyCart>();

        foreach (CinemachineDollyCart railCrriage in railCarriages)
        {
            railCrriage.m_Speed = trainSpeed;
        }
    }
}
