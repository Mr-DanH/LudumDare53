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

    int m_score;
    int m_level;

    HitBoxRender m_hitBoxRender;

    float m_idleTime;

    void Awake()
    {
        m_hitBoxRender = GetComponentInChildren<HitBoxRender>();

        UpdateUI();

        m_levelComplete.gameObject.SetActive(false);
        m_gameOver.gameObject.SetActive(false);
        m_progress.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    void Start()
    {
        ScrollingLevel.Instance.ReparentWaves(m_waveNode);

        ScrollingLevel.Instance.StartCity(4);
    }

    void UpdateUI()
    {
        m_scoreLabel.text = $"Score: {m_score}";
        m_levelLabel.text = $"Level: {m_level}";
        m_livesLabel.text = $"Lives: {Player.Instance.PlayerLives}";
    }

    void Update()
    {
        Player.Instance.Tick();

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
                m_idleTime = 3;
                ScrollingLevel.Instance.m_speed = 10;
                m_levelComplete.gameObject.SetActive(true);
            }
            else
            {
                m_idleTime -= Time.deltaTime;

                if(m_idleTime <= 0)
                {
                    ++m_level;
                    ScrollingLevel.Instance.m_speed = 1;
                    ScrollingLevel.Instance.StartCity(10);
                    m_levelComplete.gameObject.SetActive(false);
                }
            }            
        }

        UpdateUI();
    }
}
