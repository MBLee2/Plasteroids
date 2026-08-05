using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Larry;
    public string firstLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Larry = this;
        //StartGame();
    }

    void Start()
    {
        GameManager.Gary.StartGame();
    }

    public void ChangeToSpecificScene(string SceneName)
    {
         SceneManager.LoadScene(SceneName);
    }

    public void ChangeToNextScene()
    {
        int curreSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = curreSceneIndex += 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
    

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevel);
        Debug.Log("Start");
    }

    public void ReturnToMenu()
    {
        SoundManager.Sam.StopAllSounds();
        SceneManager.LoadScene("StartMenu");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
