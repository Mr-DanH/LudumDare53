using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public System.Action OnClosed;

    [Header("MainMenu")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject startLabel;
    [SerializeField] private Button startButton;

    [Header("GameOver")]
    [SerializeField] private GameObject gameOver;
    [SerializeField] private Button restartButton;

    [SerializeField] private float introDuration = 0.5f;

    private float introTimer;
    private bool runningIntro;

    public void SetupMainMenu()
    {
        InputManager.Instance.ChangeToUIInput();
        mainMenu.SetActive(true);
        gameOver.SetActive(false);
        startLabel.SetActive(false);
        introTimer = introDuration;
        runningIntro = true;
    }

    public void SetupGameOver()
    {
        InputManager.Instance.ChangeToUIInput();
        mainMenu.SetActive(false);
        gameOver.SetActive(true);
        restartButton.Select();
    }

    public void Tick()
    {
        if (runningIntro)
        {
            introTimer -= Time.deltaTime;
            if(introTimer <= 0)
            {
                IntroComplete();
            }
        }
    }

    public void OnStart()
    {
        mainMenu.SetActive(false);
        gameOver.SetActive(false);
        OnClosed.Invoke();
    }

    private void IntroComplete()
    {
        startButton.Select();
        startLabel.SetActive(true);
        runningIntro = false;
    }
}
