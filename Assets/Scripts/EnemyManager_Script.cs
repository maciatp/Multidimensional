using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyManager_Script : MonoBehaviour
{
    [SerializeField] bool mustSpawn = true;
    [SerializeField] GameManager_Script _gameManager;
    [SerializeField] float speed = 4;

    [SerializeField] float spawnCounter = 0;
    [SerializeField] float spawnTimeSpan = 4;

    
    public List<GameObject> enemies_List = new List<GameObject>();
    [SerializeField] int listLength;

    [Header("Activate to instance boss (testing)")]
    public bool spawnBossSwitch = false;
    [SerializeField] Transform bossGoalPositionTransform;
    public List<GameObject> bosses_List = new List<GameObject>();


    private void Awake()
    {

        //creo una lista de GO de los enemigos que hay en la carpeta Enemies_GO (Excluyo ENEMY ORIGINAL) para que no tenga que añadir cada enemigo a mano, pero no logro que funcione
        //Object[] enemies = AssetDatabase.LoadAllAssetsAtPath("Assets/Prefabs/Enemies/Enemies_GO/Enemy_Top");
        //Debug.Log(enemies[1].name);


        ////para cada elemento de la lista, añado ese enemigo_GO a la lista de enemigos (para usarla para spawnearlos)
        //foreach (GameObject _enemy in enemies)
        //{
        //    enemies_List.Add(_enemy as GameObject);
        //}



    }
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();

        listLength = enemies_List.Count;
        

        //METER el modelo 3d obst en un gameobject y mantener todos los componentes en el gameobject padre, para mantener la escala 1, 1,1 y que se aplique el tamaño al boxcollider
    }

    // Update is called once per frame
    void Update()
    {
        if(mustSpawn)
        {
            spawnCounter += Time.deltaTime;
            if (spawnCounter >= spawnTimeSpan)
            {
                //INSTANTIATE OBSTACLE, set scale as cage localScale
                CreateEnemy();
                spawnCounter = 0;
            }
        }

        //TESTING 
        if(spawnBossSwitch)
        {
            SpawnBoss();
            spawnBossSwitch = false;
        }
        
    }

    void CreateEnemy()
    {
        //Desvaríos? //HAcer que al spawnear enemigos, le pases una variable de la relevancia del enemigo a spawnear en ese momento, y que, teniendo en cuenta otra variable de la probabilidad de que salga ese enemigo en ese momento, y que con esa proba


        int random = Random.Range(0, listLength);

        //Debug.Log(random);

        //CREATE ENEMY BASED ON RANDOM AND LISTLENGHT (of Enemies)
        GameObject _enemy = Instantiate(enemies_List[random],

            //SetPosition inside Cage using Gameobject bounds
            new Vector3(Random.Range((-_gameManager.cageSize.x / 2) + (enemies_List[random].gameObject.GetComponentInChildren<BoxCollider>().size.x / 2),
                            (_gameManager.cageSize.x / 2) - (enemies_List[random].gameObject.GetComponentInChildren<BoxCollider>().size.x / 2)),
                            Random.Range((-_gameManager.cageSize.y / 2) + (enemies_List[random].gameObject.GetComponentInChildren<BoxCollider>().size.x/2), //size.y / 2), para que siempre tenga margen, sea Top enemy o no
                            (_gameManager.cageSize.y / 2) - (enemies_List[random].gameObject.GetComponentInChildren<BoxCollider>().size.x/2)),   //y / 2)),para que siempre tenga margen, sea Top enemy o no
                            gameObject.transform.position.z),

            //ROTATION -> must not modify rotation.Z!!! tiene que coger la Y de EnemySpawner, y la x y la z del enemigo elegido en el random
                   
            //SETEO LA ROTACIÓN. Para pasar a Quaternion, uso transform.eulerangles -> la rotación que se muestra en el editor.
         Quaternion.Euler(enemies_List[random].gameObject.transform.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y, enemies_List[random].gameObject.transform.eulerAngles.z), null);


        
        SetEnemyParameters(random, _enemy); //FUnciona, pero para testing mejor desactivarla
    }

    private void SetEnemyParameters(int random, GameObject _enemy)
    {
        if (random == 0)
        {
            //TOP ENEMY
            _enemy.name = "Enemy_Top";

        }
        else if (random == 1)
        {
            // RIGHT ENEMY
            _enemy.name = "Enemy_Right";
           

        }

        else if (random == 2)
        {
            //CHANGING ENEMY
            _enemy.name = "Enemy_Changing";
           

        }
        else if (random == 3)
        {
            //MULTI ENEMY
            _enemy.name = "Enemy_Multi";
           

        }

        _enemy.GetComponent<EnemyController_Script>().Speed = _enemy.GetComponent<EnemyController_Script>().Speed * (1 + _gameManager.CurrentLevel / 10); //-> a la speed del gameobj original se le suma currentlevel/10 (0.1, 0.2, etc.)
    }


    public void SpawnBoss()
    {
        GameObject _boss = Instantiate(bosses_List[0], transform.position, transform.rotation, null);
        _boss.name = "BOSS 1"; 
        Debug.Log("HOLA JEFE");

        //DontSpawn(); -> lo hago desde el gamemanager para tener más control

    }


    //Para cuando saque al boss, o si quiero que no spawnee por algo
    public void DontSpawnEnemies()
    {
        mustSpawn = false;
    }
    public void SpawnEnemiesActive()
    {
        mustSpawn = true;
    }

}
