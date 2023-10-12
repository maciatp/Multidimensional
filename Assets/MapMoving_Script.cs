using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMoving_Script : MonoBehaviour
{

    [SerializeField] float speed = 4;
    [SerializeField] Rigidbody map_rB;



    // Start is called before the first frame update
    void Start()
    {
        map_rB = gameObject.GetComponent<Rigidbody>();
        map_rB.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {

        if (map_rB.velocity != transform.forward * speed)
        {
            map_rB.velocity = transform.forward * speed;
        }
    }
}
