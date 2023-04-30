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

    [SerializeField] private List<Image> deliveriesImages;
    [SerializeField] private List<Image> livesImages;
    [SerializeField] private List<Image> availablePigeonImages;
    public RectTransform m_progress;

    public Transform m_waveNode;

    public Color m_dayTimeFog;
    public Color m_nightTimeFog;

    const int NUM_TILES_PER_CITY = 5;
    const int NORMAL_SCROLL_SPEED = 2;
    const int POST_LEVEL_SCROLL_SPEED = 20;
    const int SPEED_DELTA = POST_LEVEL_SCROLL_SPEED - NORMAL_SCROLL_SPEED;
    const string DIED_MESSAGE = "You had too many accidents and your license was revoked.";
    const string FAILED_MISSION_MESSAGE = "You didn't deliver enough post!";

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

        m_progress.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }

    void Start()
    {
        ScrollingLevel.Instance.ReparentWaves(m_waveNode);
        UpdateUI();
        Player.Instance.gameObject.SetActive(false);
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

        for(int i = 0; i < deliveriesImages.Count; ++i)
        {
            float alpha = (i >= m_score) ? 0.8f : 0.2f;

            deliveriesImages[i].color = new Color(1, 1, 1, alpha);
        }

        for(int i = 0; i < livesImages.Count; ++i)
        {
            float alpha = (i < Player.Instance.PlayerLives) ? 0.8f : 0.2f;

            livesImages[i].color = new Color(1, 1, 1, alpha);
        }


        int availablePigeons = PigeonManager.Instance.GetAvailablePigeonCount();
        int maxPigeons = PigeonManager.Instance.m_maxPigeons;
        for(int i = 0; i < availablePigeonImages.Count; ++i)
        {
            float alpha = 0;
            if(i < maxPigeons)
                alpha = (i < availablePigeons) ? 0.8f : 0.2f;

            availablePigeonImages[i].color = new Color(1, 1, 1, alpha);
        }
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

        if (currentView == GameScreenView.Game)
        {
            if(Player.Instance.PlayerLives <= 0)
            {
                OpenGameOver(DIED_MESSAGE);
            }
            else if(HasFailedCompletedLevel())
            {
                OpenGameOver(FAILED_MISSION_MESSAGE);
            }
            else if(HasPassedCompletedLevel())
            {
                OpenLevelScreen();
            }
            else
            {
                float maxSize = (m_progress.parent as RectTransform).sizeDelta.y;
                m_progress.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize * ScrollingLevel.Instance.GetProgressThroughLevel());
            }
        }

        Player.Instance.gameObject.SetActive(currentView == GameScreenView.Game || currentView == GameScreenView.LevelMenu);

        levelScreen.Tick();
        menuScreen.Tick();
        UpdateUI();
    }

    private void OpenGameOver(string message)
    {      
        Player.Instance.gameObject.SetActive(false);
        Player.Instance.Reset();
        
        ScrollingLevel.Instance.Reset();

        menuScreen.SetupGameOver(message);
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

    private bool HasPassedCompletedLevel()
    {
        return ScrollingLevel.Instance.IsLevelComplete() && HasCompletedAllDeliveries();
    }

    private bool HasFailedCompletedLevel()
    {
        return ScrollingLevel.Instance.IsLevelComplete() && !HasCompletedAllDeliveries();
    }

    private bool HasCompletedAllDeliveries()
    {
        return m_score >= deliveriesImages.Count;
    }
}
