using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private bool isStarted = false; // ゲーム開始済みフラグ

    private float score;
    public bool IsStarted => isStarted;
    public bool isGameOver;
    
    private float startTiem;
    private float clearTime;

    private int enemyCount;

    [Header("最大ショット回数")]
    [SerializeField] private int maxShotCount = 5;
    private int currentShotCount;

    [Header("スコアとボーナス")]
    [SerializeField] private int enemyScore = 100;
    [SerializeField] private int remainShotBonus = 500;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            StartCoroutine(WaitForFadeThenInit());
        }

        enemyCount = FindObjectsOfType<Enemy>().Length;
    }

    private IEnumerator WaitForFadeThenInit()
    {
        // フェード完了を待つ
        if (FadeManager.instance != null)
        {
            while (!FadeManager.instance.IsFadeComplete) yield return null;
        }
        InitGame();
    }

    /// <summary>
    ///  ゲームの初期化
    /// </summary>
    private void InitGame()
    {
        // 現在のシーンをチェック
        if (SceneManager.GetActiveScene().name != "GameScene") return;

        isStarted = false;
        isGameOver= false;
        score = 0f;
        currentShotCount = 0;

        UIManager.Instance.ShowStartUI(true);

        // ゲーム停止中
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!isStarted && !isGameOver)
        {
#if UNITY_EDITOR
            if (Mouse.current.leftButton.wasPressedThisFrame)
#else
            if(Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
#endif
            {
                StartGame();
            }

            return;
        }
    }

    public int GetScore()
    {
        return Mathf.FloorToInt(score);
    }

    private void StartGame()
    {
        isStarted = true;

        UIManager.Instance.ShowStartUI(false);

        Time.timeScale = 1f;

        startTiem = Time.time;

        FindObjectOfType<MobileInputVisualizer>().EnableInput();
    }

    public void TryClear()
    {
        Debug.Log("クリア");

        int remainShot = maxShotCount - currentShotCount;

        score += remainShot * remainShotBonus;

        PlayerPrefs.SetInt("Score",GetScore());

        FadeManager.instance.FadeToScene("Result");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    // ==== シーンリセット ====
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            StartCoroutine(WaitForFadeThenInit());
        }
        else
        {
            Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void GameOver()
    {
        if(isGameOver)
        {
            return;
        }

        isGameOver = true;
        isStarted = false;

        Time.timeScale = 0f;
        UIManager.Instance.ShowGameOver(GetScore());
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void BackTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    public void OnEnemyDefeated()
    {
        enemyCount--;

        score += enemyScore;

        if(enemyCount <= 0)
        {
            TryClear();
        }
    }

    public void UseShot()
    {
        currentShotCount++;
    }

    public bool IsShotEmpty()
    {
        return currentShotCount >= maxShotCount;
    }

    public int GetRemainShot()
    {
        return maxShotCount - currentShotCount;
    }
}
