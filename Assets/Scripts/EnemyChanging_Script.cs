using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChanging_Script : MonoBehaviour
{
    [SerializeField] Animator enemyAnimator;
    [SerializeField] bool isGoingToChangeDimension = false;
    

    [SerializeField] float changeEverySeconds = 3f;
    [SerializeField] EnemyController_Script _enemyController_Script;
    //[SerializeField] GameManager_Script _gameManager;

    [SerializeField] float changeChance = 1;

    private void Start()
    {
        enemyAnimator = gameObject.GetComponent<Animator>();
        _enemyController_Script = gameObject.GetComponent<EnemyController_Script>();
      
    }

    private void Update()
    {




        
        if(!isGoingToChangeDimension && !_enemyController_Script.IsChangingDimension)
        {
            //Random.Range(0, 1); for random changing

            //CHANGING DIMENSION
            //Getting if isTopDimension and sending it to method
            StartCoroutine(ChangeDimension(_enemyController_Script.IsTopEnemy));
        }
    }

    IEnumerator ChangeDimension(bool isCurrentlyTopEnemy)
    {
        //ROTARLO POR CÓDIGO EN LUGAR DE ANIMACION?


        isGoingToChangeDimension = true;
        yield return new WaitForSeconds(changeEverySeconds);

        _enemyController_Script.ResetHitPointsPosition();
        ActivateIsChanging();

        enemyAnimator.SetTrigger("ChangeDimension");
        enemyAnimator.SetBool("isEnemyTop", !isCurrentlyTopEnemy); //LE MANDO QUE SI ES DIMENSION TOP, le mande lo contrario (para que cambie)
        _enemyController_Script.IsTopEnemy = !isCurrentlyTopEnemy; //LE MANDO QUE SI ES DIMENSION TOP, le mande lo contrario (para que cambie)
        
        
        
    
    
    }


    //Las llamo desde Anim Events
    public void DeactivateIsChanging()
    {
        _enemyController_Script.IsChangingDimension = false;
    }
    public void ActivateIsChanging()
    {
        
        _enemyController_Script.IsChangingDimension = true;
        isGoingToChangeDimension = false;
    }

}
