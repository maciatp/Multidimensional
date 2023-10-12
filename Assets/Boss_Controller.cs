using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Controller : MonoBehaviour
{
    Transform bossGoalTransform;
    [SerializeField] bool isMovingTowardsGoal = false;
    [SerializeField] bool isActiveStage = false;
    [SerializeField] float speed = 1;
    EnemyHealth_Script health_Controller;
    Vector3 bossColliderSize;

    GameManager_Script gameManager;

    [SerializeField] UI_Advisor_Script uI_Advisor;
    bool isDangerousForPlayer = false;
    Vector3 playerColliderSize;

    public bool GetIsActive
    {
        get { return isActiveStage; }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        bossGoalTransform = GameObject.Find("BossGoal").transform;
        isMovingTowardsGoal = true;
        isActiveStage = false;
        health_Controller = transform.GetComponentInChildren<EnemyHealth_Script>();
        bossColliderSize = GetComponentInChildren<BoxCollider>().size;

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
        playerColliderSize = gameManager.playerInScene.GetComponent<BoxCollider>().size;

        DeactivateHitPoints();

        //deativate materialchecker .enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMovingTowardsGoal)
        {
            Vector3 distanceToGoal = transform.position - bossGoalTransform.transform.position ;


            if( distanceToGoal.z > 0) // > porque viene de delante
            {
                transform.position -= new Vector3(0, 0, speed * Time.deltaTime);

            }
            else
            {
                transform.position = bossGoalTransform.position;
                isMovingTowardsGoal = false;

                isActiveStage = true;
            }
        }

        if(isActiveStage)
        {
           

            //ATTACK, etc
        }

        //UI ADVISOR
        if(!uI_Advisor.IsVisible)
        {
            if((gameManager.playerInScene.transform.position.z + (playerColliderSize.z / 2)) >= transform.position.z - (bossColliderSize.z / 2))
            {
                uI_Advisor.ActivateAdvisorImage();
            }
        }

    }

    public void ActivateHitPoints()
    {
        if(gameManager.IsTopDimension)
        {
            foreach (Collider  hitPoint in health_Controller.topColliders)
            {
                hitPoint.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = true;
            }
        }
        else
        {
            foreach (Collider hitPoint in health_Controller.rightcolliders)
            {
                hitPoint.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = true;
            }
        }
       
    }

    public void DeactivateHitPoints()
    {
        foreach (Collider hitPoint in health_Controller.colliders)
        {
            hitPoint.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable = false;
        }
    }

}
