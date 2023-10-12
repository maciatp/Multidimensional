using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMinor_Script : MonoBehaviour
{
    [SerializeField] bool isDangerousForPlayer = false;
    public bool IsDangerousForPlayer
    {
        get { return isDangerousForPlayer; }
        set { isDangerousForPlayer = value; }
    }
    [SerializeField] Rigidbody minorObst_rB;
    [SerializeField] BoxCollider minorObst_Collider;
    [SerializeField] float speed = 3;
    [SerializeField] Vector3 originalColliderSize;
    [SerializeField] Vector3 originalColliderLocalPosition;

    [SerializeField] GameManager_Script _gameManager;

    private void Start()
    {

        //SET COLLIDER
        minorObst_Collider = gameObject.transform.Find("MinorObs_Collider").GetComponent<BoxCollider>();
        originalColliderSize = minorObst_Collider.size;
        originalColliderLocalPosition = minorObst_Collider.transform.localPosition;

        //SET SPEED
        minorObst_rB = gameObject.GetComponent<Rigidbody>();
        minorObst_rB.velocity = transform.forward * speed;

        //SET GAMEMANAGER
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();


    }



    private void Update()
    {
        //TOP DIMENSION
        if(_gameManager.IsTopDimension)
        {
            //SET TOP DIMENSION SIZE
            minorObst_Collider.size = new Vector3(originalColliderSize.x, _gameManager.cageSize.y, originalColliderSize.z);

            //SET COLLIDER POSITION
            minorObst_Collider.transform.localPosition = new Vector3(originalColliderLocalPosition.x, -gameObject.transform.position.y, originalColliderLocalPosition.z);

        }
        else //NOT TOP DIMENSION
        {
            minorObst_Collider.size = new Vector3(_gameManager.cageSize.x, originalColliderSize.y, originalColliderSize.z);

            //SET COLLIDER POSITION
            minorObst_Collider.transform.localPosition = new Vector3(gameObject.transform.position.x, originalColliderLocalPosition.y, originalColliderLocalPosition.z);


        }


        //UI ADVISOR -> IsDangerousForPlayer
        if(_gameManager.playerInScene != null)
        {   
            //IF TOP DIMENSION
            if(_gameManager.IsTopDimension)
            {
                //IF ON PLAYER POSITION IN OTHER DIMENSION -> Como el obstáculo es más pequeño, tengo que comprobar una dimensión más, en este caso la y + la z original (del obstacle grande)
                //CALCULO si se solapan las posiciones, delante y detrás (teniendo en cuenta que está mirando a -Forward), teniendo en cuenta el tamaño de las hitboxes para que se reste bien, y que sea que coinciden las posiciones en dos de las dimensiones de la posición
                if ((((_gameManager.playerInScene.transform.position.z + _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.z / 2) >= transform.position.z - (originalColliderSize.z / 2)) &&   //front checking
                    ((_gameManager.playerInScene.transform.position.z - _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.z / 2) <= transform.position.z + (originalColliderSize.z / 2))) &&      
                (((_gameManager.playerInScene.transform.position.y + _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.y / 2) <= transform.position.y + (originalColliderSize.y / 2)) &&  
                    ((_gameManager.playerInScene.transform.position.y - _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.y / 2) >= transform.position.y - (originalColliderSize.y / 2)))) 
                {
                    IsDangerousForPlayer = true;
                    //Debug.Log("DANGER");
                }
                ////NOT IN PLAYER Z
                //CALCULO si se solapan las posiciones, delante y detrás (teniendo en cuenta que está mirando a -Forward), teniendo en cuenta el tamaño de las hitboxes para que se reste bien
                else if ((((_gameManager.playerInScene.transform.position.z + _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.z / 2) < transform.position.z - (originalColliderSize.z / 2))         //front checking
                    || ((_gameManager.playerInScene.transform.position.z - _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.z / 2) > transform.position.z + (originalColliderSize.z / 2))) ||
                    (((_gameManager.playerInScene.transform.position.y + _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.y / 2) > transform.position.y + (originalColliderSize.y / 2))         //front checking
                    || ((_gameManager.playerInScene.transform.position.y - _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.y / 2) < transform.position.y - (originalColliderSize.y / 2))))     //back checking  //PLAYER IS NEITHER IN FRONT OR BACK BOUNDARIES 
                {
                    IsDangerousForPlayer = false;
                    //Debug.Log("NO DANGER");
                }
               

            }
            //NOT TOP DIMENSION
            else
            {
                if ((((_gameManager.playerInScene.transform.position.z + _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.z / 2) >= transform.position.z - (originalColliderSize.z / 2)) &&   //front checking
                    ((_gameManager.playerInScene.transform.position.z - _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.z / 2) <= transform.position.z + (originalColliderSize.z / 2))) &&
                (((_gameManager.playerInScene.transform.position.x + _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.x / 2) <= transform.position.x + (originalColliderSize.x / 2)) &&
                    ((_gameManager.playerInScene.transform.position.x - _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.x / 2) >= transform.position.x - (originalColliderSize.x / 2))))                    
                {

                    IsDangerousForPlayer = true;
                    //Debug.Log("DANGER");
                }
                ////NOT IN PLAYER Z
                //CALCULO si se solapan las posiciones, delante y detrás (teniendo en cuenta que está mirando a -Forward), teniendo en cuenta el tamaño de las hitboxes para que se reste bien
                else if ((((_gameManager.playerInScene.transform.position.z + _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.z / 2) < transform.position.z - (originalColliderSize.z / 2))         //front checking
                    || ((_gameManager.playerInScene.transform.position.z - _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.z / 2) > transform.position.z + (originalColliderSize.z / 2))) ||
                    (((_gameManager.playerInScene.transform.position.x + _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.x / 2) > transform.position.x + (originalColliderSize.x / 2))         //front checking
                    || ((_gameManager.playerInScene.transform.position.x - _gameManager.playerInScene.gameObject.GetComponentInChildren<BoxCollider>().size.x / 2) < transform.position.x - (originalColliderSize.x / 2))))     //back checking  //PLAYER IS NEITHER IN FRONT OR BACK BOUNDARIES 
                {
                    IsDangerousForPlayer = false;
                    //Debug.Log("NO DANGER");
                }
            }

                        
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player_Controller>().KillPlayer();
        }
    }





}
