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
    private ScreenMode previousStatus = ScreenMode.None;
    private float delay;

    List<string> m_pigeonFactPool = new List<string>();
    string m_pigeonFact;

    public void Setup(int levelIndex)
    {
        DeactivateAll();

        var level = levelData.Levels[levelIndex];
        levelLabel.SetText($"{level.Title}");
        messageLabel.SetText($"{level.Message}");

        int factIndex = Random.Range(0, m_pigeonFactPool.Count);
        m_pigeonFact = $"Pigeon Fact: {m_pigeonFactPool[factIndex]}";
        m_pigeonFactPool.RemoveAt(factIndex);

        background.SetActive(true);

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
            m_pigeonFactLabel.SetText(text.Substring(0, Mathf.Min(text.Length, Mathf.CeilToInt(letters))));
        }

        m_pigeonFactLabel.SetText(text);
    }

    public void SetupStartLevel()
    {
        DeactivateAll();

        var level = levelData.Levels[0];
        levelLabel.SetText($"{level.Title}");
        messageLabel.SetText($"{level.Message}");
        m_pigeonFactAnimator.SetBool("in", false);

        m_pigeonFactPool.Clear();
        m_pigeonFactPool.AddRange(m_pigeonFacts);

        background.SetActive(true);

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
                    OnClosed.Invoke();
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
}
