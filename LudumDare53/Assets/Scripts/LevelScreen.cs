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

    public List<string> m_pigeonFacts = new List<string>();

    public Animator m_pigeonFactAnimator;
    public TextMeshProUGUI m_pigeonFactLabel;

    private ScreenMode status = ScreenMode.Off;
    private ScreenMode previousStatus = ScreenMode.Off;
    private float delay;

    List<string> m_pigeonFactPool = new List<string>();
    string m_pigeonFact;

    void Awake()
    {
        DeactivateAll();
        background.SetActive(false);
    }

    public void Setup(int levelIndex)
    {
        SetupLevel(levelIndex);
        int factIndex = Random.Range(0, m_pigeonFactPool.Count);
        m_pigeonFact = $"Pigeon Fact: {m_pigeonFactPool[factIndex]}";
        m_pigeonFactPool.RemoveAt(factIndex);
        status = ScreenMode.Completed;
    }

    IEnumerator RevealPigeonFact(string text)
    {
        m_pigeonFactLabel.SetText("");

        yield return new WaitForSeconds(0.5f);

        float letters = 0;

        while(letters < text.Length)
        {
            yield return null;

            letters += 30 * Time.deltaTime;

            int ceilLetterCount = Mathf.Min(text.Length, Mathf.CeilToInt(letters));
            string richText = $"{text.Substring(0, ceilLetterCount)}<color=#FFFFFF00>{text.Substring(ceilLetterCount, text.Length - ceilLetterCount)}</color>"; 
            m_pigeonFactLabel.SetText(richText);
        }

        m_pigeonFactLabel.SetText(text);
    }

    public void SetupStartLevel()
    {
        m_pigeonFactPool.Clear();
        m_pigeonFactPool.AddRange(m_pigeonFacts);

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
                    m_pigeonFactAnimator.SetBool("in", true);
                    StartCoroutine(RevealPigeonFact(m_pigeonFact));
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
                    m_pigeonFactAnimator.SetBool("in", false);
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
