using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameState;

public class UI : MonoBehaviour
{
    [SerializeField] AudioManager audioManager;

    [SerializeField] TextMeshProUGUI scoresText;
    [SerializeField] TextMeshProUGUI scoresTextNumber;

    [SerializeField] TextMeshProUGUI ammoText;

    [SerializeField] UnityEngine.UI.Button pauseButton;
    [SerializeField] UnityEngine.UI.Button pauseShade;

    [SerializeField] UnityEngine.UI.Button resumeButton;

    [SerializeField] TextMeshProUGUI failScoreText;
    [SerializeField] TextMeshProUGUI failText;
    [SerializeField] UnityEngine.UI.Button restartButton;

    [SerializeField] UnityEngine.UI.Button startButton;
    [SerializeField] UnityEngine.UI.Image startPanel;
    [SerializeField] TextMeshProUGUI bestResultScore;

    [SerializeField] public static GameState currentState = GameState.Start;

    private float currentScore;

    private float storageScore;

    void Start()
    {
        storageScore = PlayerPrefs.GetFloat("score");

        startButton.onClick.AddListener(SetActiveGameState);

        pauseButton.onClick.AddListener(SetPauseGameState);
        pauseShade.onClick.AddListener(OnPauseShadeCLick);

        resumeButton.onClick.AddListener(SetActiveGameState);

        restartButton.onClick.AddListener(RestartGame);

        switch (currentState)
        {
            case GameState.Start:
                SetStartGameState();
                break;
            case GameState.Pause:
                SetPauseGameState();
                break;
            case GameState.Active:
                SetActiveGameState();
                break;
            case GameState.Fail:
                SetFailGameState();
                break;
            default:
                return;
        }
    }

    public void SetScoresTextNumber(float score)
    {
        currentScore = score;

        scoresTextNumber.text = $"{Math.Round(score)} m";
    }

    private void SetStartGameState()
    {
        if (storageScore > 0)
        {
            bestResultScore.text = $"{Math.Round(storageScore)} m";
        }

        audioManager.PlayMenu();

        startPanel.gameObject.SetActive(true);

        pauseButton.gameObject.SetActive(false);
        pauseShade.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        failScoreText.gameObject.SetActive(false);
        failText.gameObject.SetActive(false);

        currentState = GameState.Start;
        Time.timeScale = 0;
    }

    private void SetPauseGameState()
    {
        audioManager.PlayMenu();

        pauseButton.gameObject.SetActive(false);

        pauseShade.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(true);

        Time.timeScale = 0;
        currentState = GameState.Pause;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    private void SetActiveGameState()
    {
        audioManager.PlayGame();

        scoresText.gameObject.SetActive(true);
        scoresTextNumber.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);

        startPanel.gameObject.SetActive(false);
        pauseShade.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        failScoreText.gameObject.SetActive(false);
        failText.gameObject.SetActive(false);

        Time.timeScale = 1;
        currentState = GameState.Active;
    }

    public void SetFailGameState()
    {
        if (currentScore > storageScore) PlayerPrefs.SetFloat("score", currentScore);

        scoresText.gameObject.SetActive(false);
        scoresTextNumber.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);

        failScoreText.text = scoresTextNumber.text;
        failScoreText.gameObject.SetActive(true);
        failText.gameObject.SetActive(true);

        pauseShade.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);

        Time.timeScale = 0;
        currentState = GameState.Fail;
    }

    public void SetAmmoCount(int newAmmo)
    {
        ammoText.text = $"Ammo: {newAmmo}";
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);

        SetActiveGameState();
    }

    private void OnPauseShadeCLick()
    {
        if (currentState == GameState.Pause)
        {
            SetActiveGameState();
        }
        else if (currentState == GameState.Fail)
        {
            RestartGame();
        }
    }

    void Update()
    {

    }
}
