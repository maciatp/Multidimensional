using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fire_Script : MonoBehaviour
{
    [SerializeField] bool canShoot = true;
    public bool CanShoot
    {
        get { return canShoot; }
        set { canShoot = value; }
    }
    [SerializeField] bool isAlreadyShooting = false;
    [SerializeField] bool canShootAgain = false;

    [SerializeField] bool isSuperMode = false;

    public bool IsSuperMode
    {
        get { return isSuperMode; }
        set { isSuperMode = value; }
    }
    [SerializeField] bool canActivateSuperMode = false;


    [SerializeField] public bool isShootButtonPressed = false;


    
    [Header("Parameters")]
    //CHARGED SHOT
    //[SerializeField] float conteoChargingLaser = 0;
    //[SerializeField] float chargeLaserTimeSpan = 1f;

    //[SerializeField] bool isLaserCharging = false;

    //[SerializeField] int shootsToChargeLaser = 3; //-> cuántos disparos para cargar el laser?
    //[SerializeField] int currentShootsToChargeLaser = 0;
    //[SerializeField] bool isLaserCharged = false;

    //[SerializeField] bool isLaserChargedAndButtonUp = false;

    //[SerializeField] bool isChargedLaserInstanced = false;
    //[SerializeField] ChargedBullet_Script _chargedBulletInScene;
    //[SerializeField] GameObject chargedBullet;


    //[SerializeField] float conteoUseBeforeDeactivateChargedLaser = 0; //conteo que hace para cuando sueltas disparo con el láser cargado
    //[SerializeField] float UseBeforeDeactivateChargedLaserTimeSpan = 2.5f; //tiempo que tienes para volver a pulsar disparo antes de que se desactive disp cargado

    [SerializeField] float laserRafagaIntervalBetweenShots = 1f;
    public float originalLaserInterval = 1f;
    [SerializeField] float superModeIntervalMultiplier = 0.45f; // originalInterval * superModeIntervalMultiplier -> mantener por debajo de 1!
    [SerializeField] float shootCounter = 0; //usado para cuando mantienes pulsado
    [SerializeField] int powerUpsCollected = 0;

    public int PowerUpsCollected
    {
        get { return powerUpsCollected; }
        set { powerUpsCollected = value; }
    }
    [SerializeField] int currentPowerLevel = 1;
    public int CurrentPowerLevel
    {
        get { return currentPowerLevel; }
    }
    [SerializeField] int upgradesLevelsSpan = 7; //cada cuántos power ups subo de poder? (en todos baja el intervalo entre disparos, ésto es para multidisparo y demás)
    public int UpgradesLevelsSpan
    {
        get { return upgradesLevelsSpan; }
    }
    [SerializeField] int maxPowerLevel = 5; // 5 niveles incluyendo el superMode. Ésta es la variable a partir de la cual se contruyen las demás 
    public int MaxPowerLevelIs
    {
        get { return maxPowerLevel; }
    }
    //[SerializeField] int totalPowerUpsToMAXCharge; //maxpowerlevel * upgradeslevelSpan // QUE SEA UN NUMERO ENTERO -> ENCONTRAR CÓMO PASAR A FLOAT (no he encontrado método a priori)
    [SerializeField] float angleBetweenBullets = 25f;
    [SerializeField] float superModeCounter = 0;
    public float SuperModeCounterIs
    {
        get { return superModeCounter; }
    }
    [SerializeField] float superModeTimeSpan = 5f;
    public float SuperModeTimeSpanIs
    {
        get { return superModeTimeSpan; }
    }

    [SerializeField] Transform fire_Transform = null;
    [SerializeField] Transform fireDualLeft_Transform = null;
    [SerializeField] Transform fireDualRight_Transform = null;

    [SerializeField] Transform fireDualUP_Transform = null;
    [SerializeField] Transform fireDualDOWN_Transform = null;


    [SerializeField] GameObject bullet = null;


    [SerializeField] GameManager_Script _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
        PowerUpManager_Script _powerUpsManager = _gameManager.transform.parent.Find("PowerUpManager").GetComponent<PowerUpManager_Script>();

        //testing
        //totalPowerUpsToMAXCharge = maxPowerLevel * upgradesLevelsSpan;
        //originalLaserInterval = laserRafagaIntervalBetweenShots;


        //LOADING DATA
        currentPowerLevel = PlayerPrefs.GetInt("currentPowerLevel", 1);
        laserRafagaIntervalBetweenShots = PlayerPrefs.GetFloat("laserRafagaIntervalBetweenShots", laserRafagaIntervalBetweenShots);
        //powerUpsCollected = PlayerPrefs.GetInt("powerUpsCollected", 0);
        //int canActivateSuperMode_BOOL = PlayerPrefs.GetInt("canActivateSuperMode_BOOL", 0);
        //if (canActivateSuperMode_BOOL == 1)
        //{
        //    canActivateSuperMode = true;
        //}
        //else
        //{
        //    canActivateSuperMode = false;
        //}



       


        //los transforms de posiciones de disparo
        fire_Transform = gameObject.transform.Find("Fire_Position").gameObject.transform;
        fireDualRight_Transform = fire_Transform.GetChild(0);
        fireDualLeft_Transform = fire_Transform.GetChild(1);
        fireDualUP_Transform = fire_Transform.GetChild(2);
        fireDualDOWN_Transform = fire_Transform.GetChild(3);

        //1 + tal - tal, para que al iniciar, laserRafagaInterval sea SIEMPRE 1. -> COMENTAR PARA TESTEAR CON DIFERENTES TIEMPOS -> ª!!!!!! DESCOMENTAR CUANDO TERMINE PRUEBAS DE DISPARO
        laserRafagaIntervalBetweenShots = (laserRafagaIntervalBetweenShots 
            + ((float)_powerUpsManager.powerUps[0].gameObject.GetComponent<PowerUp_Script>().PowerUpPower / (float)upgradesLevelsSpan)) 
            - (((float)_powerUpsManager.powerUps[0].gameObject.GetComponent<PowerUp_Script>().PowerUpPower / (float)upgradesLevelsSpan));

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(laserRafagaIntervalBetweenShots);


        //USING COROUTINE
        //if (isShootButtonPressed && canShoot) //sólo tengo en cuenta canShoot para que dispare cuando pulse el botón aunque esté waitforSeconds contando. -> Da error. Entra en un bucle que no puedo controlar. por ahora lo hago con un counter.
        //{
        //    //Debug.Log(" SHOOTING UPDATE");
        //    StartCoroutine(FireCoroutine());
        //}


        //USING COUNTER TO CONTROL SHOOTING
        if (isShootButtonPressed && !IsSuperMode)// && !isLaserCharged) //-> CUANDO DISPARo chargedBullet, desactivo isLaserCharged, por lo que entra aquí al siguiente frame, por eso aparecen dos. añadir un timer?
        {
            if (shootCounter == 0)
            {
                Fire();

            }

            shootCounter += Time.unscaledDeltaTime;

            if (shootCounter >= laserRafagaIntervalBetweenShots)
            {
                shootCounter = 0;
            }


        }
        else if (!isShootButtonPressed && !IsSuperMode)
        {
            shootCounter = 0;
        }

        //IF SHOOT BUTTON PRESSED AND ISLASERCHARGED ->> SHOOT!



        //IN SUPER MODE, FIRE AUTOMATICALLY
        if (IsSuperMode)
        {

            if (shootCounter == 0)
            {
                Fire();
            }
            shootCounter += Time.unscaledDeltaTime;

            if (shootCounter >= originalLaserInterval / superModeIntervalMultiplier) //modificar el float para que aparezcan más o menos seguido
            {
                shootCounter = 0;
            }


            superModeCounter += Time.deltaTime;
            if (superModeCounter >= superModeTimeSpan)
            {
                DeactivateSuperMode();

            }
        }


        ////CHARGING LASER -> Cargo el laser según el número de disparos(3), no tiempo
        //if (isShootButtonPressed && !isLaserCharged)
        //{
        //    conteoChargingLaser += Time.deltaTime;
        //    if (conteoChargingLaser >= chargeLaserTimeSpan)
        //    {
        //        isLaserCharged = true;
        //        conteoChargingLaser = 0;
        //    }
        //}


        ////LASER CHARGED TIMER DEACTIVATION
        //if (isLaserCharged && !isShootButtonPressed)
        //{
        //    conteoUseBeforeDeactivateChargedLaser += Time.deltaTime;
        //    if (conteoUseBeforeDeactivateChargedLaser >= UseBeforeDeactivateChargedLaserTimeSpan)
        //    {
        //        //isLaserCharged = false;
        //        //conteoChargingLaser = 0;
        //        //currentShootsToChargeLaser = 0;
        //        DeactivateChargedLaser();
        //    }
        //}

        ////CHARGED LASER DEACTIVATION
        //if (isLaserChargedAndButtonUp == true)
        //{
        //    conteoUseBeforeDeactivateChargedLaser += Time.deltaTime;
        //}

        //if (conteoUseBeforeDeactivateChargedLaser > UseBeforeDeactivateChargedLaserTimeSpan)
        //{
        //    DeactivateChargedLaser();
        //    conteoUseBeforeDeactivateChargedLaser = 0;
        //}




    }



    public void Fire()
    {


        //IF TOPDIMENSION
        if (_gameManager.IsTopDimension)
        {

            //CHECK POWERUPS COLLECTED, if powerupscollected is less than the first upgradeLaserSpan Level
            // 1 STRAIGHT
            if (currentPowerLevel == 1)  //(powerUpsCollected < (upgradesLevelsSpan * currentPowerLevel))
            {
                //INSTANTIATE
                GameObject _bullet = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bullet.name = "Bullet";
            }
            //CHECK POWERUPS COLLECTED, if % 5 (*1)

            // 2 SHOTS STRAIGHT
            else if (currentPowerLevel == 2)  // (powerUpsCollected <= (upgradesLevelsSpan * currentPowerLevel))
            {
                //INSTANTIATE RIGHT
                GameObject _bulletRIGHT = Instantiate(bullet, fireDualRight_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT.name = "Bullet_RIGHT";

                //INSTANTIATE LEFT
                GameObject _bulletLEFT = Instantiate(bullet, fireDualLeft_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT.name = "Bullet_LEFT";

            }

            //CHECK POWERUPS COLLECTED, if % 5 (*2)...
            // 3 SHOTS; 2 ANGLED, 1 STRAIGHT
            else if (currentPowerLevel == 3)  //  (powerUpsCollected > (upgradesLevelsSpan * currentPowerLevel))
            {

                //INSTANTIATE CENTER
                GameObject _bulletCenter = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletCenter.name = "Bullet_CENTER";


                //INSTANTIATE RIGHT
                GameObject _bulletRIGHT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT.name = "Bullet_RIGHT";
                _bulletRIGHT.transform.Rotate(transform.up, gameObject.transform.rotation.y + (angleBetweenBullets));

                //INSTANTIATE LEFT
                GameObject _bulletLEFT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT.name = "Bullet_LEFT";
                _bulletLEFT.transform.Rotate(transform.up, gameObject.transform.rotation.y - (angleBetweenBullets));


            }

            //5 shots
            else if (currentPowerLevel == 4)  //  (powerUpsCollected > (upgradesLevelsSpan * currentPowerLevel))
            {

                //INSTANTIATE CENTER
                GameObject _bulletCenter = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletCenter.name = "Bullet_CENTER";


                //INSTANTIATE RIGHT
                GameObject _bulletRIGHT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT.name = "Bullet_RIGHT";
                _bulletRIGHT.transform.Rotate(transform.up, gameObject.transform.rotation.y + (angleBetweenBullets * 0.75f));

                //INSTANTIATE RIGHT 2
                GameObject _bulletRIGHT2 = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT2.name = "Bullet_RIGHT2";
                _bulletRIGHT2.transform.Rotate(transform.up, gameObject.transform.rotation.y + (angleBetweenBullets * 1.5f));



                //INSTANTIATE LEFT
                GameObject _bulletLEFT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT.name = "Bullet_LEFT";
                _bulletLEFT.transform.Rotate(transform.up, gameObject.transform.rotation.y - (angleBetweenBullets * 0.75f));

                //INSTANTIATE LEFT
                GameObject _bulletLEFT2 = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT2.name = "Bullet_LEFT2";
                _bulletLEFT2.transform.Rotate(transform.up, gameObject.transform.rotation.y - (angleBetweenBullets * 1.5f));


            }

            //MAX POWER LEVEL NOT SUPER MODE
            else if (!isSuperMode && currentPowerLevel == maxPowerLevel)  //  (powerUpsCollected > (upgradesLevelsSpan * currentPowerLevel))
            {
                //INSTANTIATE CENTER
                GameObject _bulletCenter = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletCenter.name = "Bullet_CENTER";


                //INSTANTIATE RIGHT
                GameObject _bulletRIGHT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT.name = "Bullet_RIGHT";
                _bulletRIGHT.transform.Rotate(transform.up, gameObject.transform.rotation.y + (angleBetweenBullets * 0.75f));

                //INSTANTIATE RIGHT 2
                GameObject _bulletRIGHT2 = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT2.name = "Bullet_RIGHT2";
                _bulletRIGHT2.transform.Rotate(transform.up, gameObject.transform.rotation.y + (angleBetweenBullets * 1.5f));



                //INSTANTIATE LEFT
                GameObject _bulletLEFT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT.name = "Bullet_LEFT";
                _bulletLEFT.transform.Rotate(transform.up, gameObject.transform.rotation.y - (angleBetweenBullets * 0.75f));

                //INSTANTIATE LEFT
                GameObject _bulletLEFT2 = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT2.name = "Bullet_LEFT2";
                _bulletLEFT2.transform.Rotate(transform.up, gameObject.transform.rotation.y - (angleBetweenBullets * 1.5f));

            }


            //SUPER MODE
            else if (isSuperMode && currentPowerLevel == maxPowerLevel)
            {

                for (float i = 0; i < 360; i += angleBetweenBullets * 0.75f)
                {
                    GameObject _bulletSuper = Instantiate(bullet, gameObject.transform.position, gameObject.transform.rotation, null);
                    _bulletSuper.name = "Bullet_SUPER";
                    _bulletSuper.transform.Rotate(transform.up, gameObject.transform.rotation.y + (i));
                }


            }
        }
        //IF RIGHT DIMENSION
        else
        {
            //CHECK POWERUPS COLLECTED, if powerupscollected is less than the first upgradeLaserSpan Level
            // 1 STRAIGHT
            if (currentPowerLevel == 1)  //(powerUpsCollected < (upgradesLevelsSpan * currentPowerLevel))
            {
                //INSTANTIATE
                GameObject _bullet = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bullet.name = "Bullet";
            }
            //CHECK POWERUPS COLLECTED, if % 5 (*1)

            // 2 SHOTS STRAIGHT
            else if (currentPowerLevel == 2)  // (powerUpsCollected <= (upgradesLevelsSpan * currentPowerLevel))
            {
                //INSTANTIATE RIGHT
                GameObject _bulletRIGHT = Instantiate(bullet, fireDualUP_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT.name = "Bullet_RIGHT";

                //INSTANTIATE LEFT
                GameObject _bulletLEFT = Instantiate(bullet, fireDualDOWN_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT.name = "Bullet_LEFT";

            }

            //CHECK POWERUPS COLLECTED, if % 5 (*2)...
            // 3 SHOTS; 2 ANGLED, 1 STRAIGHT
            else if (currentPowerLevel == 3)  //  (powerUpsCollected > (upgradesLevelsSpan * currentPowerLevel))
            {

                //INSTANTIATE CENTER
                GameObject _bulletCenter = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletCenter.name = "Bullet_CENTER";


                //INSTANTIATE RIGHT
                GameObject _bulletRIGHT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT.name = "Bullet_RIGHT";
                _bulletRIGHT.transform.Rotate(transform.right, gameObject.transform.rotation.x + (angleBetweenBullets));

                //INSTANTIATE LEFT
                GameObject _bulletLEFT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT.name = "Bullet_LEFT";
                _bulletLEFT.transform.Rotate(transform.right, gameObject.transform.rotation.x - (angleBetweenBullets));


            }

            //5 shots
            else if (currentPowerLevel == 4)  //  (powerUpsCollected > (upgradesLevelsSpan * currentPowerLevel))
            {

                //INSTANTIATE CENTER
                GameObject _bulletCenter = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletCenter.name = "Bullet_CENTER";


                //INSTANTIATE RIGHT
                GameObject _bulletRIGHT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT.name = "Bullet_RIGHT";
                _bulletRIGHT.transform.Rotate(transform.right, gameObject.transform.rotation.x + (angleBetweenBullets * 0.75f));

                //INSTANTIATE RIGHT 2
                GameObject _bulletRIGHT2 = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT2.name = "Bullet_RIGHT2";
                _bulletRIGHT2.transform.Rotate(transform.right, gameObject.transform.rotation.x + (angleBetweenBullets * 1.5f));



                //INSTANTIATE LEFT
                GameObject _bulletLEFT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT.name = "Bullet_LEFT";
                _bulletLEFT.transform.Rotate(transform.right, gameObject.transform.rotation.x - (angleBetweenBullets * 0.75f));

                //INSTANTIATE LEFT
                GameObject _bulletLEFT2 = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT2.name = "Bullet_LEFT2";
                _bulletLEFT2.transform.Rotate(transform.right, gameObject.transform.rotation.x - (angleBetweenBullets * 1.5f));


            }

            //5Shots MAX POWER But not SuperMode
            else if (!isSuperMode && currentPowerLevel == maxPowerLevel)  //  (powerUpsCollected > (upgradesLevelsSpan * currentPowerLevel))
            {

                //INSTANTIATE CENTER
                GameObject _bulletCenter = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletCenter.name = "Bullet_CENTER";


                //INSTANTIATE RIGHT
                GameObject _bulletRIGHT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT.name = "Bullet_RIGHT";
                _bulletRIGHT.transform.Rotate(transform.right, gameObject.transform.rotation.x + (angleBetweenBullets * 0.75f));

                //INSTANTIATE RIGHT 2
                GameObject _bulletRIGHT2 = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletRIGHT2.name = "Bullet_RIGHT2";
                _bulletRIGHT2.transform.Rotate(transform.right, gameObject.transform.rotation.x + (angleBetweenBullets * 1.5f));



                //INSTANTIATE LEFT
                GameObject _bulletLEFT = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT.name = "Bullet_LEFT";
                _bulletLEFT.transform.Rotate(transform.right, gameObject.transform.rotation.x - (angleBetweenBullets * 0.75f));

                //INSTANTIATE LEFT
                GameObject _bulletLEFT2 = Instantiate(bullet, fire_Transform.position, gameObject.transform.rotation, null);
                _bulletLEFT2.name = "Bullet_LEFT2";
                _bulletLEFT2.transform.Rotate(transform.right, gameObject.transform.rotation.x - (angleBetweenBullets * 1.5f));


            }



            //SUPER MODE
            else if (isSuperMode && currentPowerLevel == maxPowerLevel)
            {

                for (float i = 0; i < 360; i += angleBetweenBullets * 0.75f)
                {
                    GameObject _bulletSuper = Instantiate(bullet, gameObject.transform.position, gameObject.transform.rotation, null);
                    _bulletSuper.name = "Bullet_SUPER";
                    _bulletSuper.transform.Rotate(transform.right, gameObject.transform.rotation.x + (i));
                }


            }
        }


        ////CHARGE LASER WHILE SHOOTING
        //currentShootsToChargeLaser++;
        //if(currentShootsToChargeLaser >= shootsToChargeLaser)
        //{
        //    isLaserCharged = true;
        //    DeactivateConteoChargeLaser();
        //    InstantiateChargedBullet();
        //}

        //HACER QUE EN SUPER MODE DISPARE EN 3D ??

    }




    //NO LA USO PQ ENTRABA EN UN BUCLE NO DESEADO. NO puedo cancelar un waitforseconds al levantar el botón?? + CHARGED LASER MAS ABAJO

    //public IEnumerator FireCoroutine()
    //{
    //    //if (laserUpgradesCaught == 0)
    //    {
    //        //for (int lasersShot = 0; lasersShot < 30; lasersShot += 1)// DESCOMENTAR CUANDO HAYAN TERMINADO PRUEBAS DE DISPARO
    //        {
    //            canShoot = false;

    //            if (isShootButtonPressed && canShootAgain)
    //            {
    //                // Debug.Log("DISPARO COROUTINE");

    //                GameObject _bullet = Instantiate(bullet, fire_Position.position, gameObject.transform.rotation, null);
    //                _bullet.name = "Bullet";// SÓLO FUNCIONA SI SE LLAMA EXACTAMENTE IGUAL (CON CLONE INCLUIDO), por eso lo renombro

    //                canShootAgain = false;

    //                //A PARTIR DE AQUI IBA DESPUÉS DEL ELSE
    //                yield return new WaitForSecondsRealtime(laserRafagaIntervalBetweenShots);// DESCOMENTAR CUANDO HAYAN TERMINADO PRUEBAS DE DISPARO

    //                canShoot = true;
    //                canShootAgain = true;
    //                //isAlreadyShooting = false;
    //            }


    //        }




    //    }

    //}





    //CHARGED LASER
        
    
    //void BeginChargeLaser()
    //{
    //    isLaserCharging = true;
    //    //chargingLaserFX_INSCENE = (GameObject)Instantiate(chargingLaserFX, chargedLaser_Spawn.position, chargedLaser_Spawn.rotation, this.transform);
    //}
    //void DeactivateConteoChargeLaser()
    //{
    //    isLaserCharging = false;
    //    //conteoChargingLaser = 0;
    //    currentShootsToChargeLaser = 0;


    //}
    //void InstantiateChargedBullet()
    //{
    //    GameObject _chargedBullet = GameObject.Instantiate(chargedBullet, fire_Transform.Find("Charged_SpawnPosition").gameObject.transform.position, gameObject.transform.rotation, fire_Transform);

    //    isChargedLaserInstanced = true;

    //    _chargedBulletInScene = _chargedBullet.GetComponent<ChargedBullet_Script>();

    //}

    //void ShootChargedLaser()
    //{

    //    _chargedBulletInScene.ShootChargedBullet();// activo los colliders, el rigidbody.velocity, etc.
    //    isLaserChargedAndButtonUp = false;
    //    conteoUseBeforeDeactivateChargedLaser = 0; //reinicio el counter de desactivación

    //    //estos dos provocan que dispare bullet normal al disparar la cargada
    //    isLaserCharged = false;
    //    currentShootsToChargeLaser = 0;
    //    isChargedLaserInstanced = false;
    //    shootCounter = 0.01f;
    //    //StartCoroutine(DeactivateIsLaserChargedCoroutine());

    //    //Debug.Log("He DisparadoCargado");

    //    //Destroy(chargingLaserFX_INSCENE.gameObject);
    //}

    //IEnumerator DeactivateIsLaserChargedCoroutine()
    //{
    //    yield return new WaitForSecondsRealtime(laserRafagaIntervalBetweenShots);
    //    isLaserCharged = false;
    //    currentShootsToChargeLaser = 0;

    //}

    //void BeginChargedLaserConteoDeactivation() //-> When button released and laser is charged
    //{
    //    isLaserChargedAndButtonUp = true;
    //}
    //void DeactivateChargedLaser()
    //{
    //    if(_chargedBulletInScene != null)
    //    {

    //    _chargedBulletInScene.DestroyChargedBullet();// -> para cuando tenga el prefab?
    //    }


    //    isLaserCharged = false;
    //    isChargedLaserInstanced = false;
    //    isLaserChargedAndButtonUp = false;
    //    currentShootsToChargeLaser = 0;
    //    DeactivateConteoChargeLaser();
    //}
    


    public void AddPower(int powerWillAdd)
    {
        //Debug.Log("AÑADO POWER");
        float powerIncrease = (float)powerWillAdd / ((float)upgradesLevelsSpan*2f) ; // el 2 es para que vaya restando numeros pequeños. a mayor numero donde el 2, menos resta del rafaga interval
        //Debug.Log(powerIncrease);
        if(laserRafagaIntervalBetweenShots >= 0.15f)
        {
            laserRafagaIntervalBetweenShots -= powerIncrease;

        }
        

        if(!IsSuperMode)
        {
            powerUpsCollected++;
            if (powerUpsCollected >= upgradesLevelsSpan)
            {
                currentPowerLevel++;

                if (currentPowerLevel < maxPowerLevel)
                {
                    powerUpsCollected = 0;

                    if (currentPowerLevel != maxPowerLevel)
                    {
                        //laserRafagaIntervalBetweenShots = originalLaserInterval - (currentPowerLevel/20);

                    }

                }
                else
                {
                    currentPowerLevel = maxPowerLevel;
                    canActivateSuperMode = true;

                }


            }
        }

        //SAVING
        PlayerPrefs.SetFloat("laserRafagaIntervalBetweenShots", laserRafagaIntervalBetweenShots);
        PlayerPrefs.SetInt("currentPowerLevel", currentPowerLevel);
        PlayerPrefs.SetInt("powerUpsCollected", powerUpsCollected);
        PlayerPrefs.SetInt("canActivateSuperMode_BOOL", 1);

     

    }

    public void ActivateSuperMode()
    {
        if (canActivateSuperMode && currentPowerLevel == maxPowerLevel)
        {
            IsSuperMode = true;
            canActivateSuperMode = false;
            powerUpsCollected = 0;
            PlayerPrefs.SetInt("powerUpsCollected", powerUpsCollected);

        }
    }

    private void DeactivateSuperMode()
    {
        IsSuperMode = false;
        superModeCounter = 0;
        currentPowerLevel = 4;
        PlayerPrefs.SetInt("currentPowerLevel", currentPowerLevel);
    }

    public void WhenPlayerKilled()
    {
        if(currentPowerLevel > 4)
        {
            currentPowerLevel = 4;
            powerUpsCollected = 0;
            canActivateSuperMode = false;
            PlayerPrefs.SetInt("canActivateSuperMode_BOOL", 0);
            PlayerPrefs.SetInt("currentPowerLevel", currentPowerLevel);
            PlayerPrefs.SetInt("powerUpsCollected", powerUpsCollected);

        }
    }



    //INPUT (CALLED ONCE)
    public void ShootButtonPressed()
    {
       // Debug.Log("esto debería salir 1 vez");
        isShootButtonPressed = true;


        //CHARGED SHOOT
        //if (isLaserCharged == false)
        //{
        //    isShootButtonPressed = true;
        //    BeginChargeLaser();
        //    //StartCoroutine(Fire); -> uso el timer en el update; esto venía de StarFoxCV
        //}
        //if (isLaserCharged == true)
        //{
        //    if ((isLaserChargedAndButtonUp == true) && (!_chargedBulletInScene.IsShot)) //DISPARO. cuando tenga el prefab del disparo cargado?
        //    {

        //        ShootChargedLaser();
        //        isShootButtonPressed = true;
        //       // BeginChargeLaser(); creo que aquí está el por qué se instancian varios Fx al disparar.comentar para probar

        //    }
        //}

        //StartCoroutine(FireCoroutine());
    }
    public void ShootButtonUnpressed()
    {
        isShootButtonPressed = false;



        //if (isLaserCharged == false)
        //{
        //    isShootButtonPressed = false;
        //    DeactivateConteoChargeLaser();
        //    //shootCounter = 0; -> ya lo hago en DeactivateConteoChargedLaser

        //    //Destroy(chargingLaserFX_INSCENE.gameObject);
        //}
        //else
        //{
        //    isShootButtonPressed = false;
        //    BeginChargedLaserConteoDeactivation();
        //    //Destroy(chargingLaserFX_INSCENE.gameObject);
        //}



        //StopCoroutine(FireCoroutine());
        //canShoot = true;
        //canShootAgain = true;


    }
}
