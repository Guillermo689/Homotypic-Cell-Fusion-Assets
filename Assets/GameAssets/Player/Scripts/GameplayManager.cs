using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    private PlayerMain player;
    private bool paused;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseResumeButton;
    [SerializeField] private GameObject gameOverResumeButton;
    private EventSystem eventSystem;
    private CellFusion cellFusion;
    private bool gameOverPlayed = false;
    public float GameplayTimer;
    [SerializeField] private TMP_Text gameplayText;
    public TMP_Text highScore;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMain>();
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
        cellFusion = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<CellFusion>();
        highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        gameplayText.text = "Score: " + Mathf.RoundToInt(GameplayTimer).ToString();
        
        if (player.oxygen <= 0)
        {
            if (!gameOverPlayed)
            {
                if(GameplayTimer > PlayerPrefs.GetInt("HighScore", 0))
                {
                    PlayerPrefs.SetInt("HighScore", Mathf.RoundToInt(GameplayTimer));
                }

               
                cellFusion.audioSource.PlayOneShot(cellFusion.gameOver);
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().Stop();
                gameOverPlayed = true;
                Invoke("GameOver", 2f);
            }
            
        }
        else
        {
            GameplayTimer += Time.deltaTime;
            PauseToggle();
        }
    }
    public void PauseToggle()
    {
        if (player.pause.WasPressedThisFrame())
        {
            if (paused)
            {

                ResumeGame();
            }
            else
            {
                PauseGame();
            }

        }
    }

    public void ResumeGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        paused = false;
        AudioListener.pause = false;
    }
    public void PauseGame()
    {
        eventSystem.SetSelectedGameObject(pauseResumeButton);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        paused = true;
        AudioListener.pause = true;
       
       
    }
    public void GameOver()
    {
        gameOverMenu.SetActive(true);
        
        eventSystem.SetSelectedGameObject(gameOverResumeButton);
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
