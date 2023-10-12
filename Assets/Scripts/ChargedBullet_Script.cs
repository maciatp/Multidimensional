using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedBullet_Script : MonoBehaviour
{
    [SerializeField] Bullet_Script _bullet_Script;
    [SerializeField] Rigidbody bulletRb;
    [SerializeField] BoxCollider bulletCollider;
    [SerializeField] bool isShot = false;
    public bool IsShot
    { get { return isShot; }
      set { isShot = value; }
    }


    private void Start()
    {
        _bullet_Script = gameObject.GetComponent<Bullet_Script>();
        bulletRb = gameObject.GetComponent<Rigidbody>();
        bulletCollider = gameObject.GetComponent<BoxCollider>();


        //SET PARAMETERS AT START (NOT MOVE,NO COLLISIONS...)
        bulletRb.velocity = Vector3.zero;
        bulletCollider.enabled = false;
        _bullet_Script.Trail.emitting = false;
    }


    public void ShootChargedBullet()
    {
        //SETTING PARAMETERS TO SHOOT
        bulletCollider.enabled = true;
        bulletRb.velocity = transform.forward * _bullet_Script.Speed;
        gameObject.transform.parent = null;
        _bullet_Script.Trail.emitting = true;
    }
    public void DestroyChargedBullet()
    {
        //Debug.Log("he destruído charged bullet");
        Destroy(gameObject);
    }


}
