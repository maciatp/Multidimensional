using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerActivator_Script : MonoBehaviour
{
    public GameObject enemy_GO;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cage"))
        {
            GameObject enemy = Instantiate(enemy_GO, transform.position, transform.rotation, null);
            Destroy(gameObject);
        }
    }
}
