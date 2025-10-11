using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MusicManager))]
[RequireComponent(typeof(LevelManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region Configuration

    [Header("Canvase References")]
    [SerializeField] GameObject playCanvas;
    
    #endregion

    #region Cached References
    #endregion

    #region Runtime Variables
    //Level settings

    bool gameOver = false;
    bool gameWon = false;
    int score = 0;
    #endregion

    #region Properties
    public bool GameIsOver { get => gameOver; }
    public bool GameWon { get => gameWon; }
    public static int Score => Instance.score;
    #endregion

    private void Awake()
    {
        // Setup singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this instance across scenes

        // Subscrite to sceneLoaded once.
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    void Update()
    {
        if(gameOver || gameWon) return;

        
    }
    private void WinGame()
    {
        gameWon = true;
        playCanvas.SetActive(false);

        LevelManager.LoadWinScreen();
    }
    void GameOver()
    {
        playCanvas.SetActive(false);
        LevelManager.LoadGameOver();
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RunPerSceneSetup();
    }
    private void RunPerSceneSetup()
    {

    }

    public void CheckWinGame()
    {
        if (Enemy.EnemyCount > 0) return;
        if (!EnemySpawner.Instance.doneSpawning) return;

        gameWon = true;
        LevelManager.LoadWinScreen();

    }

    public void LoseGame()
    {
        gameOver = true;
        LevelManager.LoadGameOver();
    }
    private void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks if destroyed manually
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
