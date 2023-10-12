using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_Script : MonoBehaviour
{
    [SerializeField] public Camera mainCamera = null;
    [SerializeField] bool isTopDimension = true;
    [SerializeField] GameObject player_GO = null;
    public GameObject playerInScene = null;
    [SerializeField] Transform playerSpawnPos = null;
    [SerializeField] float playerRespawnTime = 2f;
    [SerializeField] bool playerIsRespawning = false;
    [SerializeField] float GAMEOVERDELAYINSECONDS = 3; //CONST


    [SerializeField] bool isGamePaused = false;

    [SerializeField] int currentLives = 3;
    [SerializeField] int addLifePointsSpan = 30;

    [SerializeField] public Vector3 cageSize;
    [SerializeField] public GameObject cage;


    [SerializeField] ScoreManager_Script _scoreManager; //añadido en editor

    [SerializeField] CanvasManager_Script _canvasManager;
    [SerializeField] EnemyManager_Script _enemyManager; //añadido en editor
    [SerializeField] ObstacleManager_Script _obstacleManager; //añadido en editor

    [SerializeField] float currentLevel = 1; //currentLevel = (currentScore * maxLevel) / scoreToMaxLevel
    [SerializeField] int maxLevel = 10; //CUáNTOS NIVELES HAY? APUNTAR AQUÍ el nivel máximo que quieras q haya 
    [SerializeField] float scoreToMaxLevel = 100; // cuántos puntos quieres que sean necesarios para llegar al nivel máximo?


    [SerializeField] bool isBossSpawned = false;

    public bool IsTopDimension
    {
        get { return isTopDimension; }
        set { isTopDimension = value; }
    }


    public bool isCameraMoving = false;
    public bool IsCameraMoving
    {
        get { return isCameraMoving; }
    }

    public bool IsGamePaused
    {
        get { return isGamePaused; }
        set { isGamePaused = value; }
    }

    
    public int CurrentLives
    {
        get { return currentLives; }
        set { currentLives = value; }
    }
    public int AddLifePointsSpan
    {
        get { return addLifePointsSpan; }
        set { addLifePointsSpan = value; }
    }


    public float CurrentLevel
    {
        get { return currentLevel; }
        set { currentLevel = value; }
    }

    
    public bool IsBossSpawned
    {
        get { return isBossSpawned; }
        set { isBossSpawned = value; }
    }



   
    public ScoreManager_Script GetScoreManager //-> por si otros managers necesitan acceso, mejor centralizarlo todo a través del gameManager, o tenerlo por separado y que GetScoreManager esté en ScoreManager? ir viendo.
    {
        get { return _scoreManager; }
       //set { _scoreManager = value; }
    }

    public CanvasManager_Script GetCanvasManager
    {
        get { return _canvasManager; }
        set { _canvasManager = value; }
    }

    

    private void Awake()
    {
        mainCamera = Camera.main;
        
        playerSpawnPos = GameObject.FindGameObjectWithTag("Respawn").transform; //también disponible con FindChild de la cage

        //Primero intento buscar un player en la escena (para testing, etc), si no hay,  lo creo
        playerInScene = GameObject.FindGameObjectWithTag("Player");


        //CREO UN PLAYER AL INICIAR LA ESCENA SI NO HAY NINGUNO
        if(playerInScene == null)
        {
            playerInScene = Instantiate(player_GO, playerSpawnPos.position, playerSpawnPos.rotation, null);
            playerInScene.name = "Player";
        }

        cage = GameObject.FindGameObjectWithTag("Cage").gameObject;
        cageSize = cage.transform.localScale;


       // _scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager_Script>(); // también puedo encontrarlo con transform.parent.find("ScoreManager") pq todos los managers son hijos del mismo Gameobject. -> AÑADIDO EN EDITOR

    }
    // Start is called before the first frame update
    void Start()
    {
        
        _canvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager_Script>();

       
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInScene == null && !playerIsRespawning && currentLives > 0)
        {
            StartCoroutine(PlacePlayerInGame());
        }

        //PENSAR UNA FORMA DE QUE IS TOP DIMENSION SE ACTIVE TENIENDO EN CUENTA DE DÓNDE VIENE 
        //(para que los hitpoints de los enemigos no se vea que se mueven. DEBEN MOVERSE EN EL PRIMER FRAME DEL MOVIMIENTO DE CAMBIO DE DIMENSION)

        //DETECCIÓN DE isTopDimension mediante la rotación de CameraOrbit (sólo entra en el primer bucle, no sé por qué. Seguir mirando con ChangeDimension de Movement_script) -> transform.rotation es LOCALROTATION, que se muestra en el editor debug
        if ((!isCameraMoving) && (Mathf.Abs(mainCamera.gameObject.transform.parent.transform.rotation.x) == 0f)) // <= 0.48f) //-90 grados es -0.5f; a la mitad (-0.25 en absoluto), que isTopDimension deje de ser true
        {
            //Debug.Log("CámaraTOP");
            //isTopDimension = true;
           
        }
        else if ((!isCameraMoving) && (Mathf.Abs(mainCamera.gameObject.transform.parent.transform.rotation.x) >= 0.49f))
        {
            //Debug.Log("Cámara RIGHT");
            //isTopDimension = false;
        }
    }

    private void FixedUpdate()
    {
        //Para que no suba más del nivel 10
        if(currentLevel < 10)
        {
            currentLevel = (_scoreManager.CurrentScore * maxLevel) / scoreToMaxLevel; // en el fixed update se llama menos veces, no?

            //para cuando lleves menos del 10% de topscore, que no baje del nivel 1
            if (currentLevel < 1)
            {
                currentLevel = 1;
            }
           
        }
        else //if(currentLevel >= 10 && !isBossSpawned) // Estás en level 10 -> sacar el Boss
        {
            currentLevel = 10;
            if(!isBossSpawned)
            {
                MustSpawnBoss();
                // SpawnBoss();
            }
        }




    }

    IEnumerator PlacePlayerInGame()
    {
        playerIsRespawning = true;
        
        yield return new WaitForSeconds(playerRespawnTime);
        SubstractLife();//resto una vida al colocar el player, así juega con lives = 0;
        playerInScene = Instantiate(player_GO, playerSpawnPos.position, playerSpawnPos.rotation, null);
        playerInScene.name = "Player";


        //SETTING PLAYER WHERE NEEDED AFTER RESPAWN
        GameObject.FindGameObjectWithTag("CameraOrbit").GetComponent<DisableAnimator_Script>().player = playerInScene;
        //GameObject.Find("Canvas").GetComponent<CanvasManager_Script>().playerInScene = playerInScene; -> accedo al de GameManager, por ahora
        GameObject.Find("UI_ChargeRing").GetComponent<UI_ChargeRing_Script>()._movement_Script = playerInScene.GetComponent<Player_Controller>();
        GameObject.Find("UI_ChargeRing").GetComponent<UI_ChargeRing_Script>().MakeRingVisible();
        GameObject.Find("Canvas").transform.Find("UI_GameplayScreen").Find("UI_ChargeMeters").GetComponent<UI_PowerLevel_Script>()._FireScript = playerInScene.GetComponent<Fire_Script>();
        GameObject.Find("UI_DashMeter").GetComponent<UI_DashMeter_Script>().SetMovementScript = playerInScene.GetComponent<Player_Controller>();
        


        isCameraMoving = false;

        
        playerIsRespawning = false;
    }



    public void MustSpawnBoss()
    {
        _enemyManager.SpawnBoss();
        IsBossSpawned = true;

        //Al menos para los primeros bosses
        _enemyManager.DontSpawnEnemies();
        _obstacleManager.DontSpawnObstacles();

    }


    public void SetTopDimension()
    {
        isTopDimension = true;
    }
    public void SetRightDimension()
    {
        isTopDimension = false;
    }

    public void AddLife()
    {
        CurrentLives++;
    }

    public void SubstractLife()
    {
        CurrentLives--;
        
        

    }
    public void GameOver()
    {
        //lives checked when player dies. then Gameover is called

        //bool gameover ?
        
        PlayerPrefs.SetFloat("currentMovementSpeed", player_GO.GetComponent<Player_Controller>().ORIGINAL_MOVEMENT_SPEED);
        PlayerPrefs.SetFloat("laserRafagaIntervalBetweenShots", player_GO.GetComponent<Fire_Script>().originalLaserInterval);
        PlayerPrefs.SetInt("currentPowerLevel", 1);
        PlayerPrefs.SetInt("powerUpsCollected", 0);
        PlayerPrefs.SetInt("canActivateSuperMode_BOOL", 0);

        StartCoroutine(GameOverWihtDelay());
        //_canvasManager.DeactivateGAMEPLAYscreen();
        //_canvasManager.ActivateGameOVERscreen();
        

    }


    IEnumerator GameOverWihtDelay()
    {
        
        yield return new WaitForSecondsRealtime(GAMEOVERDELAYINSECONDS);
        _canvasManager.DeactivateGAMEPLAYscreen();
        _canvasManager.ActivateGameOVERscreen();

    }

    //RESET SCENE
    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
