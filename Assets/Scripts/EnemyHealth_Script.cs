using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth_Script : MonoBehaviour
{
    public bool isEnemyAlive = true;

    [SerializeField] bool isMatchingTopSetup = false;
    [SerializeField] bool isMatchingRightSetup = false;

   

    [SerializeField] public List<SphereCollider> colliders = new List<SphereCollider>();
    [SerializeField] public List<SphereCollider> topColliders = new List<SphereCollider>();
    [SerializeField] public List<SphereCollider> rightcolliders = new List<SphereCollider>();



    [SerializeField] EnemyController_Script _enemyController;
    [SerializeField] PowerUpManager_Script _powerUpsManager;

   
    


    // Start is called before the first frame update
    void Start()
    {
        _enemyController = gameObject.GetComponent<EnemyController_Script>();
        _powerUpsManager = GameObject.Find("PowerUpManager").GetComponent<PowerUpManager_Script>();

        //Cuento cuántos hijos tiene HitPoints
        int collidersTotal = gameObject.transform.GetChild(0).Find("HitPoints").childCount;


        //añado cada componente sphere collider de los hijos de HitPoints
        for (int i = 0; i < collidersTotal; i++)
        {

            colliders.Add(gameObject.transform.GetChild(0).Find("HitPoints").GetChild(i).GetComponent<SphereCollider>());
        }


        //SE PODRÍA AÑADIR UN IF Childcount > 2 == ISENEMYMULTI de enemy controller -> NO. porque así puedo hacer enemigos que tengan más hitpoints y que sean fixed, sin ser MUlti. 
        //Lo tengo que activar desde el editor, en el prefab.
        
    

        //SEPARO TOPCOLLIDERS Y RIGHTCOLLIDERS SI ES MULTIENEMY
        if(_enemyController.IsMultiEnemy)
        {

            foreach  (SphereCollider collider in colliders)
            {
                
                //comprobar la Y. Si la Y es 0, los colliders (que son los Hitpoints), es que son hitpoints como en TOP ENEMY, vulnerables en TOP DIMENSION
                if(collider.transform.localPosition.y == 0)
                {
                    //ADD TO TOPCOLLIDERS
                    topColliders.Add(collider);

                }
                else
                {
                    //SI LA Y NO ES 0, comprobar la X. Si la X es 0, es que son hitpoints de MULTI ENEMY, VULNERABLES en RIGHT DIMENSION
                    if(collider.transform.localPosition.x == 0)
                    {
                        //ADD TO RIGHT COLLIDERS
                        rightcolliders.Add(collider);
                    }
                }
            }


        }
    
    }

    // Update is called once per frame
    void Update()
    {

        //TOP ENEMY MATCHING 
        if (_enemyController.GetGameManager.IsTopDimension && _enemyController.IsTopEnemy && !isMatchingTopSetup)   //SE PUEDEN CAMBIAR LAS 2 PRIMERAS CONDICIONES POR isVulnerable de EnemyController????
        {
            ActivateMatchingTOPHitPointsColliders();
        }
        else if (!_enemyController.GetGameManager.IsTopDimension && _enemyController.IsTopEnemy && !isMatchingRightSetup)    //_enemyController.GetGameManager.IsTopDimension
        {
            ActivateMathingRIGHThitPointsColliders();
        }

        //IF IS MULTI ENEMY TOP ENEMY, activate Matching colliders (method also deactivates unmatching colliders)
        if (_enemyController.GetGameManager.IsTopDimension && _enemyController.IsTopEnemy && !isMatchingTopSetup)
        {
            ActivateMatchingTOPHitPointsColliders();
        }
        else if(!_enemyController.GetGameManager.IsTopDimension && _enemyController.IsTopEnemy && !isMatchingRightSetup)
        {
            ActivateMathingRIGHThitPointsColliders();
        }

        //IF IS MULTI ENEMY RIGHT ENEMY, activate Matching colliders (method also deactivates unmatching colliders)
        if (_enemyController.GetGameManager.IsTopDimension && !_enemyController.IsTopEnemy && !isMatchingRightSetup)
        {
            ActivateMathingRIGHThitPointsColliders();
        }
        else if (!_enemyController.GetGameManager.IsTopDimension && !_enemyController.IsTopEnemy && !isMatchingTopSetup)
        {
            ActivateMatchingTOPHitPointsColliders();
        }


    }

    private void ActivateMatchingTOPHitPointsColliders()
    {
        isMatchingRightSetup = false;

        if(!_enemyController.IsMultiEnemy)
        {
            foreach (SphereCollider collider in colliders)
            {
               // collider.enabled = true; //QUIZÁ DEBERÍA CONTROLARSE DESDE HITPOINT_SCRIPT SEGÚN SEA VULNERABLE && Esté dentro de cage
                collider.gameObject.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = true;

            }
        }


        else if(_enemyController.IsMultiEnemy)
        {
            //ACTIVATE TOP COLLIDERS(MESHRENDERERS ALREADY ACTIVE)
            foreach (SphereCollider collider in topColliders)
            {
               // collider.enabled = true; //QUIZÁ DEBERÍA CONTROLARSE DESDE HITPOINT_SCRIPT SEGÚN SEA VULNERABLE && Esté dentro de cage
                collider.gameObject.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = true;
            }

            //DEACTIVATE RIGHT COLLIDERS(NOT MESHRENDERERS)
            foreach (SphereCollider collider in rightcolliders)
            {
                //collider.enabled = false; //QUIZÁ DEBERÍA CONTROLARSE DESDE HITPOINT_SCRIPT SEGÚN SEA VULNERABLE && Esté dentro de cage
                collider.gameObject.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = false;
            }


        }
        isMatchingTopSetup = true;
    }
    private void ActivateMathingRIGHThitPointsColliders()
    {
        isMatchingTopSetup = false;


        if (!_enemyController.IsMultiEnemy)
        {
            foreach (SphereCollider collider in colliders)
            {
                //collider.enabled = false; //QUIZÁ DEBERÍA CONTROLARSE DESDE HITPOINT_SCRIPT SEGÚN SEA VULNERABLE && Esté dentro de cage
                collider.gameObject.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = false;

            }
        }
        else if(_enemyController.IsMultiEnemy)
        {
            //ACTIVATE RIGHT COLLIDERS(NOT MESHRENDERERS)
            foreach (SphereCollider collider in rightcolliders)
            {
               // collider.enabled = true; //QUIZÁ DEBERÍA CONTROLARSE DESDE HITPOINT_SCRIPT SEGÚN SEA VULNERABLE && Esté dentro de cage
                collider.gameObject.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = true;
            }

            //DEACTIVATE TOP COLLIDERS(MESHRENDERERS ALREADY ACTIVE)
            foreach (SphereCollider collider in topColliders)
            {
                //collider.enabled = false; //QUIZÁ DEBERÍA CONTROLARSE DESDE HITPOINT_SCRIPT SEGÚN SEA VULNERABLE && Esté dentro de cage
                collider.gameObject.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = false;
            }
        }
        
        isMatchingRightSetup = true;
    }

    public void RemoveHitPoint(SphereCollider colliderDestroyed)
    {
        colliders.Remove(colliderDestroyed);
        if(colliders.Count == 0)
        {   
               //ENEMy killed when no hitpoints remaining
            KillEnemy();
        }
    }

    private void KillEnemy()
    {
        isEnemyAlive = false;

        //AÑADO los puntos aquí porque es cuando "matas" al enemigo. El otro método(DestroyEnemy) es para destruir el GameObject
        GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager_Script>().AddScore(gameObject.GetComponent<EnemyController_Script>().ScoreWillAdd);

        GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>().IncreaseChargeAtMAX();

        //SPAWN POWER UP
        //GameObject.FindGameObjectWithTag("PowerUpsManager").GetComponent<PowerUpManager_Script>().SpawnPowerUpAtLocation(transform.position);

        float random = Random.value;

        if(random <= (1 - _enemyController.ChanceOfPowerUpDropping))
        {
            //FIXED ENEMY -> Fixed PowerUP
            if(_enemyController.IsFixedEnemy && !_enemyController.IsMultiEnemy)
            {
                _powerUpsManager.SpawnPowerUpAtLocation(transform.position, _enemyController.WhichPowerUpWillSpawnIfFixedEnemy);

            }
            //CHANGING ENEMY OR MULTI ENEMY -> RANDOM PowerUp
            else if(!_enemyController.IsFixedEnemy || _enemyController.IsMultiEnemy)
            {
                int randomPowerUp = Random.Range(0, _powerUpsManager.powerUps.Count);

                _powerUpsManager.SpawnPowerUpAtLocation(transform.position, randomPowerUp);
            }
          

        }
        


        gameObject.GetComponent<EnemyController_Script>().DestroyEnemy();
    }
}
