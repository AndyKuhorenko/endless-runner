using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameState;

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoresText;
    [SerializeField] TextMeshProUGUI scoresTextNumber;
    [SerializeField] UnityEngine.UI.Button pauseButton;

    GameState currentState = GameState.Active;

    // Start is called before the first frame update
    void Start()
    {
        if (currentState == GameState.Active)
        {
            pauseButton.onClick.AddListener(SetPauseGameState);
        }
    }

    public void SetScoresTextNumber(float score)
    {
        scoresTextNumber.text = $"{Math.Round(score)} m";
    }

    private void SetPauseGameState()
    {
        pauseButton.gameObject.SetActive(false);
        Time.timeScale = 0;
        currentState = GameState.Pause;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
