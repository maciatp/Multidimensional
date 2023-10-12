using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Script : MonoBehaviour
{

    [SerializeField] bool isMinorObstacle;
    [SerializeField] bool isChangingObstacle;
    [SerializeField] bool isRotatingObstacle;

    [SerializeField] Rigidbody obst_Rb;
    [SerializeField] float speed = 3;

    [SerializeField] bool isHorizontalObstacle = false;
    [SerializeField] bool isMatching = false;
    [SerializeField] int scoreWillAdd = 1;

    [SerializeField]public BoxCollider obst_Collider;

    //[SerializeField] Transform cageTransform;

    [SerializeField] Vector3 extendedColliderScale;
    [SerializeField] Vector3 contractedColliderScale; //puedo cambiarle el nombre por collider originalSize?

    //MINOR OBSTACLES
    [SerializeField] Vector3 originalColliderLocalPosition;
    [SerializeField] Vector3 originalColliderSize;

    [SerializeField] GameManager_Script _gameManager;
    [SerializeField] Vector3 playerColliderSize;
    [SerializeField] float marginForPlayer = 0.5f;

    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] ObstacleChangingDimension_Script _changing_Script;


    // UI ADVISOR
    
    [SerializeField] ObstacleManager_Script _obstacleSpawnerManager;
    [SerializeField] bool isDangerousForPlayer = false;
    public bool IsDangerous
    {
        get { return isDangerousForPlayer; }
        set { isDangerousForPlayer = value; }
    }


    private void Awake()
    {

        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
        obst_Rb = gameObject.GetComponent<Rigidbody>();


        if(isMinorObstacle)
        {
            //SET COLLIDER
            obst_Collider = gameObject.transform.Find("Collider").GetComponent<BoxCollider>();
            originalColliderLocalPosition = obst_Collider.transform.localPosition;

           
        }
        else //HACEN LO MISMO, NO?
        {

            obst_Collider = gameObject.GetComponentInChildren<BoxCollider>();

        }

            originalColliderSize = obst_Collider.size;
            _meshRenderer = gameObject.transform.Find("model").GetComponentInChildren<MeshRenderer>();

        if(isChangingObstacle)
        {

            _changing_Script = gameObject.GetComponent<ObstacleChangingDimension_Script>();
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //SI EL PLAYER HA MUERTO Y SE INSTANCIA EN ESE MOMENTO, hago que se pase al update
        if(_gameManager.playerInScene != null)
        {

            playerColliderSize = _gameManager.playerInScene.GetComponentInChildren<BoxCollider>().size;
        }
        //SET SPEED
       // obst_Rb.velocity = transform.forward * speed;


        //COJE EL MANAGER AL INICIARSE
        if (_obstacleSpawnerManager == null)
        {
            _obstacleSpawnerManager = GameObject.Find("Obstacle_Spawner").GetComponent<ObstacleManager_Script>();
        }
    }

    // Update is called once per frame
    void Update()
    {

        //si el player aparece cuando ya se ha instanciado el obstáculo, así cojo el playerColliderSize
        if(playerColliderSize == null && _gameManager.playerInScene != null)
        {
            playerColliderSize = _gameManager.playerInScene.GetComponentInChildren<BoxCollider>().size;
        }


        //CHANGING OBSTACLE
        if(isChangingObstacle)
        {

            // IS TOP ENEMY CHECKING
            float worldAngle = Vector3.Angle(transform.right, Vector3.down);

            //de esta forma se activa al terminar la rotación, permitiendo al player pasar apuradamente
            if(_gameManager.IsTopDimension)
            {

                if (worldAngle < 91)
                {
                    isHorizontalObstacle = true;
                }
                else
                {
                    isHorizontalObstacle = false;
                }
            }
            else
            {
                if (worldAngle < 180)
                {
                    isHorizontalObstacle = true;
                }
                else
                {
                    isHorizontalObstacle = false;
                }
            }


            //COMPROBANDO SI está MATCH CON LA CÁMARA (CUANDO ESTé plano en TOP dimens, y Vertical en Right Dim)

            float angleWithCameraFront = Vector3.Angle(transform.right, _gameManager.mainCamera.transform.forward);
            //Debug.Log(worldAngle);



            //de esta forma se activa al terminar la rotación, permitiendo al player pasar apuradamente
            if(_gameManager.IsTopDimension)
            {
                // angle matching top dim = 90 -> a la mínima que se mueva deja de ser matching, por lo que cambia el collider.
                if(angleWithCameraFront < 91)
                {
                    isMatching = true;
                }
                else
                {
                    isMatching = false;
                }
            }
            else
            {
                // angle matching top dim = 90 -> a la mínima que se mueva deja de ser matching, por lo que cambia el collider.
                if(angleWithCameraFront < 90)
                {
                    isMatching = false;
                }
                else
                {
                    isMatching = true;
                }
            }

           

        }

        else if(isRotatingObstacle)
        {
            // IS TOP ENEMY CHECKING
            float worldAngle = Vector3.Angle(transform.right, Vector3.down);
            //Debug.Log(worldAngle);

            //de esta forma se activa al terminar la rotación, permitiendo al player pasar apuradamente
            if (_gameManager.IsTopDimension)
            {

                if (worldAngle < 91)
                {
                    isHorizontalObstacle = true;
                }
                else
                {
                    isHorizontalObstacle = false;
                }
            }
        }





        //TOP DIMENSION
        if (_gameManager.IsTopDimension)
        {

            //MINOR OBSTACLE
            if(isMinorObstacle)
            {
                //SET TOP DIMENSION SIZE
                obst_Collider.size = new Vector3(originalColliderSize.x, _gameManager.cageSize.y, originalColliderSize.z);

                //SET COLLIDER POSITION
                obst_Collider.transform.localPosition = new Vector3(originalColliderLocalPosition.x, -gameObject.transform.position.y, originalColliderLocalPosition.z);
            }
            else if(!isChangingObstacle) //BIG FIXED OBSTACLE
            {
                // Si es obst horiz y estás en top view, debe verse inesquivable, por tanto EXTENDED COLLIDER SCALE
                if ((isHorizontalObstacle) && (obst_Collider.size != extendedColliderScale))
                {
                    obst_Collider.size = extendedColliderScale;

                    //posiciono el collider dentro de la cage independientemente de la posición del obstáculo (sólo cuando estás en la dimensión que no puedes esquivarlo, para que no queden huecos libres
                    obst_Collider.transform.localPosition = new Vector3(0, -gameObject.transform.position.y, 0);



                }
                //si es obst vertical, pues tiene que ser pequeño, CONTRACTED
                else if ((!isHorizontalObstacle) && (obst_Collider.size != contractedColliderScale))
                {
                    obst_Collider.size = contractedColliderScale;
                    obst_Collider.transform.localPosition = Vector3.zero;
                }
            }
            //CHANGING OBSTACLE
            else if(isChangingObstacle)
            {
                //IS horizontal -> Extend collider
                if(isHorizontalObstacle && isMatching)//  && !_changing_Script.IsRotating)
                {
                    obst_Collider.size = new Vector3(_gameManager.cageSize.x, _gameManager.cageSize.y, obst_Collider.size.z);


                }
                //IS VERTICAL -> CONTRACT COLLIDER
                else if(!isHorizontalObstacle && !isMatching)// && _changing_Script.IsRotating)
                {
                    obst_Collider.size = new Vector3(_gameManager.cageSize.y, originalColliderSize.y, obst_Collider.size.z);
                }

            }
            
        }
        //RIGHT DIMENSION
        else
        {
            //MINOR OBSTACLE
            if(isMinorObstacle) 
            {
                obst_Collider.size = new Vector3(_gameManager.cageSize.x, originalColliderSize.y, originalColliderSize.z);

                //SET COLLIDER POSITION
                 obst_Collider.transform.localPosition = new Vector3(gameObject.transform.position.x, originalColliderLocalPosition.y, originalColliderLocalPosition.z);
            }
            //BIG FIXED OBSTACLE
            else if(!isChangingObstacle)
            {
                //SI ESTAS EN RIGHT VIEW, los obst vertic son los que deben verse grandes, EXTENDED
                if ((isHorizontalObstacle) && (obst_Collider.size != contractedColliderScale))
                {
                    obst_Collider.size = contractedColliderScale;
                    obst_Collider.transform.localPosition = Vector3.zero;
                }
                else if ((!isHorizontalObstacle) && (obst_Collider.size != extendedColliderScale))
                {
                    obst_Collider.size = extendedColliderScale;

                    //posiciono el collider dentro de la cage independientemente de la posición del obstáculo (sólo cuando estás en la dimensión que no puedes esquivarlo, para que no queden huecos libres
                    obst_Collider.transform.localPosition = new Vector3(gameObject.transform.position.x, 0, 0);   //IMPORTANTE EL MENOS del Vector3. Es porque el obstáculo está girado 


                }
                

            }
            //CHANGING OBSTACLE
            else if (isChangingObstacle)
            {
                //IS HORIZONTAL -> contract collider
                if(isHorizontalObstacle && !isMatching)// && !_changing_Script.IsRotating)
                {
                    obst_Collider.size = new Vector3(_gameManager.cageSize.x, originalColliderSize.y, obst_Collider.size.z);
                }
                //IS VERTICAL -> extend collider
                else if(!isHorizontalObstacle && isMatching)// && !_changing_Script.IsRotating)
                {
                    obst_Collider.size = new Vector3(_gameManager.cageSize.y,_gameManager.cageSize.x, obst_Collider.size.z);
                }

            }


        }


        ////UI ADVISOR
       
        if (_gameManager.playerInScene != null)
        {
            //MINOR OBSTACLE
            if(isMinorObstacle)
            {
                        //IF TOP DIMENSION
                        if (_gameManager.IsTopDimension)
                        {
                            //IF ON PLAYER POSITION IN OTHER DIMENSION -> Como el obstáculo es más pequeño, tengo que comprobar una dimensión más, en este caso la y + la z original (del obstacle grande)
                            //CALCULO si se solapan las posiciones, delante y detrás (teniendo en cuenta que está mirando a -Forward), teniendo en cuenta el tamaño de las hitboxes para que se reste bien, y que sea que coinciden las posiciones en dos de las dimensiones de la posición
                            if (((_gameManager.playerInScene.transform.position.z + ((playerColliderSize.z / 2) )) >= transform.position.z - ((originalColliderSize.z / 2 +marginForPlayer))) &&   //front checking
                                 ((_gameManager.playerInScene.transform.position.z - ((playerColliderSize.z / 2) )) <= transform.position.z + ((originalColliderSize.z / 2 +marginForPlayer))) &&
                            ((_gameManager.playerInScene.transform.position.y + ((playerColliderSize.y / 2) )) <= transform.position.y + ((originalColliderSize.y / 2 +marginForPlayer))) &&
                                ((_gameManager.playerInScene.transform.position.y - ((playerColliderSize.y / 2) )) >= transform.position.y - ((originalColliderSize.y / 2 +marginForPlayer))))
                            {
                                isDangerousForPlayer = true;
                                //Debug.Log("DANGER");
                            }
                            ////NOT IN PLAYER Z
                            //CALCULO si se solapan las posiciones, delante y detrás (teniendo en cuenta que está mirando a -Forward), teniendo en cuenta el tamaño de las hitboxes para que se reste bien
                            else if (((_gameManager.playerInScene.transform.position.z + ((playerColliderSize.z / 2) )) < transform.position.z - ((originalColliderSize.z / 2) +marginForPlayer))         //front checking
                                || ((_gameManager.playerInScene.transform.position.z - ((playerColliderSize.z / 2) )) > transform.position.z + ((originalColliderSize.z / 2) +marginForPlayer)) ||
                                ((_gameManager.playerInScene.transform.position.y + ((playerColliderSize.y / 2) )) > transform.position.y + ((originalColliderSize.y / 2) +marginForPlayer))         //front checking
                                || ((_gameManager.playerInScene.transform.position.y - ((playerColliderSize.y / 2) )) < transform.position.y - ((originalColliderSize.y / 2) +marginForPlayer)))     //back checking  //PLAYER IS NEITHER IN FRONT OR BACK BOUNDARIES 
                            {
                                isDangerousForPlayer = false;
                                //Debug.Log("NO DANGER");
                            }


                        }
                        else // NOT TOP DIMENSION
                        {
                            if (((_gameManager.playerInScene.transform.position.z + ((playerColliderSize.z / 2) )) >= transform.position.z - ((originalColliderSize.z / 2 +marginForPlayer))) &&   //front checking
                                 ((_gameManager.playerInScene.transform.position.z - ((playerColliderSize.z / 2) )) <= transform.position.z + ((originalColliderSize.z / 2 +marginForPlayer))) &&
                            ((_gameManager.playerInScene.transform.position.x + ((playerColliderSize.x / 2) )) <= transform.position.x + ((originalColliderSize.x / 2 +marginForPlayer))) &&
                           ((_gameManager.playerInScene.transform.position.x - ((playerColliderSize.x / 2) )) >= transform.position.x - ((originalColliderSize.x / 2 +marginForPlayer))))
                            {

                                isDangerousForPlayer = true;
                                //Debug.Log("DANGER");
                            }
                            ////NOT IN PLAYER Z
                            //CALCULO si se solapan las posiciones, delante y detrás (teniendo en cuenta que está mirando a -Forward), teniendo en cuenta el tamaño de las hitboxes para que se reste bien
                            else if (((_gameManager.playerInScene.transform.position.z + ((playerColliderSize.z / 2) )) < transform.position.z - ((originalColliderSize.z / 2) +marginForPlayer))         //front checking
                                || ((_gameManager.playerInScene.transform.position.z - ((playerColliderSize.z / 2) )) > transform.position.z + ((originalColliderSize.z / 2) +marginForPlayer)) ||
                                ((_gameManager.playerInScene.transform.position.x + ((playerColliderSize.x / 2) )) > transform.position.x + ((originalColliderSize.x / 2) +marginForPlayer))         //front checking
                                || ((_gameManager.playerInScene.transform.position.x - ((playerColliderSize.x / 2) )) < transform.position.x - ((originalColliderSize.x / 2) +marginForPlayer)))     //back checking  //PLAYER IS NEITHER IN FRONT OR BACK BOUNDARIES 
                            {
                                isDangerousForPlayer = false;
                                //Debug.Log("NO DANGER");
                            }
                        }
            }
            //BIG obstacle
            else
            {
                //IF ON PLAYER Z
                if (((_gameManager.playerInScene.transform.position.z + (playerColliderSize.z / 2)) >= transform.position.z - (obst_Collider.size.z / 2)) &&   //front checking
                    ((_gameManager.playerInScene.transform.position.z - (playerColliderSize.z / 2)) <= transform.position.z + (obst_Collider.size.z / 2)))       //back checking //PLAYER IS IN FRONT OR BACK BOUNDARIES
                {
                    isDangerousForPlayer = true;
                    //Debug.Log("DANGER");
                }
                //IF NOT IN PLAYER Z
                else if (((_gameManager.playerInScene.transform.position.z + (playerColliderSize.z / 2)) < transform.position.z - (obst_Collider.size.z / 2))         //front checking
                    || (_gameManager.playerInScene.transform.position.z - (playerColliderSize.z / 2) > transform.position.z + (obst_Collider.size.z / 2)))    //back checking  //PLAYER IS NEITHER IN FRONT OR BACK BOUNDARIES 
                {
                    isDangerousForPlayer = false;
                    //Debug.Log("NO DANGER");
                }




            }

          


           
        }

    }

    //la llamo como constructor cuando creo cada obstáculo
    public void SetObstacleParameters(Vector3 _scale, bool _isHorizontal, float _speed, ObstacleManager_Script obstacleSpawner_)//, Transform _cageTransform)
    {
        speed = _speed;

        extendedColliderScale = new Vector3(_scale.x, _scale.y, _meshRenderer.bounds.size.z);
        
        isHorizontalObstacle = _isHorizontal;

        if(_isHorizontal)
        {
            //SET LOCALSCALE TO MAKE HORIZONTAL OBST. TENGO EN CUENTA EL TAMAÑO DEL MODELO 3D Con _meshrenderer
            //accedo a la escala del fbx a través de mesherenderer.gameobject
           _meshRenderer.gameObject.transform.localScale = new Vector3(_scale.x, _meshRenderer.bounds.size.y, _meshRenderer.bounds.size.z);
            
            contractedColliderScale = new Vector3(_scale.x, _meshRenderer.bounds.size.y, _meshRenderer.bounds.size.z);

        }
        else
        {
            //SET LOCALSCALE TO MAKE VERTICAL OBST
            _meshRenderer.gameObject.transform.localScale = new Vector3(_meshRenderer.bounds.size.x, _scale.y, _meshRenderer.bounds.size.z);//COJO EL TAMAÑO Z DEL FBX scale z Del obstáculo. Más adelante cambiar por una variable para que el scaleZ sea random
            contractedColliderScale = new Vector3(gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.x, _scale.y, gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.z);
        
        }
        
        

       // cageTransform = _cageTransform;
    }

    public void DestroyObstacle()
    {
        //OBST añaden puntos al destruirse??
        //GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager_Script>().AddScore(scoreWillAdd);

        _obstacleSpawnerManager.obstaclesInScene.Remove(gameObject.GetComponent<Obstacle_Script>());

        if(_obstacleSpawnerManager.dangerousObstaclesList.Contains(gameObject))
        {
            _obstacleSpawnerManager.dangerousObstaclesList.Remove(gameObject);
        }
        Destroy(gameObject);
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Player_Controller player = other.GetComponent<Player_Controller>();

            if ((!player.isInvulnerable) && (!_gameManager.IsCameraMoving))// && (!_changing_Script.IsRotating)) //Cuando NO es Invulnerable, OJO
            {
                //Debug.Log("Player KILLED");
                player.KillPlayer();
            }
        }
        
        else if(other.CompareTag("Bullet"))
        {
            other.GetComponent<Bullet_Script>().DestroyBullet();
        }
            
    }
}
