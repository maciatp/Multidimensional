using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager_Script : MonoBehaviour
{
    [SerializeField] float chanceTopUniDimensional;
    [SerializeField] float chanceRightUniDimensional;
    [SerializeField] float chanceMultidimensional;


    [SerializeField]public List<GameObject> powerUps = new List<GameObject>();

   

    public void SpawnPowerUpAtLocation(Vector3 location, int powerUpWillSpawn) // por ahora instancia el power up que se le pasa por random y ya.
    {
        //int random = Random.Range(0, powerUps.Count);
        // GameObject _powerUp = Instantiate(powerUps[random].gameObject, location, powerUps[random].transform.rotation, null);

        //_powerUp.name = powerUps[random].gameObject.name;



        //selecciono en el enemigo qué power up puede spawnear, hago el random cuando muere el enemigo, e instancio el power up que le paso

        GameObject _powerUp = Instantiate(powerUps[powerUpWillSpawn].gameObject, location, powerUps[powerUpWillSpawn].transform.rotation, null);
        _powerUp.name = powerUps[0].gameObject.name;



    }

}
