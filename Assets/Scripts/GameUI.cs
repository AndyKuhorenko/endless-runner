using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    void Awake()
    {
        int musicPlayersCount = FindObjectsOfType<GameUI>().Length;

        //if (musicPlayersCount > 1)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
        //    DontDestroyOnLoad(gameObject);
        //}
    }
}
