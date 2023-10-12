using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Boundaries_Script : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            other.gameObject.GetComponent<Bullet_Script>().DestroyBullet();
            //Debug.Log("He destruido una bala");
        }
    }
}
