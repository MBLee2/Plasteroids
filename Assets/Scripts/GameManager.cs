using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum GameState{None, PreRound, Playing, PostRound}

public class GameManager : MonoBehaviour
{
    public static GameManager Gary;

    [Header("UI")]
    public int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public GameObject[] level1Lives;
    public GameObject[] level2Lives;
    public GameObject[] level3Lives;
    public GameObject[] bossLevelLives;
    public GameObject[][] lifeSprites;

    [Header("SceneInfo")]
    public float sceneWidth = 17.8f;
    public float sceneHeight = 10f;
    public Animator sceneAnimator;
    public GameObject nextBackground;

    //Player Respawn Variables --------------
    [Header("PlayerRespawn")]
    public GameObject playerObject;
    private Vector3 respawnPosition = Vector3.zero;
    public float respawnTime;
    private Quaternion respawnRotation = Quaternion.identity;
        //Lives
    public int Lives = 3;
    public TextMeshProUGUI livesText;

    //Game States
    [Header("GameStates")]
    public GameState currentState = GameState.None;

    [Header("TrashSpawning")]
    public GameObject trashObject;
    private float trashSize;
    private bool spawningTrash = false;
    
    [Header("Enemies")]
    public GameObject[] enemies;
    public string[] enemyNames;
    public Dictionary<string, GameObject> finalEnemies = new Dictionary<string, GameObject>();
    private bool spawningEnemy = false;

    [Header("StageProgression")]
    public int stage = 0, level;
    struct Stage
    {
        public int numTrash, numEnemies, maxEnemies;
        public float enemyStart, enemySpawnRate;
        public string[] enemyList;

        public Stage(int numT, int numE, int maxE, float start, float rate, string[] list)
        {
            numTrash = numT;
            numEnemies = numE;
            maxEnemies = maxE;
            enemyStart = start;
            enemySpawnRate = rate;
            enemyList = list;
        }
    }
    Stage currentStage;

    // Dictionary<int, Stage> level1 = new Dictionary<int, Stage>{
    //     {1, new Stage(2, 0, 0, new string[] {"Swordfish"})},
    //     {2, new Stage(3, 0, 0, new string[] {"Swordfish"})},
    //     {3, new Stage(3, 1, 1, new string[] {"Swordfish"})},
    //     {4, new Stage(5, 1, 1, new string[] {"Swordfish"})},
    //     {5, new Stage(6, 1, 2, new string[] {"Swordfish"})}
    // };

    Stage[] level1 = {
        new Stage(2, 0, 0, 0, 0, new string[] {}),
        new Stage(3, 1, 1, 10, 20, new string[] {"Swordfish"}),
        new Stage(5, 1, 2, 5, 15, new string[] {"Swordfish"})
    };

    Stage[] level2 = {
        new Stage(3, 1, 1, 0, 0, new string[] {"Grouper", "Parrotfish"}),
        new Stage(4, 1, 2, 10, 15, new string[] {"Ray", "Grouper", "Parrotfish"}),
        new Stage(3, 2, 5, 5, 5, new string[] {"Ray", "Grouper", "Parrotfish"})
    };

    Stage[] level3 = {
        new Stage(4, 1, 1, 10, 20, new string[] {"AnglerFish"}),
        new Stage(5, 1, 2, 10, 15, new string[] {"AnglerFish", "Squid"}),
        new Stage(6, 2, 3, 5, 10, new string[] {"AnglerFish", "Squid", "Eel"})
    };

    Stage[] bossLevel = {
        new Stage(0, 1, 1, 0, 200, new string[] {"Boss"})
    };

    Stage[] testLevel = {
        new Stage(0, 1, 1, 2, 200, new string[] {"Eel"})
    };

    Stage[][] levels;
    Stage[] currentLevel;
    public int startingLevel;
    private int maxLevel = 4;


    void Awake()
    {
        if (Gary)
        {
            Destroy(this.gameObject);
        } else
        {
            Gary = this;
            DontDestroyOnLoad(this.gameObject);

            
            levels = new Stage[][]{level1, level2, level3, bossLevel};
            level = startingLevel;
            Debug.Log(level);
            SetLevel(startingLevel);

            trashSize = trashObject.transform.localScale.x - 0.2f;

            for(int i = 0; i < enemies.Length; i++)
            {
                finalEnemies.Add(enemyNames[i], enemies[i]);
            }

            score = 0;
            scoreText.text = "0";
            scoreText.enabled = true;

            lifeSprites = new GameObject[][]{level1Lives, level2Lives, level3Lives, bossLevelLives};

            UpdateLivesUI();

            //Set State to Preround
            currentState = GameState.PreRound;
            Debug.Log("Awake!");
        }
        
    }

