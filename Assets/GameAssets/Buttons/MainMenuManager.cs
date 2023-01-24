using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    private EventSystem eventSystem;
   
    [SerializeField] private GameObject startButton;
    public TMP_Text highScore;

    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1;
        eventSystem = GetComponent<EventSystem>();
        eventSystem.SetSelectedGameObject(startButton);
        highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
    }
    private void Update()
    {
       if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            eventSystem.SetSelectedGameObject(startButton);
        }
    }
}
