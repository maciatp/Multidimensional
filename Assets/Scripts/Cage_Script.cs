using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            other.gameObject.GetComponent<Bullet_Script>().DestroyBullet();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if(other.CompareTag("Bullet"))
        //{
        //    Debug.Log("Tengo bullets");
        //}
    }
}
