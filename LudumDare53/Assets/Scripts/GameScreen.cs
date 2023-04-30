using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameScreen : MonoBehaviour
{
    private enum GameScreenView
    {
        None = 0,
        MainMenu = 1,
        LevelMenu = 2,
        Game = 3,
        GameOver = 4
    }

    [SerializeField] private GameObject uiContainer;
    public TMPro.TextMeshProUGUI m_scoreLabel;
    public TMPro.TextMeshProUGUI m_levelLabel;
    public TMPro.TextMeshProUGUI m_livesLabel;

    public RectTransform m_progress;

    public Transform m_waveNode;

    public Color m_dayTimeFog;
    public Color m_nightTimeFog;

    const int NUM_TILES_PER_CITY = 5;
    const int NORMAL_SCROLL_SPEED = 2;
    const int POST_LEVEL_SCROLL_SPEED = 20;
    const int SPEED_DELTA = POST_LEVEL_SCROLL_SPEED - NORMAL_SCROLL_SPEED;
    const int TIME_BETWEEN_LEVELS = 5;

    private GameScreenView currentView = GameScreenView.MainMenu;

    int m_score;
    int m_level = -1;

    HitBoxRender m_hitBoxRender;
    LevelScreen levelScreen;
    MenuScreen menuScreen;

    void Awake()
    {
        m_hitBoxRender = GetComponentInChildren<HitBoxRender>();
        m_hitBoxRender.OnPigeonArrived += () => m_score++;

        levelScreen = GetComponentInChildren<LevelScreen>(includeInactive:true);
        levelScreen.gameObject.SetActive(true);
        levelScreen.OnClosed += LevelUiClosed;

        menuScreen = GetComponentInChildren<MenuScreen>(includeInactive:true);
        menuScreen.OnClosed += MenuClosed;
        menuScreen.SetupMainMenu();

        Player.Instance.gameObject.SetActive(false);

        UpdateUI();

        m_progress.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    void Start()
    {
        ScrollingLevel.Instance.ReparentWaves(m_waveNode);
    }

    private void StartLevel()
    {
        ScrollingLevel.Instance.StartCity(NUM_TILES_PER_CITY);
        Player.Instance.gameObject.SetActive(true);
        
        currentView = GameScreenView.Game;
    }

    void UpdateUI()
    {
        uiContainer.SetActive(currentView == GameScreenView.Game);

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

        if (Player.Instance.PlayerLives <= 0 && currentView == GameScreenView.Game)
        {
            OpenGameOver();
        }
        else if(currentView == GameScreenView.Game && ScrollingLevel.Instance.IsLevelComplete())
        {
            OpenLevelScreen();
        }
        else if(currentView == GameScreenView.Game)
        {
            float maxSize = (m_progress.parent as RectTransform).sizeDelta.y;
            m_progress.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize * ScrollingLevel.Instance.GetProgressThroughLevel());
        }

        Player.Instance.gameObject.SetActive(currentView == GameScreenView.Game || currentView == GameScreenView.LevelMenu);

        levelScreen.Tick();
        menuScreen.Tick();
        UpdateUI();
    }

    private void OpenGameOver()
    {      
        Player.Instance.gameObject.SetActive(false);
        Player.Instance.Reset();
        
        ScrollingLevel.Instance.Reset();
        menuScreen.SetupGameOver();
        m_level = -1;
        m_score = 0;

        currentView = GameScreenView.GameOver;
    }

    private void OpenLevelScreen()
    {
        levelScreen.Setup(m_level+1);
        m_score = 0;

        currentView = GameScreenView.LevelMenu;
    }

    private void LevelUiClosed()
    {
        ++m_level;

        StartLevel();
    }

    private void MenuClosed()
    {
        if(currentView == GameScreenView.MainMenu)
        {
            levelScreen.SetupStartLevel();
            currentView = GameScreenView.LevelMenu;
        }
        else if (currentView == GameScreenView.GameOver)
        {
            menuScreen.SetupMainMenu();
            currentView = GameScreenView.MainMenu;
        }
    }

    private bool InWasteland()
    {
        return currentView != GameScreenView.Game;
    }
}
