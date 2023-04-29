using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_scoreLabel;
    public TMPro.TextMeshProUGUI m_levelLabel;
    public TMPro.TextMeshProUGUI m_livesLabel;

    int m_score;
    int m_level;
    int m_lives = 3;

    HitBoxRender m_hitBoxRender;

    void Awake()
    {
        TryGetComponent(out m_hitBoxRender);

        UpdateUI();
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
                }
            }
        }

        //collision detection

        UpdateUI();
    }
}
