using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public string firstLevel;

    void Start()
    {
        if (GameManager.Gary)
        {
            Destroy(GameManager.Gary.gameObject);
        }

        SoundManager.Sam.StartMenu();
    }

    public void button_StartGame()
    {
        SoundManager.Sam.PlayTransition();
        Invoke("GameStart", 0.911f);
    }

    private void GameStart()
    {
        SceneManager.LoadScene(firstLevel);
    }

    public void button_Quit()
    {
        //quit
        Application.Quit();
    }

    public void button_BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

