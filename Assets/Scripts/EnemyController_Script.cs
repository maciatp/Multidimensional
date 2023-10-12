using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyController_Script : MonoBehaviour
{
    [SerializeField] bool isFixedEnemy = false;
    [SerializeField]private bool isTopEnemy = false;
    [SerializeField] bool isMultiEnemy = false;
    [SerializeField] bool isVulnerable = true;
    [SerializeField] bool isBossEnemy = false;
    [SerializeField] float speed = 5;
    [SerializeField] bool isChangingDimension = false;

    [SerializeField] int scoreWillAdd = 1;
    [SerializeField] float chanceOfPowerUpDropping;
    [SerializeField] int whichPowerUpWillSpawn = 0;
    [SerializeField] Rigidbody enemy_Rb = null;
    [SerializeField] EnemyHealth_Script _healthEnemyScript;

    [SerializeField] GameManager_Script _gameManager = null;

    [SerializeField] public BoxCollider enemyCollider = null;

    [SerializeField] Vector3 collider_MATCHingDimension;
    // [SerializeField] Vector3 collider_isRIGHTEnemy_MatchingDimensionSize;
    [SerializeField] Vector3 collider_UnMatchingDimension;

    [SerializeField] Vector3 originalColliderPosition;

    [SerializeField] GameObject hitPoints = null;
    [SerializeField] Vector3 originalHitPointsLocalPosition;

    [SerializeField] Camera mainCamera;

    //FX
    [SerializeField] GameObject enemyDestroyedParticles;


    public bool IsFixedEnemy
    {
        get { return isFixedEnemy; }
        set { isFixedEnemy = value; }
    }

    public bool IsTopEnemy
    {
        get { return isTopEnemy; }
        set { isTopEnemy = value; }
    }



    public bool IsMultiEnemy
    {
        get { return isMultiEnemy; }
        set { isMultiEnemy = value; }
    }


    public bool IsVulnerable
    {
        get { return isVulnerable; }
        set { isVulnerable = value; }
    }

    public bool IsBossEnemy
    {
        get { return isBossEnemy; }
    }


    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }



    public bool IsChangingDimension
    {
        get { return isChangingDimension; }
        set { isChangingDimension = value; }
        
    }




    public int ScoreWillAdd
    {
        get { return scoreWillAdd; }
        set { scoreWillAdd = value; }
    }

  
    public float ChanceOfPowerUpDropping
    {
        get { return chanceOfPowerUpDropping; }
        set { chanceOfPowerUpDropping = value; }
    }

    public int WhichPowerUpWillSpawnIfFixedEnemy
    {
        get { return whichPowerUpWillSpawn; }

    }

    public GameManager_Script GetGameManager
    {
        get { return _gameManager; }
    }

    


    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        _healthEnemyScript = gameObject.GetComponent<EnemyHealth_Script>();
        enemyCollider = gameObject.transform.GetChild(0).GetComponentInChildren<BoxCollider>();
        //enemyCollider = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().bounds; no porque el fbx son diferentes modelos compuestos. Si le paso un fbx de 3dmax en teoría debería funcionar. Se lo pasaría automático. Por ahora tengo que configurar el collider en el editor.
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
        originalColliderPosition = enemyCollider.transform.localPosition;
        mainCamera = _gameManager.mainCamera;

       

       //storing COLLIDER DIMENSIONS ACORDING TO CAGESIZE
       if(isTopEnemy)  //isTopEnemy -> Matches looking upwards when TopDimension
        {
            collider_MATCHingDimension = new Vector3(enemyCollider.size.x, _gameManager.cageSize.y, enemyCollider.size.z);
        
            collider_UnMatchingDimension = new Vector3(_gameManager.cageSize.x, enemyCollider.size.y, enemyCollider.size.z);


        }
        else
        {
            collider_MATCHingDimension = new Vector3(enemyCollider.size.x, _gameManager.cageSize.x, enemyCollider.size.z); //OJO! LA X de enemy collider se estira en vertical, pq el gameobject root está girado 90 grados!

            collider_UnMatchingDimension = new Vector3(_gameManager.cageSize.y, enemyCollider.size.x, enemyCollider.size.z);

        }

       if(isMultiEnemy)
        {
            IsVulnerable = true;
        }

        //SETTING SPEED
        enemy_Rb = gameObject.GetComponent<Rigidbody>();
        enemy_Rb.velocity = transform.forward * speed;

        hitPoints = gameObject.transform.GetChild(0).Find("HitPoints").gameObject;
        originalHitPointsLocalPosition = hitPoints.transform.localPosition;
    
    }

    // Update is called once per frame
    void Update()
    {

        // IS TOP ENEMY CHECKING
        float worldAngle = Vector3.Angle(transform.right, Vector3.down);
        if(worldAngle < 91)
        {
            isTopEnemy = true;
        }
        else
        {
            isTopEnemy = false;
        }

        //COMPROBANDO SI ES VULNERABLE (CUANDO ESTÉ DE COSTADO A LA Cámara

        float angleWithCameraBack = Vector3.Angle(transform.right, -mainCamera.transform.forward);
          
       // Debug.Log(worldAngle); // worldAngle



        //IS VULNERABLE CHECKING
        
        {
            if (isTopEnemy && !isMultiEnemy) 
            {
               //TOP ENEMY VULNERABLE
                if(angleWithCameraBack < 91)
                {

               
                isVulnerable = true;
                
                }
                //TOP ENEMY NO VULNERABLE
                else
                {
                    isVulnerable = false;
                }

            }
            else if(!IsTopEnemy && !isMultiEnemy)
            {
                //RIGHT ENEMY VULNERABLE
                if (angleWithCameraBack > 90)
                {

                    
                    isVulnerable = true;
                   
                }
                //RIGHT ENEMY NO VULNERABLE
                else
                {
                    isVulnerable = false;
                }
            }
        }


    
        //SE PUEDEN JUNTAR LOS 4 EN 2 ?? NOP

        if (_gameManager.IsTopDimension && isFixedEnemy && isTopEnemy && !isMultiEnemy && isVulnerable) //TopEnemy FIXED MATCHING Dimension
        {

            // Debug.Log("TOP FIXED VULNERABLE");
            //SETTING UP THE COLLIDER TO APPROPIATE SIZE AND POSITION
            enemyCollider.size = collider_MATCHingDimension;
            enemyCollider.transform.localPosition = new Vector3(originalColliderPosition.x, -gameObject.transform.position.y, originalColliderPosition.z);

            if (_gameManager.playerInScene != null)
            {
                //HITPOINTS RELOCATION
                hitPoints.transform.localPosition = new Vector3(hitPoints.transform.localPosition.x, -gameObject.transform.position.y + _gameManager.playerInScene.transform.position.y, hitPoints.transform.localPosition.z);
            }

        }
        else if (!_gameManager.IsTopDimension && isFixedEnemy && isTopEnemy  && !isMultiEnemy && !isVulnerable) //TOP ENEMY FIXED UNMATCHING Dimension
        {
            //Debug.Log("TOP FIXED NO VULNERABLE");
            //SETTING UP THE COLLIDER TO APPROPIATE SIZE AND POSITION
            enemyCollider.size = collider_UnMatchingDimension;
            enemyCollider.transform.localPosition = new Vector3(gameObject.transform.position.x, originalColliderPosition.y, originalColliderPosition.z);

            //HITPOINTS RELOCATION TO ORIGINAL LocalPOSITION
            hitPoints.transform.localPosition = originalHitPointsLocalPosition;
        }
        else if (!_gameManager.IsTopDimension && isFixedEnemy && !isTopEnemy && !isMultiEnemy && isVulnerable)//Right Enemy FIXED  MATCHING Dimension
        {
            //Debug.Log("RIGHT FIXED VULNERABLE");
            //SETTING UP THE COLLIDER TO APPROPIATE SIZE AND POSITION
            enemyCollider.size = collider_MATCHingDimension;
            enemyCollider.transform.localPosition = new Vector3(originalColliderPosition.y, -gameObject.transform.position.x, originalColliderPosition.z); //OJO! COJO LA X en el componente Y pq está girado 90º

            if (_gameManager.playerInScene != null)
            {
                //HITPOINTS RELOCATION                          //AQUÍ el negativo para que cambie de signo, ya que cuando va para el player está GIRADO
                hitPoints.transform.localPosition = new Vector3(hitPoints.transform.localPosition.x, (-gameObject.transform.position.x + _gameManager.playerInScene.transform.position.x), hitPoints.transform.localPosition.z);
            }
        }
        else if (_gameManager.IsTopDimension && isFixedEnemy && !isTopEnemy && !isMultiEnemy && !isVulnerable) //Right Enemy FIXED UNMATCHING Dimension
        {
            //Debug.Log("RIGHT FIXED NO VULNERABLE");
            //SETTING UP THE COLLIDER TO APPROPIATE SIZE AND POSITION
            enemyCollider.size = collider_UnMatchingDimension;
            enemyCollider.transform.localPosition = new Vector3(-gameObject.transform.position.y, originalColliderPosition.x, originalColliderPosition.z);

            //HITPOINTS RELOCATION TO ORIGINAL LocalPOSITION
            hitPoints.transform.localPosition = originalHitPointsLocalPosition;
        }
        else if (_gameManager.IsTopDimension && !isFixedEnemy && isTopEnemy && !isMultiEnemy && isVulnerable) // ENEMY  CHANGING  MATCHING TOP Dimension
        {
            enemyCollider.size = collider_MATCHingDimension;
            enemyCollider.transform.localPosition = new Vector3(originalColliderPosition.x, -gameObject.transform.position.y, originalColliderPosition.z);

            if (_gameManager.playerInScene != null)
            {
                //HITPOINTS RELOCATION
                hitPoints.transform.localPosition = new Vector3(hitPoints.transform.localPosition.x, -gameObject.transform.position.y + _gameManager.playerInScene.transform.position.y, hitPoints.transform.localPosition.z);
            }
        }
        else if (_gameManager.IsTopDimension && !isFixedEnemy && !isTopEnemy && !isMultiEnemy && !isVulnerable) // ENEMY  CHANGING  UNMATCHING TOP Dimension
        {
            //Debug.Log("RIGHT ENEMY  CHANGING  UNMATCHING TOP");
            //SETTING UP THE COLLIDER TO APPROPIATE SIZE AND POSITION
            enemyCollider.size = new Vector3(_gameManager.cageSize.y, collider_UnMatchingDimension.y, enemyCollider.size.z);
            enemyCollider.transform.localPosition = new Vector3(-gameObject.transform.position.y, originalColliderPosition.y, originalColliderPosition.z);

            //HITPOINTS RELOCATION TO ORIGINAL LocalPOSITION
            hitPoints.transform.localPosition = originalHitPointsLocalPosition;
        }
        else if (!_gameManager.IsTopDimension && !isFixedEnemy && isTopEnemy && !isMultiEnemy && !isVulnerable) // ENEMY  CHANGING  UNMATCHING RIGHT Dimension
        {
           // Debug.Log("RIGHT ENEMY  CHANGING  UNMATCHING");
            //SETTING UP THE COLLIDER TO APPROPIATE SIZE AND POSITION
            enemyCollider.size = new Vector3(_gameManager.cageSize.x, collider_UnMatchingDimension.y, enemyCollider.size.z);
            enemyCollider.transform.localPosition = new Vector3(gameObject.transform.position.x, originalColliderPosition.x, originalColliderPosition.z);

            //HITPOINTS RELOCATION TO ORIGINAL LocalPOSITION
            hitPoints.transform.localPosition = originalHitPointsLocalPosition;
        }

        else if (!_gameManager.IsTopDimension && !isFixedEnemy && !isTopEnemy && !isMultiEnemy && isVulnerable) // ENEMY  CHANGING  MATCHING RIGHT Dimension
        {
           // Debug.Log("RIGHT ENEMY  CHANGING  MATCHING");
            enemyCollider.size = new Vector3(collider_MATCHingDimension.x, _gameManager.cageSize.x, enemyCollider.size.z);
            enemyCollider.transform.localPosition = new Vector3(originalColliderPosition.x, -gameObject.transform.position.x, originalColliderPosition.z);

            if (_gameManager.playerInScene != null)
            {
                //HITPOINTS RELOCATION
                hitPoints.transform.localPosition = new Vector3(hitPoints.transform.localPosition.x, -gameObject.transform.position.x + _gameManager.playerInScene.transform.position.x, hitPoints.transform.localPosition.z);
            }
        }

        //me falta mover los hitpoints de rightcolliders a la posición X del player en enemycontroller cuando es RIGHTDIMENSION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        //ENEMY MULTI  TOP DIMENSION MATCHING (IS TOP ENEMY)
        if (_gameManager.IsTopDimension && isMultiEnemy && isTopEnemy)
        {
            //RESIZE COLLIDER?
            enemyCollider.size = collider_MATCHingDimension;
            enemyCollider.transform.localPosition = new Vector3(originalColliderPosition.x, -gameObject.transform.position.y, originalColliderPosition.z);


            //MOVING COLLIDERS  
            //MOVE TO PLAYER Y
            if (_gameManager.playerInScene != null)
            {
                //HITPOINTS RELOCATION
                hitPoints.transform.localPosition = new Vector3(hitPoints.transform.localPosition.x, -gameObject.transform.position.y + _gameManager.playerInScene.transform.position.y, hitPoints.transform.localPosition.z);
            }


        }


        //ENEMY MULTI RIGHT DIMENSION UNMATCHING (IS TOP ENEMY)
        else if (!_gameManager.IsTopDimension && isMultiEnemy && isTopEnemy)
        {

            enemyCollider.size = new Vector3(_gameManager.cageSize.x, collider_UnMatchingDimension.y, enemyCollider.size.z);
            enemyCollider.transform.localPosition = new Vector3(gameObject.transform.position.x, originalColliderPosition.y, originalColliderPosition.z);  //POSICIONO EL COLLIDER PARA QUE SIEMPRE EstÉ CENTRADO

            //MOVING COLLIDERS
            //MOVE TO PLAYER X
            if (_gameManager.playerInScene != null)
            {
                //HITPOINTS RELOCATION                          //AQUÍ el negativo para que cambie de signo, ya que cuando va para el player está GIRADO
                hitPoints.transform.localPosition = new Vector3((gameObject.transform.position.x - _gameManager.playerInScene.transform.position.x), hitPoints.transform.localPosition.y, hitPoints.transform.localPosition.z);
            }

            //MOVING TOP COLLIDERS TO ORIGINAL
            //foreach

        }

        //ENEMY MULTI RIGHT DIMENSION MATCHING
        else if (!_gameManager.IsTopDimension && isMultiEnemy && !IsTopEnemy)
        {

            enemyCollider.size = new Vector3(collider_MATCHingDimension.x, _gameManager.cageSize.x, enemyCollider.size.z);
            enemyCollider.transform.localPosition = new Vector3(originalColliderPosition.y, -gameObject.transform.position.x, originalColliderPosition.z); //OJO! COJO LA X en el componente Y pq está girado 90º

            if (_gameManager.playerInScene != null)
            {
                //HITPOINTS RELOCATION                          //AQUÍ el negativo para que cambie de signo, ya que cuando va para el player está GIRADO
                hitPoints.transform.localPosition = new Vector3(hitPoints.transform.localPosition.x, (-gameObject.transform.position.x + _gameManager.playerInScene.transform.position.x), hitPoints.transform.localPosition.z);
            }
        }

        //ENEMY MULTI TOP DIMENSION UNMATCHING (IS RIGHT ENEMY)
        else if (_gameManager.IsTopDimension && isMultiEnemy && !isTopEnemy)
        {
            enemyCollider.size = new Vector3(_gameManager.cageSize.y, collider_UnMatchingDimension.y, enemyCollider.size.z);
            enemyCollider.transform.localPosition = new Vector3(-gameObject.transform.position.y, originalColliderPosition.x, originalColliderPosition.z);

            if (_gameManager.playerInScene != null)
            {
                //HITPOINTS RELOCATION TO ORIGINAL LocalPOSITION     !!!! OJO AQUí en negativo porque está girado 90 grados. el negativo es en todos los que están girados?? Comprobar.
                hitPoints.transform.localPosition = new Vector3(-gameObject.transform.position.y + _gameManager.playerInScene.transform.position.y, hitPoints.transform.localPosition.y, hitPoints.transform.localPosition.z);

            }
        }


        //SI LA CáMARA SE MUEVE, que se quede en 0 hasta que pare
        if (_gameManager.isCameraMoving || isChangingDimension)
        {
            hitPoints.transform.localPosition = originalHitPointsLocalPosition;
        }
        
    }

    //YA LO HAGO CON GET/SET
    //public void SetSpeed(float _speed)
    //{
    //    speed = _speed;
    //}

    public void DestroyEnemy() // -> Kill Enemy para cuando lo "matas" (en enemyHealth_script), Destroy enemy es para la lógica
    {
        GameObject destroyedParticles = Instantiate(enemyDestroyedParticles, transform.position, enemyDestroyedParticles.transform.rotation, null);

        Destroy(gameObject);
    }


  public void ResetHitPointsPosition()
    {
        hitPoints.transform.localPosition = originalHitPointsLocalPosition;
    }


    private void OnTriggerEnter(Collider other)
    {
        if((other.CompareTag("Bullet") && !isVulnerable))
        {
            other.GetComponent<Bullet_Script>().DestroyBullet();
        }

        if(other.CompareTag("Cage"))
        {
            //enem
        }
    }
}
