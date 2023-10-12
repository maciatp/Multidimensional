
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleManager_Script : MonoBehaviour
{
    [SerializeField] bool spawnObstacles = true;
    [SerializeField] float speed = 2;
    [SerializeField] float spawnCounter = 0;
    [SerializeField] float spawnTimeSpan = 4;

    [SerializeField] GameManager_Script _gameManager;

    //USO UNA LISTA
    //[SerializeField] GameObject obstacleHORIZ_GO = null;
    //[SerializeField] GameObject obstacleVERT_GO = null;
    public List<GameObject> obstacles_GO = new List<GameObject>();

    public List <Obstacle_Script> obstaclesInScene = new List<Obstacle_Script>();

    //YA HE CJUNTADO LOS DOS SCRIPTS
//    public List<ObstacleMinor_Script> obstaclesMINOrInScene = new List<ObstacleMinor_Script>();

    public List <GameObject> dangerousObstaclesList = new List<GameObject>();

    [SerializeField] UI_Advisor_Script UI_Advisor_Script;

 

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();


        //METER el modelo 3d obst en un gameobject y mantener todos los componentes en el gameobject padre, para mantener la escala 1, 1,1 y que se aplique el tamaño al boxcollider
        GameObject[] obstaclesAtStart = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach  (GameObject obstacle in obstaclesAtStart)
        {
            if(obstacle.gameObject.name != "Collider")
            {
                obstaclesInScene.Add(obstacle.gameObject.GetComponent<Obstacle_Script>());
            }
        }

        //UI ADVISOR
     //   UI_Advisor_Script = GameObject.Find("Canvas").transform.Find("UI_GameplayScreen").Find("UI_Advisor").GetComponent<UI_Advisor_Script>();// AÑadido en editor
    }

    // Update is called once per frame
    void Update()
    {

        //MUST SPAWN OBSTACLES? -> Así puedo desactivar que spawnee y sigue activando UI Advisor
       if(spawnObstacles)
        {
            spawnCounter += Time.deltaTime;
            if (spawnCounter >= spawnTimeSpan)
            {
                //INSTANTIATE OBSTACLE, set scale as cage localScale
                CreateObstacle();
                spawnCounter = 0;
            }
        }

        //LO LLEVO AL FIXEDUPDATE para que consuma menos
        ////compruebo todos los obstáculos de la lista. 
        //foreach (Obstacle_Script obstacle in obstaclesInScene)
        

       //UI ADVISOR OFF
        // si la lista de bools está vacía (no hay ningún obstáculo peligroso para el player, desactivar UI_Advisor)
        if(dangerousObstaclesList.Count == 0 && UI_Advisor_Script.IsVisible)
        {
            UI_Advisor_Script.DeactivateAdvisorImage();
        }
    }

    private void FixedUpdate()
    {
        //compruebo todos los obstáculos de la lista. 
        foreach (Obstacle_Script obstacle in obstaclesInScene)
        {
            //si el obstáculo supone un peligro (q está sobre el player), se añade el bool a la lista de bools SI no está ya añadido
            if (obstacle.IsDangerous && !dangerousObstaclesList.Contains(obstacle.gameObject))
            {
                //UI ADVISOR ON
                //si el player está debajo, debo activar UI_Advisor y añado el bool a la lista de bools
                UI_Advisor_Script.ActivateAdvisorImage();
                dangerousObstaclesList.Add(obstacle.gameObject); 
                
            }
            //si no es peligroso y está en la lista, lo borro de la lista
            else if (!obstacle.IsDangerous && dangerousObstaclesList.Contains(obstacle.gameObject))
            {   // si no está debajo, debo borrar el bool de la lista
                dangerousObstaclesList.Remove(obstacle.gameObject);
                //Debug.Log("UI ADVISOR OFF");
            }


        }

      
    
    }

    public void CreateObstacle()
    {

        int random = Random.Range(0, obstacles_GO.Count);


        if (random == 0) // -> TOP OBST (HORIZONTAL)
        {
            //Creo un random que esté dentro del rango de la Cage y sea mayor a la mitad de la ALTURA del gameobject(para que salga siempre dentro de los bordes)
            float yRandomLocation = Random.Range((-_gameManager.cageSize.y / 2) + ((obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.y / 2)),
                (_gameManager.cageSize.y / 2) - ((obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.y / 2)));



            //CREATE HORIZONTAL OBST usando el randomLocation
            GameObject horizontal_Obstacle = Instantiate(obstacles_GO[random], new Vector3(gameObject.transform.position.x, yRandomLocation, gameObject.transform.position.z), gameObject.transform.rotation, null);
            horizontal_Obstacle.GetComponent<Obstacle_Script>().SetObstacleParameters(_gameManager.cageSize, true, speed * (1 + (_gameManager.CurrentLevel / 10)), gameObject.GetComponent<ObstacleManager_Script>());//, cage.transform);
            horizontal_Obstacle.name = "Obstacle_Horizontal";
            //Debug.Log("he creado un obstáculo HORIZONTAL");
            horizontal_Obstacle.GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));

            obstaclesInScene.Add(horizontal_Obstacle.GetComponent<Obstacle_Script>());

        }
        else if( random == 1)  // RIGHT OBST (VERTICAL)
        {
            //Creo un random que esté dentro del rango de la Cage y sea mayor a la mitad de la ANCHURA del gameobject(para que salga siempre dentro de los bordes)
            float xRandomLocation = Random.Range((-_gameManager.cageSize.x / 2) + (obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.x / 2), //cojo la Y en bounds.size porque el obstáculo prefab es en horizontal, si añado más prefabs de obstáculos, crear el Vertical y cambiar aquí por bounds.size.x HECHO
                 (_gameManager.cageSize.x / 2) - ((obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.x / 2)));

            //CREATE VERTICAL OBST usando el randomLocation
            GameObject vertical_Obstacle = Instantiate(obstacles_GO[random], new Vector3(xRandomLocation, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation, null);
            vertical_Obstacle.GetComponent<Obstacle_Script>().SetObstacleParameters(_gameManager.cageSize, false, speed * (1 + (_gameManager.CurrentLevel / 10)), gameObject.GetComponent<ObstacleManager_Script>());//, cage.transform);
            vertical_Obstacle.name = "Obstacle_Vertical";
            //Debug.Log("he creado un obstáculo VERTICAL");
            vertical_Obstacle.GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));

            obstaclesInScene.Add(vertical_Obstacle.GetComponent<Obstacle_Script>());
        }

        else if(random == 2) //MINOR OBSTACLE
        {

            //RANDOM LOCATION WITHIN BOUNDS
            Vector2 randomXY = new Vector2(
                Random.Range((-_gameManager.cageSize.x / 2) + (obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.x / 2),
                ((_gameManager.cageSize.x / 2) - (obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.x / 2))),
                Random.Range((-_gameManager.cageSize.y / 2) + (obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.y / 2),
                ((_gameManager.cageSize.y / 2) - (obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.y / 2))));


            //CREATE MINOR OBST
            GameObject minor_Obstacle = Instantiate(obstacles_GO[random], new Vector3(randomXY.x, randomXY.y, gameObject.transform.position.z), gameObject.transform.rotation, null);
            minor_Obstacle.name = "Minor_Obstacle";

            minor_Obstacle.GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));

            obstaclesInScene.Add(minor_Obstacle.GetComponent<Obstacle_Script>());

            //YA HE JUNTADO LOS DOS SCRIPTS. NO HACE FALTA
            //obstaclesMINOrInScene.Add(minor_Obstacle.GetComponent<ObstacleMinor_Script>());
        }

        else if(random == 3) //ROTATING Obstacle
        {
            //CREATE OBST en el centro
            GameObject rotatingObstacle = Instantiate(obstacles_GO[random], gameObject.transform.position, gameObject.transform.rotation, null);


            //VELOCITY SETTING
            rotatingObstacle.GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));
        }

        else if(random == 4) //Changing OBstacle TOP
        {
            //Creo un random que esté dentro del rango de la Cage y sea mayor a la mitad de la ALTURA del gameobject(para que salga siempre dentro de los bordes)
            float yRandomLocation = Random.Range((-_gameManager.cageSize.y / 2) + ((obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.y / 2)),
                (_gameManager.cageSize.y / 2) - ((obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.y / 2)));



            //CREATE OBST  en el centro
            GameObject changinObstacle_TOP = Instantiate(obstacles_GO[random], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation, null);


            //VELOCITY SETTING
            changinObstacle_TOP.GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));

        }

        else if(random == 5) //CHANGING OBSTACLE RIGHT
        {
            //Creo un random que esté dentro del rango de la Cage y sea mayor a la mitad de la ANCHURA del gameobject(para que salga siempre dentro de los bordes)
            float xRandomLocation = Random.Range((-_gameManager.cageSize.x / 2) + (obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.x / 2), //cojo la Y en bounds.size porque el obstáculo prefab es en horizontal, si añado más prefabs de obstáculos, crear el Vertical y cambiar aquí por bounds.size.x HECHO
                 (_gameManager.cageSize.x / 2) - ((obstacles_GO[random].gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>().bounds.size.x / 2)));

            //CREATE VERTICAL OBST  en el centro
            GameObject changinObstacle_RIGHT = Instantiate(obstacles_GO[random], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation, null);

            //VELOCITY SETTING
            changinObstacle_RIGHT.GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));
        }
        else if(random == 6) //CHANGIN OBSTACLE MULTI
        {
            //CREATE VERTICAL OBST usando el randomLocation
            GameObject changinObstacle_MULTI = Instantiate(obstacles_GO[random], gameObject.transform.position, gameObject.transform.rotation, null);

            //VELOCITY SETTING
            changinObstacle_MULTI.transform.GetChild(0).GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));
            changinObstacle_MULTI.transform.GetChild(1).GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));

        }


        //COMPOSED OBSTACLES
        else if (random > 2)
        {

            GameObject composedObstacle = Instantiate(obstacles_GO[random], gameObject.transform.position, gameObject.transform.rotation, null);
            composedObstacle.name = "Composed_Obstacle" + composedObstacle.name;



            for (int i = 0; i < composedObstacle.transform.childCount; i++)
            {
                obstaclesInScene.Add(composedObstacle.transform.GetChild(i).GetComponent<Obstacle_Script>());
                composedObstacle.transform.GetChild(i).GetComponent<Rigidbody>().velocity = transform.forward * speed * (1 + (_gameManager.CurrentLevel / 10));
            }



        }







    }


    //Por si quiero que no spawnee obst (boss, etc)

    public void MustSpawn()
    {
        spawnObstacles = true;
    }

    public void DontSpawnObstacles()
    {
        spawnObstacles = false;
    }

}
