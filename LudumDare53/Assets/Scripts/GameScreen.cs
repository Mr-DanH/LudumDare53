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
    int m_lives = 3;

    HitBoxRender m_hitBoxRender;

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
    }

    void UpdateUI()
    {
        m_scoreLabel.text = $"Score: {m_score}";
        m_levelLabel.text = $"Level: {m_level}";
        m_livesLabel.text = $"Lives: {m_lives}";
    }

    void Update()
    {
        Player.Instance.Tick();

        ScrollingLevel.Instance.Tick();

        m_hitBoxRender.Tick();

        RectTransform playerRectTransform = Player.Instance.transform as RectTransform;
        Rect playerRect = playerRectTransform.rect;
        playerRect.position += (Vector2)playerRectTransform.position;

        foreach(var tile in ScrollingLevel.Instance.ActiveTiles)
        {
            foreach(RectTransform enemy in tile.m_waveOffset)
            {
                if(!enemy.gameObject.activeInHierarchy)
                    continue;

                if(m_lives == 0)
                    break;
                    
                Rect enemyRect = enemy.rect;
                enemyRect.position += (Vector2)enemy.position;

                if(playerRect.Overlaps(enemyRect))
                {
                    enemy.gameObject.SetActive(false);
                    --m_lives;

                    if(m_lives == 0)
                    {
                        Player.Instance.gameObject.SetActive(false);
                        m_gameOver.gameObject.SetActive(true);
                    }
                }
            }
        }

        //collision detection

        UpdateUI();
    }
}
