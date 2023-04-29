using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_scoreLabel;
    public TMPro.TextMeshProUGUI m_levelLabel;
    public TMPro.TextMeshProUGUI m_livesLabel;
    public TMPro.TextMeshProUGUI m_levelComplete;
    public TMPro.TextMeshProUGUI m_gameOver;
    public RectTransform m_progress;

    public Transform m_waveNode;

    const int NUM_TILES_PER_CITY = 5;
    const int NORMAL_SCROLL_SPEED = 2;
    const int POST_LEVEL_SCROLL_SPEED = 20;
    const int SPEED_DELTA = POST_LEVEL_SCROLL_SPEED - NORMAL_SCROLL_SPEED;
    const int TIME_BETWEEN_LEVELS = 5;

    int m_score;
    int m_level;

    HitBoxRender m_hitBoxRender;

    float m_idleTime;

    void Awake()
    {
        m_hitBoxRender = GetComponentInChildren<HitBoxRender>();
        m_hitBoxRender.OnPigeonArrived += () => m_score++;

        UpdateUI();

        m_levelComplete.gameObject.SetActive(false);
        m_gameOver.gameObject.SetActive(false);
        m_progress.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    void Start()
    {
        ScrollingLevel.Instance.ReparentWaves(m_waveNode);
        ScrollingLevel.Instance.m_speed = NORMAL_SCROLL_SPEED;
        ScrollingLevel.Instance.StartCity(NUM_TILES_PER_CITY);
    }

    void UpdateUI()
    {
        m_scoreLabel.text = $"Deliveries: {m_score}/{6}"; // todo - proper level settings
        m_levelLabel.text = $"Level: {m_level}";
        m_livesLabel.text = $"Lives: {Player.Instance.PlayerLives}";
    }

    void Update()
    {
        Player.Instance.Tick();

        if(ScrollingLevel.Instance.IsLevelComplete())
            ScrollingLevel.Instance.m_speed = Mathf.MoveTowards(ScrollingLevel.Instance.m_speed, POST_LEVEL_SCROLL_SPEED, Time.deltaTime * SPEED_DELTA);
        else if(!ScrollingLevel.Instance.ActiveTiles.TrueForAll(a => a.m_type == Tile.eType.Wasteland))
            ScrollingLevel.Instance.m_speed = Mathf.MoveTowards(ScrollingLevel.Instance.m_speed, NORMAL_SCROLL_SPEED, Time.deltaTime * SPEED_DELTA);

        ScrollingLevel.Instance.Tick();

        m_hitBoxRender.Tick();

        CollisionDetector.Instance.Tick();

        if (Player.Instance.PlayerLives <= 0)
        {
            Player.Instance.gameObject.SetActive(false);
            m_gameOver.gameObject.SetActive(true);
        }
        else if(ScrollingLevel.Instance.IsLevelComplete())
        {
            if(m_idleTime <= 0)
            {
                m_idleTime = TIME_BETWEEN_LEVELS;
                m_levelComplete.gameObject.SetActive(true);
            }
            else
            {
                m_idleTime -= Time.deltaTime;

                if(m_idleTime <= 0)
                {
                    ++m_level;
                    ScrollingLevel.Instance.StartCity(NUM_TILES_PER_CITY);
                    m_levelComplete.gameObject.SetActive(false);
                }
            }            
        }

        UpdateUI();
    }
}
