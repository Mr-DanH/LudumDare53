using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelScreen : MonoBehaviour
{
    private enum ScreenMode
    {
        None = 0,
        Completed = 1,
        Upgrading = 2,
        LevelDetails = 3,
        Off = 4,
    }

    public System.Action OnClosed;

    [SerializeField] private GameObject background;
    [SerializeField] private GameObject levelComplete;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextMeshProUGUI messageLabel;
    [SerializeField] private GameObject upgradeArea;
    [SerializeField] private float delayDuration = 1;

    [SerializeField] private LevelData levelData;

    private ScreenMode status = ScreenMode.Off;
    private ScreenMode previousStatus = ScreenMode.Off;
    private float delay;

    void Awake()
    {
        DeactivateAll();
        background.SetActive(false);
    }

    public void Setup(int levelIndex)
    {
        SetupLevel(levelIndex);
        status = ScreenMode.Completed;
    }

    public void SetupStartLevel()
    {
        SetupLevel(0);
        status = ScreenMode.LevelDetails;
    }

    public void Tick()
    {
        switch (status)
        {
            case ScreenMode.Completed:
            {
                delay -= Time.deltaTime;
                if (previousStatus != status)
                {
                    DeactivateAll();
                    levelComplete.SetActive(true);
                    previousStatus = status;
                    delay = delayDuration;
                }
                if(delay <= 0)
                {
                    status = ScreenMode.Upgrading;
                }

                break;
            }
            case ScreenMode.Upgrading:
            {
                if (previousStatus != status)
                {
                    DeactivateAll();
                    upgradeArea.SetActive(true);
                    previousStatus = status;

                    // todo - upgrades!
                    status = ScreenMode.LevelDetails;
                }
                break;
            }
            case ScreenMode.LevelDetails:
            {
                delay -= Time.deltaTime;
                if (previousStatus != status)
                {
                    DeactivateAll();
                    levelLabel.gameObject.SetActive(true);
                    messageLabel.gameObject.SetActive(true);
                    previousStatus = status;
                    delay = delayDuration;
                }

                if(delay <= 0)
                {
                    status = ScreenMode.Off;
                }

                break;
            }
            case ScreenMode.Off:
            {
                if (previousStatus != status)
                {
                    DeactivateAll();
                    previousStatus = status;
                    background.SetActive(false);
                    InputManager.Instance.ChangeToPlayerInput();
                    OnClosed.Invoke();
                }
                break;
            }
        }
    }

    private void DeactivateAll()
    {
        levelComplete.SetActive(false);
        upgradeArea.SetActive(false);
        levelLabel.gameObject.SetActive(false);
        messageLabel.gameObject.SetActive(false);
    }

    private void SetupLevel(int levelIndex)
    {
        InputManager.Instance.ChangeToUIInput();

        DeactivateAll();

        var level = levelData.Levels[levelIndex];
        levelLabel.SetText($"{level.Title}");
        messageLabel.SetText($"{level.Message}");

        background.SetActive(true);
    }
}
