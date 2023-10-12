using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerTrigger_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        //DEBO CAMBIARLO A QUE SE DESTRUYAN AL COLISIONAR CON CAGE ? -> COLISIONARáN CON EL ESCENARIO Y SE DESTRUirán?
        //if(other.CompareTag("Bullet"))
        //{
        //     //cuando están en el trigger, si han metido más de la mitad, hacerlos desaparecer (ya no se estarían viendo)
        //    if (( (gameObject.transform.position.z) - (other.transform.gameObject.transform.position.z)) >= (other.gameObject.transform.GetComponent<MeshRenderer>().bounds.size.z/2))
        //    {
        //        other.transform.GetComponent<Bullet_Script>().DestroyBullet();
        //    }
        //}

        if(other.transform.CompareTag("Obstacle"))
        {
            //DEBUGGING
           // Vector3 distance = new Vector3(0, 0, (gameObject.transform.position.z) - (other.transform.parent.gameObject.transform.position.z));
            //Debug.Log(distance);

            //cuando están en el trigger, si han metido más de la mitad, hacerlos desaparecer (ya no se estarían viendo)
            if (( (gameObject.transform.position.z) - (other.transform.parent.gameObject.transform.position.z)) >= (other.gameObject.transform.parent.GetComponent<Obstacle_Script>().obst_Collider.size.z/2))
            {
                other.transform.parent.GetComponent<Obstacle_Script>().DestroyObstacle();
            }

        }
       
        
        else if(other.transform.CompareTag("Enemy")  && other.transform.parent.name != "HitPoints")  //IF ENEMY, but not HITPOINTS child
        {
            //Debug.Log(other.name);
            //cuando están en el trigger, si han metido más de la mitad, hacerlos desaparecer (ya no se estarían viendo)
            if(((gameObject.transform.position.z) - (other.transform.parent.parent.gameObject.transform.position.z)) >= (other.gameObject.transform.parent.parent.GetComponent<EnemyController_Script>().enemyCollider.size.z/2))
            {         
                other.transform.parent.parent.GetComponent<EnemyController_Script>().DestroyEnemy();
            }

        }
    }
}