    //Game States Function
    public void StartGame()
    {
        SoundManager.Sam.StopMenu();
        Debug.Log("Start");

        Lives = 3;
        for(int i = 0; i < lifeSprites.Length; i++)
        {
            if(i == level - 1)
            {
                foreach(GameObject life in lifeSprites[i])
                {
                    life.SetActive(true);
                }
            } else
            {
                foreach(GameObject life in lifeSprites[i])
                {
                    life.SetActive(false);
                }
            }
        }
        UpdateLivesUI();
        UpdateLevelUI();

        sceneAnimator = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<Animator>();
        // nextBackground = GameObject.FindGameObjectWithTag("NextBackground");
        // nextBackground.SetActive(false);
        // nextBackground.transform.position = Vector3.zero;

        ChangeStateToPlaying();
        if(level == maxLevel)
        {
            StartCoroutine(StartBossMusic());
            respawnPosition += new Vector3(0, 4, 0);
        } else
        {
            SoundManager.Sam.StartBackground(); 
        }

        BeginStage();

        Debug.Log(currentState);
    }

    private IEnumerator StartBossMusic()
    {
        while (SoundManager.Sam.bossIntro.isPlaying)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        SoundManager.Sam.StartBackground(); 
    }

    public void ChangeStateToPlaying()
    {
        currentState = GameState.Playing;
        Debug.Log("ChangeFunction");
        
    }

    void FixedUpdate()
    {
        if(currentState == GameState.Playing)
        {
            CheckForTrash();
        }
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }
//UIs ---------------------------------------------------
    private void UpdateScoreUI()
    {
        scoreText.text = "" + score;
    }

    private void UpdateLivesUI()
    {
        livesText.text = "Lives ";
        for(int i = 0; i < 3; i++)
        {
            lifeSprites[level-1][i].SetActive(i < Lives);
        }
    }

    private void UpdateLevelUI()
    {
        if(level != maxLevel)
        {
            levelText.text = "Level " + level;
        } else
        {
            levelText.text = "Level ?";
        }
    }

//Respawn ----------------------------------------------
    
    public void SetRespawnTimer()
    {
        Invoke(nameof(PlayerRespawn),respawnTime);
    }

    private void PlayerRespawn()
    {
         if (Lives > 1)
        {
            Lives -= 1;
        }
        else
        {
            currentState = GameState.PostRound;
            LevelManager.Larry.ReturnToMenu();
        }
        UpdateLivesUI();

        Instantiate(playerObject, respawnPosition, respawnRotation);
    }

//Spawning ------------------------------------------

    private void SpawnTrash()
    {
        for(int i = 0; i < currentStage.numTrash; i++)
        {
            bool verticalSide = UnityEngine.Random.Range(0, 1f) < 0.5f;
            float xPosition, yPosition;
            if (verticalSide)
            {
                yPosition = UnityEngine.Random.Range(-sceneHeight / 2, sceneHeight / 2);
                xPosition = (UnityEngine.Random.Range(0, 1f) < 0.5f ? 1 : -1) * ((sceneWidth / 2) + (trashSize / 2) - 0.1f);
            } else
            {
                xPosition = UnityEngine.Random.Range(-sceneWidth / 2, sceneWidth / 2);
                yPosition = (UnityEngine.Random.Range(0, 1f) < 0.5f ? 1 : -1) * ((sceneHeight / 2) + (trashSize / 2) - 0.1f);
            }
            Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0);

            Instantiate(trashObject, spawnPosition, Quaternion.identity);
        }
        spawningTrash = true;
    }

    private void SpawnEnemy()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0; i < Mathf.Min(currentStage.numEnemies, currentStage.maxEnemies - enemyObjects.Length); i++)
        {
            if(currentStage.enemyList[0] == "Boss")
            {
                Instantiate(finalEnemies["Boss"], Vector3.zero, Quaternion.identity);
            levelText.text = "Boss Level";
            } else
            {
                int spawn = (int) UnityEngine.Random.Range(0, currentStage.enemyList.Length);

                string enemyName = currentStage.enemyList[spawn];
                GameObject enemy = finalEnemies[enemyName];
                float enemyWidth = enemy.GetComponent<Collider2D>().bounds.size.x;

                float xPosition = (sceneWidth / 2) + (enemyWidth / 2);
                float yPosition = UnityEngine.Random.Range(-sceneHeight / 2, sceneHeight / 2);
                Instantiate(enemy, new Vector3(xPosition, yPosition, 0f), Quaternion.identity); 
            }
        }
        spawningEnemy = true;
    }

    private void StartSpawningEnemies()
    {
        spawningEnemy = true;
    }

    private void RepeatSpawnEnemies()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemyObjects.Length < currentStage.maxEnemies && spawningEnemy)
        {
            spawningEnemy = false;
            Invoke("SpawnEnemy", 2f);
        }
        Invoke("RepeatSpawnEnemies", currentStage.enemySpawnRate);
    }

    private void CheckForTrash()
    {
        GameObject[] trashObjects = GameObject.FindGameObjectsWithTag("Trash");
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        if(trashObjects.Length == 0 && spawningTrash)
        {
            spawningTrash = false;
            Invoke("ProgressStage", 2f);
        } else if(trashObjects.Length < 2 && enemyObjects.Length < currentStage.maxEnemies && spawningEnemy)
        {
            spawningEnemy = false;
            Invoke("SpawnEnemy", 2f);
        }
    }

