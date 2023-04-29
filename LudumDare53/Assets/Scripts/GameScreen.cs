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

        //collision detection

        UpdateUI();
    }
}
