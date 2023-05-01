using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameScreen : Singleton<GameScreen>
{
    private enum GameScreenView
    {
        None = 0,
        MainMenu = 1,
        LevelMenu = 2,
        Game = 3,
        GameOver = 4
    }

    [System.Serializable]
    public class LevelData
    {
        public Color m_fog;
        public List<Material> m_materials;
    }

    [SerializeField] private GameObject uiContainer;
    public TMPro.TextMeshProUGUI m_scoreLabel;
    public TMPro.TextMeshProUGUI m_overtimeBonusLabel;
    public TMPro.TextMeshProUGUI m_levelLabel;
    public TMPro.TextMeshProUGUI m_livesLabel;
    public CanvasGroup m_controls;
    public GameObject m_bossUiRoot;
    public GameObject m_deliveryUiRoot;
    public RectTransform m_bossHealthSlider;

    [SerializeField] private List<Image> deliveriesImages;
    [SerializeField] private List<Image> livesImages;
    [SerializeField] private List<Image> availablePigeonImages;
    public RectTransform m_progress;

    public Transform m_waveNode;

    public Color m_dayTimeFog;
    public List<LevelData> m_levelData;
    public Color m_nightTimeFog;

    const int NUM_TILES_PER_CITY = 5;
    const float NORMAL_SCROLL_SPEED = 2;
    const float SPEED_INCREASE_PER_LEVEL = 0.4f;
    const float POST_LEVEL_SCROLL_SPEED = 20;
    const float SPEED_DELTA = POST_LEVEL_SCROLL_SPEED - NORMAL_SCROLL_SPEED;
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
        LevelDetails.Instance.Reset();
        Player.Instance.Init();

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
        Boss.Instance.gameObject.SetActive(false);
        m_controls.alpha = 0;
        ScrollingLevel.Instance.ReparentWaves(m_waveNode);
        UpdateUI();
        Player.Instance.gameObject.SetActive(false);
    }

    private void StartLevel()
    {
        bool isBossLevel = false;//(m_level == 0);

        if(isBossLevel)
            ScrollingLevel.Instance.StartCity(999, m_levelData[m_level]);
        else
            ScrollingLevel.Instance.StartCity(NUM_TILES_PER_CITY, m_levelData[m_level]);

        m_bossUiRoot.gameObject.SetActive(isBossLevel);
        m_deliveryUiRoot.gameObject.SetActive(!isBossLevel);

        Player.Instance.gameObject.SetActive(true);
        Player.Instance.LevelReset();
        
        currentView = GameScreenView.Game;
    }

    IEnumerator FadeDownControls()
    {
        m_controls.alpha = 1;

        yield return new WaitForSeconds(3);

        for(float t = 1; t > 0; t -= Time.deltaTime * 0.5f)
        {
            m_controls.alpha = t;
            yield return null;
        }

        m_controls.alpha = 0;
    }

    void UpdateUI()
    {
        uiContainer.SetActive(currentView == GameScreenView.Game);

        m_scoreLabel.text = $"{m_score}/6";
        m_overtimeBonusLabel.gameObject.SetActive(m_score > 6);

        for(int i = 0; i < deliveriesImages.Count; ++i)
        {
            float alpha = (i >= m_score) ? 0.8f : 0.2f;

            deliveriesImages[i].color = new Color(1, 1, 1, alpha);
        }

        m_livesLabel.text = $"x{Player.Instance.PlayerLives}";

        for(int i = 0; i < livesImages.Count; ++i)
        {
            float alpha = (i < Player.Instance.PlayerLives) ? 0.8f : 0.2f;

            livesImages[i].color = new Color(1, 1, 1, alpha);
        }

        List<Image> pigeonQueue = PigeonManager.Instance.GetPigeonQueue(out int availableCount);


        for(int i = 0; i < availablePigeonImages.Count; ++i)
        {
            Image targetImage = availablePigeonImages[i];

            if (i >= pigeonQueue.Count)
            {
                targetImage.color = new Color(1, 1, 1, 0);
            }
            else
            {

                Color color = pigeonQueue[i].color;
                color.a = (i < availableCount) ? 0.8f : 0.2f;

                targetImage.sprite = pigeonQueue[i].sprite;
                targetImage.color = color;
            }

        }        

        float maxSize = (m_bossHealthSlider.parent as RectTransform).sizeDelta.y;
        m_bossHealthSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize * Boss.Instance.GetNormalisedHealth());
    }

    void Update()
    {
        Player.Instance.Tick();

        if(InWasteland())
        {
            float speedDelta = POST_LEVEL_SCROLL_SPEED - NORMAL_SCROLL_SPEED;
            ScrollingLevel.Instance.m_speed = Mathf.MoveTowards(ScrollingLevel.Instance.m_speed, POST_LEVEL_SCROLL_SPEED, Time.deltaTime * speedDelta);
        }
        else if(!ScrollingLevel.Instance.ActiveTiles.TrueForAll(a => a.m_type == Tile.eType.Wasteland))
        {
            float targetSpeed = NORMAL_SCROLL_SPEED + (m_level * SPEED_INCREASE_PER_LEVEL);
            float speedDelta = POST_LEVEL_SCROLL_SPEED - targetSpeed;
            ScrollingLevel.Instance.m_speed = Mathf.MoveTowards(ScrollingLevel.Instance.m_speed, targetSpeed, Time.deltaTime * speedDelta);
        }

        if(m_level == -1)
        {
            RenderSettings.fogColor = m_nightTimeFog;
        }
        else
        {
            float fogLerp = Mathf.InverseLerp(NORMAL_SCROLL_SPEED, POST_LEVEL_SCROLL_SPEED, ScrollingLevel.Instance.m_speed);
            Color dayTimeFog = m_levelData[m_level].m_fog;
            RenderSettings.fogColor = Color.Lerp(dayTimeFog, m_nightTimeFog, fogLerp);
        }

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
            else if (HasCompletedAllLevels())
            {
                OpenGameCompleted();
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
        menuScreen.SetupGameOver(message);
        Restart();
    }

    private void OpenGameCompleted()
    {
        menuScreen.SetupGameComplete();
        Restart();
    }

    private void Restart()
    {
        Player.Instance.gameObject.SetActive(false);
        Player.Instance.Reset();
        
        ScrollingLevel.Instance.Reset();
        LevelDetails.Instance.Reset();

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

        if(m_level == 0)
        {
            Boss.Instance.Init();
            Boss.Instance.gameObject.SetActive(false);
            PigeonManager.Instance.NewGame();
            StartCoroutine(FadeDownControls());
        }
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
        if(Boss.Instance.IsDead)
            return true;

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

    private bool HasCompletedAllLevels()
    {
        bool hasAnotherLevel = levelScreen.HasAnotherLevel(m_level);

        return !hasAnotherLevel && HasPassedCompletedLevel();
    }
}