// Levels and Stages ---------------------------------
    private void BeginStage()
    {
        CancelInvoke();
        
        spawningEnemy = false;
        spawningTrash = false;

        SpawnTrash();
        
        Invoke("StartSpawningEnemies", currentStage.enemyStart);
        Invoke("RepeatSpawnEnemies", currentStage.enemySpawnRate);
    }

    private void SetLevel(int n)
    {
        level = n;
        if(level > levels.Length)
        {
            LevelManager.Larry.ReturnToMenu();
        } else
        {
            currentLevel = levels[n - 1];
            // LevelManager.Larry.ChangeToSpecificScene("Stage" + n);
            SetStage(1);
        }
    }

    private void ProgressLevel()
    {
        level++;
        SoundManager.Sam.StopBackground();
        if(level > maxLevel)
        {
            GameObject tempPlayer = GameObject.FindGameObjectWithTag("Player");
            currentState = GameState.PostRound;
            if (tempPlayer)
            {
                PlayerScript script = tempPlayer.GetComponent<PlayerScript>();
                StartCoroutine(script.ReturnToMiddle());   
            }
            StartCoroutine(EndingTransition());
        } else
        {
            GameObject tempPlayer = GameObject.FindGameObjectWithTag("Player");
            currentState = GameState.PostRound;
            if (tempPlayer)
            {
                PlayerScript script = tempPlayer.GetComponent<PlayerScript>();
                StartCoroutine(script.ReturnToMiddle());
            }
            StartCoroutine(LevelTransition());
        }
    }

    private IEnumerator LevelTransition()
    {
        CancelInvoke();
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Enemy Bullet");
        foreach(GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in activeEnemies)
        {
            enemy.SendMessage("DeathAnim", SendMessageOptions.DontRequireReceiver);  
        }
        yield return new WaitForSeconds(1.5f);
        if(level == maxLevel)
        {
            SoundManager.Sam.PlayBossIntro();
        }
        sceneAnimator.SetTrigger("ChangeScene");

        for(int i = 0; i < 2; i++)
        {
            levelText.text = "Level " + (level - 1);
            yield return new WaitForSeconds(0.5f);
            levelText.text = "Level";
            yield return new WaitForSeconds(0.5f);
        }
        UpdateLevelUI();

        // nextBackground.SetActive(true);
        currentLevel = levels[level - 1];
        LevelManager.Larry.ChangeToNextScene();
        SetStage(1);
    }

    private IEnumerator EndingTransition()
    {
        CancelInvoke();
        yield return new WaitForSeconds(1.5f);
        sceneAnimator.SetTrigger("GameEnd");
        levelText.enabled = false;
        livesText.enabled = false;
        foreach(GameObject[] heads in lifeSprites){ 
            foreach(GameObject head in heads)
            {
                head.SetActive(false);
            }
        }
        yield return new WaitForSeconds(7.5f);
        LevelManager.Larry.ReturnToMenu();
    }

    private void SetStage(int n)
    {
        stage = n;
        if(stage > currentLevel.Length)
        {
            ProgressLevel();
        } else
        {
            currentStage = currentLevel[n - 1];
        }
    }

    private void ProgressStage()
    {
        stage++;
        if(stage > currentLevel.Length)
        {
            ProgressLevel();
        } else
        {
            currentStage = currentLevel[stage - 1];
            BeginStage();
        }
    }
}
