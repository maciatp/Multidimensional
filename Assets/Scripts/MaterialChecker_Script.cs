using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChecker_Script : MonoBehaviour
{


   
    [SerializeField] Material invulnerable_Material = null;
    [SerializeField] Material vulnerable_Material = null;

    public List<MeshRenderer> meshRenderersList = new List<MeshRenderer>();

    [SerializeField] GameManager_Script _gameManager;
    [SerializeField] EnemyController_Script _enemyController_Script;
    [SerializeField] EnemyHitPoint_Script _enemyHitPoint_Script;


    Boss_Controller bossController;

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
        _enemyController_Script = gameObject.GetComponent<EnemyController_Script>();

        


        //AHORA CAMBIO EL MATERIAL DEL OBJETO. DEBERÍA CAMBIAR EL MATERIAL DE LAS HITPOINTS -> VULNERABLE / NO VULNERABLE

       // if(!_enemyController_Script.IsMultiEnemy)
        {
            //le asigno una variable para no tener que meter el getcomponent Meshrenderer en el bucle
            int meshRenderersCount = gameObject.transform.GetChild(0).Find("HitPoints").childCount; 

            for (int i = 0; i < meshRenderersCount; i++)
            {
                meshRenderersList.Add(gameObject.transform.GetChild(0).Find("HitPoints").GetChild(i).GetChild(0).GetComponent<MeshRenderer>()); //.GetComponent<MeshRenderer>());
            }

            if (_enemyController_Script.IsVulnerable)  //(((_enemyController_Script.IsTopEnemy && _gameManager.IsTopDimension) || (!_enemyController_Script.IsTopEnemy && !_gameManager.IsTopDimension)))
            {
               
                SetMaterialWhenActive();
            }
            else if (!_enemyController_Script.IsVulnerable)//(((_enemyController_Script.IsTopEnemy && !_gameManager.IsTopDimension) || (!_enemyController_Script.IsTopEnemy && _gameManager.IsTopDimension)))
            {
                //_enemyController_Script.IsVulnerable = false;
                SetInvulnerableMaterial();
            }
        }

        //ya me encargo en boss_controller
        //if(_enemyController_Script.IsBossEnemy)
        //{
        //    bossController = GetComponentInParent<Boss_Controller>();
        //    //SetInvulnerableMaterial();
        //}
        
    }

    // Update is called once per frame
    void Update()
    {
        
        {
            //AHORA CAMBIO EL MATERIAL DEL OBJETO. DEBERÍA CAMBIAR EL MATERIAL DE LAS HITPOINTS -> VULNERABLE / NO VULNERABLE
            if (!_enemyController_Script.IsMultiEnemy)
            {

                //SETTING VULNERABLE MATERIAL                        //OJO AL && QUE COMPRUEBA SI EL MATERIAL DEL PRIMER ELEMENTO NO ES IGUAL AL MATERIAL QUE VA A CAMBIAR
                if ((_enemyController_Script.IsVulnerable && meshRenderersList[0].GetComponent<MeshRenderer>().material != vulnerable_Material))// && ((_enemyController_Script.IsTopEnemy && _gameManager.IsTopDimension) || (!_enemyController_Script.IsTopEnemy && !_gameManager.IsTopDimension)))
                {
                    //_enemyController_Script.IsVulnerable = true;
                    SetMaterialWhenActive();
                }
                else if ((!_enemyController_Script.IsVulnerable && meshRenderersList[0].GetComponent<MeshRenderer>().material != invulnerable_Material))// && ((_enemyController_Script.IsTopEnemy && !_gameManager.IsTopDimension) || (!_enemyController_Script.IsTopEnemy && _gameManager.IsTopDimension)))
                {
                    //_enemyController_Script.IsVulnerable = false;
                    //SetInvulnerableMaterial();
                    SetMaterialWhenActive();
                }
            }




            //MULTI ENEMY MATERIAL CHANGING
            else if (_enemyController_Script.IsMultiEnemy)
            {
                //SETTING VULNERABLE MATERIAL
                if ((_enemyController_Script.IsVulnerable))// && meshRenderersList[0].GetComponent<MeshRenderer>().material != vulnerable_Material))// && ((_enemyController_Script.IsTopEnemy && _gameManager.IsTopDimension) || (!_enemyController_Script.IsTopEnemy && !_gameManager.IsTopDimension)))
                {

                    SetMaterialWhenActive();
                }
                else if ((!_enemyController_Script.IsVulnerable))// && meshRenderersList[0].GetComponent<MeshRenderer>().material != invulnerable_Material))// && ((_enemyController_Script.IsTopEnemy && !_gameManager.IsTopDimension) || (!_enemyController_Script.IsTopEnemy && _gameManager.IsTopDimension)))
                {
                    //_enemyController_Script.IsVulnerable = false;
                    // SetInvulnerableMaterial();
                    SetMaterialWhenActive();
                }
            }
        }

    }

    public void SetMaterialWhenActive()
    {
        //EL FOREACH Y EL FOR DE ABAJO FUNCIONAN IGUAL
        foreach (MeshRenderer meshRenderer in meshRenderersList)
        {
            if(meshRenderer.gameObject.transform.parent.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable)
            {
                meshRenderer.gameObject.GetComponent<MeshRenderer>().material = vulnerable_Material;

            }
            else
            {
                meshRenderer.gameObject.GetComponent<MeshRenderer>().material = invulnerable_Material;

            }
        }


    }
        //EL FOR Y EL FOREACH DE ABAJO FUNCIONAN IGUAL
    public void SetInvulnerableMaterial()
    {


        foreach (MeshRenderer meshRenderer in meshRenderersList)
        {
            //if (!meshRenderer.gameObject.transform.parent.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable)
            //{
                meshRenderer.gameObject.GetComponent<MeshRenderer>().material = invulnerable_Material;
            //}
            //else
            //{
            //    meshRenderer.gameObject.GetComponent<MeshRenderer>().material = vulnerable_Material;

            //}
        }

        //for (int i = 0; i < meshRenderersList.Count; i++)
        //{
        //    if (!meshRenderer.gameObject.transform.parent.GetComponent<EnemyHitPoint_Script>().IsHitPointVulnerable)
        //    {
        //        meshRenderersList[i].material = invulnerable_Material;
        //    }


        //}


    }

}
