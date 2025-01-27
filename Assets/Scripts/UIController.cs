﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("Count down text")]
    public TextMeshProUGUI countdownText;

    [Header("Life images")]
    [SerializeField] private Image[] LifeUiElements = new Image[3];

    [Space]
    [Header("pause control")]
    [SerializeField] private Image pausePlayButt;
    [SerializeField] private Sprite[] StateSprite = new Sprite[2];
    public bool isPaused;

    [Space]
    [Header("Score control")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Space]
    public Image tutorialImage;    
    /// <summary>
    /// When the script instance is being loaded it checks if the instance is <see langword="null"/>
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// switches scene to the index corresponding to the build settings
    /// </summary>
    /// <param name="index">build settings corresponding to the index of the scene you wan't to switch to</param>
    public void LoadScene(int index) => SceneManager.LoadSceneAsync(index);

    /// <summary>
    /// Sets the UI lives according to the current amount of lives left
    /// </summary>
    /// <param name="amountOfLifes">current amount of lives</param>
    public void SetLifeUI(int amountOfLifes) => LifeUiElements[amountOfLifes].enabled = false;

    /// <summary>
    /// handels the starting and pauzing of the game
    /// </summary>
    public void Pause()
    {
        if (!isPaused)
        {
            pausePlayButt.sprite = StateSprite[0];
            Time.timeScale = 0;
            AudioListener.pause = true;
        }
        else if (isPaused)
        {
            pausePlayButt.sprite = StateSprite[1];
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
        isPaused = !isPaused;
    }

    /// <summary>
    /// Sets the text of the score to the current score
    /// </summary>
    /// <param name="score">score to add</param>
    public void setScoreText(int score) => scoreText.SetText("score:\n {0}", score);

    /// <summary>
    /// Used to exit the game
    /// </summary>
    public void ExitGame() => Application.Quit();

    /// <summary>
    /// Can be used to play spriteSheet animation
    /// </summary>
    /// <param name="textures">The textures of the sprite sheet</param>
    /// <param name="framePP">how many frames per second</param>
    public Sprite playAnimtion(Sprite[] textures, float framePP)
    {
        int index = (int)(Time.unscaledTime * framePP) % textures.Length;
        return textures[index];
    }
}