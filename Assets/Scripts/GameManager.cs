using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { GAME, PAUSE_MENU, LEVEL_COMPLETED, OPTIONS}

    private const string keyHighScore = "HighScoreLevel1";

    public GameState currentGameState;

    public Canvas inGameCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas levelCompletedCanvas;
    public Canvas optionsCanvas;

    public static GameManager instance;
    private Scene currentScene;

    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text defeatedEnemiesText;

    public TMP_Text scoreLevelCompletedText;
    public TMP_Text highScoreText;

    public TMP_Text QualityText;
    public TMP_Text VolumeText;

    private int score = 0;

    public Image[] keysTab;

    private Color originalKeyColour;

    private int keysFound = 0;

    public Transform playerLives;
    public GameObject lifeIcon;

    private float time = 0;

    private int defeatedEnemies = 0;

    private List<GameObject> playerLivesList = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Duplicated Game Manager", gameObject);
            Destroy(gameObject);
            return;
        }

        if (!PlayerPrefs.HasKey(keyHighScore))
        {
            PlayerPrefs.SetInt(keyHighScore, 0);
        }
        scoreText.text = "000";
        defeatedEnemiesText.text = "000";
        timeText.text = string.Format("{0:00}:{1:00}", 0, 0);
        originalKeyColour = keysTab[0].color;

        for (int i = 0; i < keysTab.Length; i++)
        {
            keysTab[i].color = Color.gray;
        }

        for (int i = 0; i < 3; i++)
        {
            addLife();
        }

        pauseMenuCanvas.enabled = false;
        levelCompletedCanvas.enabled = false;
        optionsCanvas.enabled = false;

        currentScene = SceneManager.GetActiveScene();

    }

    public void Resume_ButtonClicked()
    {
        InGame();
    }

    public void Reset_ButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        InGame();
    }

    public void Options_ButtonClicked()
    {
        Options();
    }

    public void MainMenu_ButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void setGameState(GameState newGameState)
    {
        currentGameState = newGameState;
    }

    public void PauseMenu()
    {
        setGameState(GameState.PAUSE_MENU);
        inGameCanvas.enabled = false;
        optionsCanvas.enabled = false;
        pauseMenuCanvas.enabled = true;
        Time.timeScale = 0;
    }

    void Options()
    {
        setGameState(GameState.OPTIONS);
        inGameCanvas.enabled = false;
        optionsCanvas.enabled = true;
        pauseMenuCanvas.enabled = false;
        RefreshQualityName();
    }

    void RefreshQualityName()
    {
        QualityText.text = "Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    public void InGame()
    {
        setGameState(GameState.GAME);
        inGameCanvas.enabled = true;
        optionsCanvas.enabled = false;
        pauseMenuCanvas.enabled = false;
        Time.timeScale = 1.0f;
    }

    public void DecreaseQuality()
    {
        QualitySettings.DecreaseLevel();
        RefreshQualityName();
    }
    public void IncreaseQuality()
    {
        QualitySettings.IncreaseLevel();
        RefreshQualityName();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void LevelCompleted()
    {
        setGameState(GameState.LEVEL_COMPLETED);
        if (currentScene.name == "level1_22")
        {
            if (score > PlayerPrefs.GetInt(keyHighScore))
            {
                PlayerPrefs.SetInt(keyHighScore, score);
            }
        }

        scoreLevelCompletedText.text = "Your score: " + score.ToString("D4");
        highScoreText.text = "High score: " + PlayerPrefs.GetInt(keyHighScore).ToString("D4");
        levelCompletedCanvas.enabled = true;
    }

    public void addPoints(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
        scoreText.text = score.ToString("D3");
    }

    public void addEnemyKilled()
    {
        defeatedEnemies+=1;
        defeatedEnemiesText.text = defeatedEnemies.ToString("D3");
    }

    public void addKey()
    {
        keysFound++;
        for (int i = 0; i < keysFound; i++)
        {
            keysTab[i].color = originalKeyColour;
        }
    }


    public void addLife()
    {
        GameObject newLife = Instantiate(lifeIcon, playerLives);

        float xOffset = playerLivesList.Count * 110.0f;
        newLife.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, 0);

        playerLivesList.Add(newLife);
    }

    public void takeLife()
    {
        if (playerLivesList.Count > 0)
        {
            GameObject lastLife = playerLivesList[playerLivesList.Count - 1];
            playerLivesList.RemoveAt(playerLivesList.Count - 1);
            Destroy(lastLife);
        }
        else
        {
            Debug.LogWarning("Nie ma ¿yæ");
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.GAME)
            {
                PauseMenu();
            }
            else if (currentGameState == GameState.PAUSE_MENU)
            {
                InGame();
            }
        }

        time += Time.deltaTime;

        //time += Time.deltaTime;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
