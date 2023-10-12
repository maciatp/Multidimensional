using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitPoint_Script : MonoBehaviour
{
    [SerializeField] bool isHitPointVulnerable;
    [SerializeField] int hitPointHealthPoints = 1;

    [SerializeField] SphereCollider hitPointCollider;

    [SerializeField] EnemyController_Script _enemyController;
    [SerializeField] GameManager_Script _gameManager;
    public bool IsHitPointVulnerable
    { get { return isHitPointVulnerable; }
      set { isHitPointVulnerable = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        hitPointCollider = gameObject.GetComponent<SphereCollider>();

        _enemyController = gameObject.transform.parent.parent.parent.GetComponent<EnemyController_Script>();

        //_gameManager = _enemyController.GetGameManager.GetComponent<GameManager_Script>(); // Mejor así o buscarlo por GameObject find?? -> COMO LOS LLAMA EN START A LOS DOS, _gamemanager no está asignado aún aquí. quizá si en enemycontroller lo muevo al awake, pero eso moviendo todos las dependencias también
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>(); 
        DisableCollider();
    }

    // Update is called once per frame
    void Update()
    {
        //QUE SE ACTIVEN AL ENTRAR EN LA CAGE
        if(isHitPointVulnerable && !hitPointCollider.enabled && gameObject.transform.position.z <= (_gameManager.cageSize.z/2))
        {
            EnableCollider();
        }
        else if(!isHitPointVulnerable && hitPointCollider.enabled)
        {
            DisableCollider();
        }
        
    }


    void EnableCollider()
    {
        hitPointCollider.enabled = true;
    }
    void DisableCollider()
    {
        hitPointCollider.enabled = false;
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            Bullet_Script bullet = other.GetComponent<Bullet_Script>();

            if(!_enemyController.IsMultiEnemy && _enemyController.IsVulnerable) // && isHitPointVulnerable)
            {
                DamageHitPoint(bullet.GetDamageWillMake);
                bullet.DestroyBullet();
            }

            else if(_enemyController.IsMultiEnemy)
            {
                DamageHitPoint(bullet.GetDamageWillMake);
                bullet.DestroyBullet();
            }

        }
        if(other.CompareTag("Cage"))
        {
            hitPointCollider.enabled = true;
        }

    }

    private void DamageHitPoint(int damageWillTake) //int damage
    {
       hitPointHealthPoints -= damageWillTake;
        if(hitPointHealthPoints <= 0)
        {
            //DESTROY HITPOINT
            //ADD FX,sound, etc.

            _enemyController.gameObject.GetComponent<EnemyHealth_Script>().RemoveHitPoint(gameObject.GetComponent<SphereCollider>());
            gameObject.SetActive(false);
        }
    }
}
