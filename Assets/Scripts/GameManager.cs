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

    [SerializeField] private int maxShotCount = 5;
    private int currentShotCount;

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

        score += Time.deltaTime * 10f;
    }

    public int GetScore()
    {
        return Mathf.FloorToInt(score);
    }

    private void StartGame()
    {
        isStarted = true;
        Time.timeScale = 1f;

        startTiem = Time.time;
    }

    public void TryClear()
    {
        Debug.Log("クリア");
        clearTime = Time.time - startTiem;
        PlayerPrefs.SetFloat("ClearTime", clearTime);
        FadeManager.instance.FadeToScene("ResultScene");
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
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void BackTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    public void OnEnemyDefeated()
    {
        enemyCount--;

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
}
