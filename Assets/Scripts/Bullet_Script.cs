using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Bullet_Script : MonoBehaviour
{
    [SerializeField] bool isChargedBullet = false;

    [SerializeField] Rigidbody bullet_Rb;
    [SerializeField] float speed = 50;
    [SerializeField] int damageWillMake = 1;
    [SerializeField] float bullet_TimeSpan = 3f;
    [SerializeField] TrailRenderer trail;
    [SerializeField] GameManager_Script _gameManager;
    [SerializeField] GameObject bulletDestroyedParticles;
     float bullet_Timer = 0;


    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public TrailRenderer Trail
    {
        get { return trail; }
        set { trail = value; }
    }

    public int GetDamageWillMake
    {
        get { return damageWillMake; }
    }

    private void Awake()
    {
        bullet_Rb = gameObject.GetComponent<Rigidbody>();
        trail = gameObject.GetComponentInChildren<TrailRenderer>();

        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();


    }


    void Start()
    {
        bullet_Rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        if(!isChargedBullet)
        {

            bullet_Timer += Time.deltaTime;
            if (bullet_Timer >= bullet_TimeSpan)
            {
                Destroy(gameObject);
            }

        }
        

    }

    public void IncreaseBulletDamage(int damageWillIncrease)
    {
        damageWillMake += damageWillIncrease;
    }


    public void DestroyBullet()
    {
        trail.autodestruct = true;
        trail.gameObject.transform.SetParent(null);
        GameObject particles = Instantiate(bulletDestroyedParticles, transform.position, bulletDestroyedParticles.transform.rotation, null);
        Destroy(gameObject);
    }
    
}
