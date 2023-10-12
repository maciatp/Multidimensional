using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class PowerUp_Script : MonoBehaviour
{
    [SerializeField] bool givesPower;
    [SerializeField] bool givesSpeed;
    [SerializeField] bool givesFasterCharge;
    [SerializeField] bool givesLife;


    [SerializeField] BoxCollider powerUpCollider;
    [SerializeField] Vector3 originalColliderSize;
    [SerializeField] Vector3 originalColliderLocalPosition;
    [SerializeField] int powerUpPower = 1; // time between rafagas gets reduced powerUpPower/12 when collected!
    [SerializeField] float speedPower = 1; // 
    [SerializeField] float chargePower = 1; // 

    [SerializeField] Material isCollectable_Material;
    [SerializeField] Material isUNCollectable_Material;
    [SerializeField] MeshRenderer _meshRenderer;

    public int PowerUpPower
    {
        get { return powerUpPower; }
        set { powerUpPower = value; }
    }

    [SerializeField] bool isTopPowerUp = true;
    [SerializeField] bool isMultiDimensionalPowerUp = false; //ACTIVARLO EN EL EDITOR! -> para el powerUp que sea para todas las dimensiones

    [SerializeField] GameManager_Script _gameManager;
    



    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
        
        powerUpCollider = transform.Find("Collider").GetComponent<BoxCollider>();
        originalColliderSize = powerUpCollider.size;
        originalColliderLocalPosition = powerUpCollider.transform.localPosition;

        _meshRenderer = gameObject.transform.GetChild(0).Find("Fbx").GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //IS TOP POWER UP CHECKING
        float worldAngle = Vector3.Angle(transform.right, Vector3.down);
        if (worldAngle < 180)
        {
            isTopPowerUp = true;
        }
        else
        {
            isTopPowerUp = false;
        }


       // Debug.Log(worldAngle);

        //MULTIDIMENSIONAL POWERUP
        if (isMultiDimensionalPowerUp)
        {
            //IS TOP DIMENSION  MULTIDIMENSIONAL
            if (_gameManager.IsTopDimension)
            {
                //SET COLLIDER SIZE
                powerUpCollider.size = new Vector3(originalColliderSize.x, _gameManager.cageSize.y, originalColliderSize.z);


                //SET COLLIDER POSITION

                powerUpCollider.transform.localPosition = new Vector3(originalColliderLocalPosition.x, -gameObject.transform.position.y, powerUpCollider.transform.localPosition.z);



            }
            else //IS RIGHT DIMENSION MULTIDIMENSIONAL
            {
                //SET COLLIDER SIZE
                powerUpCollider.size = new Vector3(_gameManager.cageSize.x, originalColliderSize.y, originalColliderSize.z);

                //SET COLLIDER POSITION

                powerUpCollider.transform.localPosition = new Vector3(-gameObject.transform.position.x, originalColliderLocalPosition.y, powerUpCollider.transform.localPosition.z);
            }
        }
        else//MONODIMENSIONAL POWERUP
        {
            //TOP MATCHING
            if(_gameManager.IsTopDimension && isTopPowerUp)
            {

                if(!powerUpCollider.enabled)
                {
                    powerUpCollider.enabled = true;
                }
                //SET COLLIDER SIZE
                powerUpCollider.size = new Vector3(originalColliderSize.x, _gameManager.cageSize.y, originalColliderSize.z);


                //SET COLLIDER POSITION

                powerUpCollider.transform.localPosition = new Vector3(originalColliderLocalPosition.x, -gameObject.transform.position.y, powerUpCollider.transform.localPosition.z);


                //SET COLLECTABLE MATERIAL
                if(_meshRenderer.material != isCollectable_Material)
                {
                    _meshRenderer.material = isCollectable_Material;
                }

            }
            //TOP UNMATCHING
            else if(!_gameManager.IsTopDimension && isTopPowerUp)
            {

                if(powerUpCollider.enabled)
                {
                    powerUpCollider.enabled = false;
                }

                ////SET COLLIDER SIZE
                //powerUpCollider.size = new Vector3(originalColliderSize.x, originalColliderSize.y, originalColliderSize.z);

                ////SET COLLIDER POSITION
                //powerUpCollider.transform.localPosition = originalColliderLocalPosition;

                //SET UNCOLLECTABLE MATERIAL
                if (_meshRenderer.material != isUNCollectable_Material)
                {
                    _meshRenderer.material = isUNCollectable_Material;
                }

            }



            //RIGHT MATCHING
            else if(!_gameManager.IsTopDimension && !isTopPowerUp)
            {
                if(!powerUpCollider.enabled)
                {
                    powerUpCollider.enabled = true;
                }
                //Set Collider SIZE
                powerUpCollider.size = new Vector3(originalColliderSize.y, _gameManager.cageSize.x, originalColliderSize.z);


                //SET COLLIDER POSITION
                powerUpCollider.transform.localPosition = new Vector3(originalColliderLocalPosition.x, gameObject.transform.position.x, originalColliderLocalPosition.z);

                //SET COLLECTABLE MATERIAL
                if (_meshRenderer.material != isCollectable_Material)
                {
                    _meshRenderer.material = isCollectable_Material;
                }

            }


            //RIGHT UNMATCHING
            else if(_gameManager.IsTopDimension && !isTopPowerUp)
            {
                if(powerUpCollider.enabled)
                {
                    powerUpCollider.enabled = false;
                }

                ////SET COLLIDER SIZE
                //powerUpCollider.size = new Vector3(originalColliderSize.y, originalColliderSize.x, originalColliderSize.z);

                ////SET COLLIDER POSITION
                //powerUpCollider.transform.localPosition = originalColliderLocalPosition;

                //SET UNCOLLECTABLE MATERIAL
                if (_meshRenderer.material != isUNCollectable_Material)
                {
                    _meshRenderer.material = isUNCollectable_Material;
                }

            }

        }


        
    }

    public void SetCollectableMaterial()
    {
        _meshRenderer.material = isCollectable_Material;
    }
    public void SetUNcollectableMaterial()
    {
        _meshRenderer.material = isUNCollectable_Material;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(givesPower)
            {
                //ADD POWER
                other.GetComponent<Fire_Script>().AddPower(powerUpPower);
            }
            else if(givesSpeed)
            {
                other.GetComponent<Player_Controller>().AddSpeed(speedPower);
            }
            else if(givesFasterCharge)
            {
                other.GetComponent<Player_Controller>().IncreaseChargePower(chargePower);
            }
            else if(givesLife)
            {
                _gameManager.AddLife();
            }
            //DESTROY GAMEOBJECT
            Destroy(gameObject);
        }
    }
}
