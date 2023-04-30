using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_scoreLabel;
    public TMPro.TextMeshProUGUI m_levelLabel;
    public TMPro.TextMeshProUGUI m_livesLabel;
    public TMPro.TextMeshProUGUI m_gameOver;
    public RectTransform m_progress;

    public Transform m_waveNode;

    public Color m_dayTimeFog;
    public Color m_nightTimeFog;

    const int NUM_TILES_PER_CITY = 5;
    const int NORMAL_SCROLL_SPEED = 2;
    const int POST_LEVEL_SCROLL_SPEED = 20;
    const int SPEED_DELTA = POST_LEVEL_SCROLL_SPEED - NORMAL_SCROLL_SPEED;
    const int TIME_BETWEEN_LEVELS = 5;

    int m_score;
    int m_level = -1;

    HitBoxRender m_hitBoxRender;
    LevelScreen levelScreen;

    private bool isBetweenLevels = false;

    void Awake()
    {
        m_hitBoxRender = GetComponentInChildren<HitBoxRender>();
        m_hitBoxRender.OnPigeonArrived += () => m_score++;

        levelScreen = GetComponentInChildren<LevelScreen>();
        levelScreen.OnClosed += LevelUiClosed;

        levelScreen.SetupStartLevel();
        isBetweenLevels = true;

        UpdateUI();

        m_gameOver.gameObject.SetActive(false);
        m_progress.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    void Start()
    {
        ScrollingLevel.Instance.ReparentWaves(m_waveNode);
    }

    void UpdateUI()
    {
        m_scoreLabel.gameObject.SetActive(!isBetweenLevels);
        m_levelLabel.gameObject.SetActive(!isBetweenLevels);
        m_livesLabel.gameObject.SetActive(!isBetweenLevels);

        m_scoreLabel.text = $"Deliveries: {m_score}/{6}";
        m_levelLabel.text = $"Level: {m_level+1}";
        m_livesLabel.text = $"Lives: {Player.Instance.PlayerLives}";
    }

    void Update()
    {
        Player.Instance.Tick();

        if(InWasteland())
            ScrollingLevel.Instance.m_speed = Mathf.MoveTowards(ScrollingLevel.Instance.m_speed, POST_LEVEL_SCROLL_SPEED, Time.deltaTime * SPEED_DELTA);
        else if(!ScrollingLevel.Instance.ActiveTiles.TrueForAll(a => a.m_type == Tile.eType.Wasteland))
            ScrollingLevel.Instance.m_speed = Mathf.MoveTowards(ScrollingLevel.Instance.m_speed, NORMAL_SCROLL_SPEED, Time.deltaTime * SPEED_DELTA);

        float fogLerp = Mathf.InverseLerp(NORMAL_SCROLL_SPEED, POST_LEVEL_SCROLL_SPEED, ScrollingLevel.Instance.m_speed);
        RenderSettings.fogColor = Color.Lerp(m_dayTimeFog, m_nightTimeFog, fogLerp);

        ScrollingLevel.Instance.Tick();

        m_hitBoxRender.Tick();

        CollisionDetector.Instance.Tick();

        if (Player.Instance.PlayerLives <= 0)
        {
            Player.Instance.gameObject.SetActive(false);
            m_gameOver.gameObject.SetActive(true);
        }
        else if(InWasteland())
        {
            if (!isBetweenLevels)
            {
                levelScreen.Setup(m_level+1);
                m_score = 0;
                isBetweenLevels = true;
            }

        }
        else
        {
            float maxSize = (m_progress.parent as RectTransform).sizeDelta.y;
            m_progress.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize * ScrollingLevel.Instance.GetProgressThroughLevel());
        }

        levelScreen.Tick();
        UpdateUI();
    }

    private void LevelUiClosed()
    {
        ScrollingLevel.Instance.StartCity(NUM_TILES_PER_CITY);
        ++m_level;
        isBetweenLevels = false;
    }

    private bool InWasteland()
    {
        return ScrollingLevel.Instance.IsLevelComplete() || isBetweenLevels;
    }
}
