using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{

    public PlayerControls inputActions;
    [Header("GAMEPAD CONTROLS")]
    [SerializeField] bool isUsingGamepad = false; //(kb+mouse if unchecked)
    Gamepad gamepad;

    [SerializeField] bool isDashButtonPressed = false;
    [SerializeField] bool canDash = false;

    [SerializeField] bool isDashing = false;
    [SerializeField] float dashVelocityMultiplier = 1.5f;

    [SerializeField] float dashCounter = 0;
    public float DashCounter
    {
        get { return dashCounter; }
    }

    [SerializeField] float dashTimeSpan = 2.5f;

    public float DashTimeSpan
    {
        get { return dashTimeSpan; }
    }

    [SerializeField] float dashChargeRate = 0.2f;

    [Header("TOUCH CONTROLS")]
    //TOUCH JOYSTICK
    [SerializeField] bool isUsingTouchScreen = false;
    [SerializeField] public Joystick joystickMove;
    public Joystick joystickAim;
    //COGER joystick.Horizontal y Vertical y pasarlos a moveInput CUANDO EL DISPOSITIVO SEA TOUCH, en el UPDATE
   
    [Header("PARAMETERS")]
    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] Vector2 aimDirection;
    [SerializeField] float currentMovementSpeed; // = 12;
    [SerializeField] public float ORIGINAL_MOVEMENT_SPEED = 10;
    [SerializeField] float maximumMovementSpeed = 20;
    [SerializeField] GameObject cage;

   

    public bool isInvulnerable = false;
    [SerializeField] float invulnerableCounter = 0;
    [SerializeField] float invulnerableTimeSpan = 2f;

    
    [SerializeField] float chargeCounter = 0;
    public float ChargeCounter
    {
        get { return chargeCounter; }
    }
    [SerializeField] float charge_TimeSpan  = 1.5f;
    public float Charge_TimeSpan
    {
        get { return charge_TimeSpan; }
    }

    [SerializeField] float charge_Cost  = 1f; // how much dimensional charge changing Dimension diminishes

    [SerializeField] float chargeMultiplier = 1; // How fast dimensional charge charges -> when powerUp givesFasterCharge, add 0.1 to this.
    float chargeMultiplierMaximum = 1.75f;

    [SerializeField] bool isCharged = false;

    


    [Header("Instances")]
    
    Camera mainCamera;
    Rigidbody player_Rb;
    [SerializeField] MeshRenderer _meshRenderer;

    [SerializeField] Animator cameraOrbit_Animator = null;

    [SerializeField] GameManager_Script _gameManager_Script;

    [SerializeField] Fire_Script _fire_Script;

    [SerializeField] UI_ChargeRing_Script _UI_ChargeRing_Script;


    private void Awake()
    {
        inputActions = new PlayerControls();

        _gameManager_Script = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();

        mainCamera = Camera.main;
    }


    // Start is called before the first frame update
    void Start()
    {

        currentMovementSpeed = PlayerPrefs.GetFloat("currentMovementSpeed", ORIGINAL_MOVEMENT_SPEED);

        if(GameObject.Find("UI_Touch") != null)
        {
            isUsingTouchScreen = true;
            isUsingGamepad = false;
            
        }

        if(isUsingTouchScreen)
        { 
            //USO GETCHILD, DEBERÍA CAMBIARLO
            joystickMove = GameObject.Find("UI_Touch").transform.GetChild(0).GetComponent<Joystick>();
            joystickAim = GameObject.Find("UI_Touch").transform.GetChild(1).GetComponent<Joystick>();
        }

        //GAMEPAD
        gamepad = Gamepad.current;

        //Debug.Log(gamepad);


        if (gamepad == null)
        {
            // Debug.Log("Juego con teclado");
            isUsingGamepad = false;
        }
        else
        {
            isUsingGamepad = true;

            // Debug.Log("uso mando");
        }



        //desde aquí estaba en el awake
        
        //Para que empiece cargado de DAsh
       // dashCounter = dashTimeSpan;


        player_Rb = gameObject.GetComponent<Rigidbody>();

        _fire_Script = gameObject.GetComponent<Fire_Script>();

        cameraOrbit_Animator = GameObject.FindGameObjectWithTag("CameraOrbit").GetComponent<Animator>();
        cameraOrbit_Animator.enabled = false;


        //uso el meshrenderer para limitar las boundaries. Así cuando le pase el modelo 3D no hará falta cambiar nada
        _meshRenderer = gameObject.transform.GetComponentInChildren<MeshRenderer>();

        //HASTA AQUÍ estaba en el awake
       


        cage = GameObject.FindGameObjectWithTag("Cage").gameObject;


        _UI_ChargeRing_Script = GameObject.Find("UI_ChargeRing").gameObject.GetComponent<UI_ChargeRing_Script>();


        isInvulnerable = true;

        

    }

    // Update is called once per frame
    void Update()
    {
        //TOUCH CONTROLS
        if(isUsingTouchScreen)
        {
            //MOVE
            moveInput.x = joystickMove.Horizontal;
            moveInput.y = joystickMove.Vertical;

            //AIM
            aimDirection.x = joystickAim.Horizontal;
            aimDirection.y = joystickAim.Vertical;

            Aim(); //HAY OTRA EN OnAim, podría quitarla y mover esta fuera del if?????

        }


        //INPUT TOP DIMENSION
        if(_gameManager_Script.IsTopDimension)
        {
            moveDirection.x = moveInput.x;
            moveDirection.z = moveInput.y;
            moveDirection.y = 0;
        }
        //INPUT RIGHT DIMENSION
        else
        {
            moveDirection.y = moveInput.y;
            moveDirection.z = moveInput.x;
            moveDirection.x = 0;
        }
        //IF CONTROLLER GETS UNPLUGGED DURING PLAY
        if (Gamepad.current == null)
        {
            // Debug.Log("Juego con teclado");
            isUsingGamepad = false;
            gamepad = null;
        }
        //IF CONTROLLER GETS PLUGGED DURING PLAY
        else if (Gamepad.current != null ) 
        {
            isUsingGamepad = true;
            gamepad = Gamepad.current;
            // Debug.Log("uso mando");
        }



        //IF GAME NOT PAUSED
        if(!_gameManager_Script.IsGamePaused)
        {
            //MOUSE POINTING
            //PARA que pueda coger mouseScreenPoint y funcione, debe haber un OnEnable y OnDisable activando y desactivando inputActions respectivamente. 
            if (!isUsingGamepad && !isUsingTouchScreen)
            {
                // Cojo la ScreenPointPos del ratón con input actions, la ScreenPos del Player, hago un vector, calculo el ángulo ATAN2, y seteo la rotación del player
                Vector2 mouseScreenPosition = inputActions.Player1.MousePosition.ReadValue<Vector2>();
                Vector2 playerScreenPoint = mainCamera.WorldToScreenPoint(gameObject.transform.position);

                Vector2 targetDirection = mouseScreenPosition - playerScreenPoint;

                //MOUSE POINTING TOP DIMENSION
                if (_gameManager_Script.IsTopDimension)
                {
                    float angle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                }
                //MOUSE POINTING RIGHT DIMENSION
                else
                {
                    //Calculo el ángulo ATAN2 de targetDirection
                    float angle = Mathf.Atan2(targetDirection.x, targetDirection.y) * Mathf.Rad2Deg - 90;


                    //MAKE PLAYER ROTATE AROUND WHEN FACING BACKWARDS WHEN RIGHT DIMENSION
                    if (targetDirection.x >= 0)
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(angle, 0, 0));
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(angle, 0, 180));
                    }
                    //Debug.Log(targetDirection);
                }
            }


            //NOT HITTING WHEN INVULNERABLE
            if (isInvulnerable)
            {
                invulnerableCounter += Time.deltaTime;
                if (invulnerableCounter >= invulnerableTimeSpan)
                {
                    isInvulnerable = false;
                    invulnerableCounter = 0;
                }
            }

            //CHANGE DIMENSION CHARGE Timer
            if (chargeCounter < charge_TimeSpan)
            {
                isCharged = false;
                chargeCounter += Time.unscaledDeltaTime * chargeMultiplier;
                if (chargeCounter >= charge_TimeSpan)
                {
                    isCharged = true;
                }

            }

            //DASH
            //LO RESTO PARA QUE LUEGO ME FACILITE LA VIDA CON EL UI IMAGE FILLAMOUNT
            if (isDashButtonPressed && canDash)
            {
                isDashing = true;
                dashCounter -= Time.unscaledDeltaTime;
                if (dashCounter <= 0)
                {
                    canDash = false;
                    isDashing = false;
                    dashCounter = 0;
                }
            }
            else if (!isDashButtonPressed && isDashing)
            {
                isDashing = false;
            }
            //CHARGE DASH
            else if (!isDashButtonPressed && dashCounter < dashTimeSpan)
            {
                dashCounter += Time.unscaledDeltaTime * dashChargeRate;

                if (dashCounter >= (dashTimeSpan / 3)) //el 3 es para que a un tercio de la carga total ya puedas hacer dash. Sacar variable si necesario
                {
                    canDash = true;
                }

                if (dashCounter >= dashTimeSpan)
                {
                    //SET MAX DASH CHARGE WHEN OVER LIMIT
                    dashCounter = dashTimeSpan;

                }

            }
        }

        


        
        

    }
    private void FixedUpdate()
    {
        if(!_gameManager_Script.IsCameraMoving)
        {

            //ME ENCARGO DE LAS BOUNDARIES EN EL LATEUPDATE
            if (!isDashing)
            {
                //player_Rb.velocity = moveDirection.normalized * (speed*100) * Time.unscaledDeltaTime;
                MovePlayer(currentMovementSpeed);
            }
            else
            {
                //player_Rb.velocity = moveDirection.normalized * (100*speed*dashVelocityMultiplier) * Time.unscaledDeltaTime;

                MovePlayer(currentMovementSpeed * dashVelocityMultiplier);
            }
        }

    }

    //MOVE PLAYER
    private void MovePlayer(float _movementSpeed)
    {
        //TOP DIMENSION
        if(_gameManager_Script.IsTopDimension)
        {
            //CALCULO NUEVA POSICION DEL PLAYER (sólo en x,z)
            //EN EL UPDATE YA ME ENCARGO DE CONVERTIR LOS INPUTS
            Vector3 newPlayerPos = new Vector3((gameObject.transform.position.x + ((_movementSpeed*moveDirection.x) * Time.unscaledDeltaTime)), gameObject.transform.position.y, (gameObject.transform.position.z + ((_movementSpeed*moveDirection.z) * Time.unscaledDeltaTime)));
            //APLICO NUEVA POSICIÖN AL PLAYER
            gameObject.transform.position = newPlayerPos;
        }
        //RIGHT DIMENSION
        else
        {
            //CALCULO NUEVA POSICION DEL PLAYER (sólo en y,z)
            //EN EL UPDATE YA ME ENCARGO DE CONVERTIR LOS INPUTS
            Vector3 newPlayerPos = new Vector3(gameObject.transform.position.x, (gameObject.transform.position.y + ((_movementSpeed*moveDirection.y) * Time.unscaledDeltaTime)), (gameObject.transform.position.z + ((_movementSpeed*moveDirection.z) * Time.unscaledDeltaTime)));
            //APLICO NUEVA POSICIÖN AL PLAYER
            gameObject.transform.position = newPlayerPos;

        }
    }

        //PLAYER BOUNDARIES
    private void LateUpdate()
    {

        //PLAYER BOUNDARIES
        if(_gameManager_Script.IsTopDimension)
        {
            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, (-cage.transform.localScale.x / 2) + (_meshRenderer.bounds.size.x / 2), (cage.transform.localScale.x / 2) - (_meshRenderer.bounds.size.x/2));
            clampedPos.z = Mathf.Clamp(clampedPos.z, (-cage.transform.localScale.z / 2) + (_meshRenderer.bounds.size.z / 2), (cage.transform.localScale.z / 2) - (_meshRenderer.bounds.size.z/2));
            transform.position = clampedPos;
        }
        else
        {
            Vector3 clampedPos = transform.position;
            clampedPos.y = Mathf.Clamp(clampedPos.y, (-cage.transform.localScale.y / 2) +(_meshRenderer.bounds.size.y/2), (cage.transform.localScale.y / 2) - (_meshRenderer.bounds.size.y/2));
            clampedPos.z = Mathf.Clamp(clampedPos.z, (-cage.transform.localScale.z / 2) +(_meshRenderer.bounds.size.y/2), (cage.transform.localScale.z / 2) - (_meshRenderer.bounds.size.z/2));
            transform.position = clampedPos;
        }
    }

    public void ChangeDimension()
    {
        if ((chargeCounter >= charge_TimeSpan) && !_gameManager_Script.isCameraMoving)
        {
            chargeCounter -= charge_Cost;

            _UI_ChargeRing_Script.MakeRingVisible();

            if (_gameManager_Script.IsTopDimension)
            {
                //SENDING IF IsTOPDim After dimension change
                ChangeCameraOrbitDimension(false);
            }
            else
            {
                //SENDING IF IsTOPDim After dimension change
                ChangeCameraOrbitDimension(true);
            }
        }
    }

    private void Aim()
    {
        //FIRE/AIM WHEN JOYSTICK MOVED
        if ((aimDirection.magnitude >= 0.03) && (!_gameManager_Script.IsGamePaused ))//&& !_gameManager_Script.isCameraMoving))
        {

            if(!_gameManager_Script.IsCameraMoving)
            {
                //TOP DIMENSION
                if (_gameManager_Script.IsTopDimension)
                {
                    {

                        float angle = Mathf.Atan2(aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
                        transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                    }
                }
                //RIGHT DIMENSION
                else
                {
                    //if ((aimDirection.magnitude >= 0.03) && (!_gameManager_Script.IsGamePaused && !_gameManager_Script.isCameraMoving))
                    {
                        //Calculo el ángulo ATAn2 de aimDirection
                        float angle = Mathf.Atan2(aimDirection.x, aimDirection.y) * Mathf.Rad2Deg - 90;

                        //MAKE PLAYER ROTATE AROUND WHEN FACING BACKWARDS WHEN RIGHT DIMENSION
                        if (aimDirection.x >= 0)
                        {

                            transform.rotation = Quaternion.Euler(new Vector3(angle, 0, 0));

                        }
                        else
                        {

                            transform.rotation = Quaternion.Euler(new Vector3(angle, 0, 180));
                        }
                    }
                }
            }

           

            //DISPARA CON JOYSTICK
            _fire_Script.ShootButtonPressed();

        }
        //DEACTIVATE SHOOTING
        else
        {
            _fire_Script.ShootButtonUnpressed();
        }

       
    }

    public void ChangeCameraOrbitDimension(bool _isNextDimensionTOP)
    {
              
        if(_isNextDimensionTOP)
        {
            cameraOrbit_Animator.SetBool("isTopDimension", true);
            if(!cameraOrbit_Animator.enabled)
            {
                cameraOrbit_Animator.enabled = true;
            }

           // _gameManager_Script.SetTopDimension(); por ahora lo hago con anim events
        }
        else
        {
            cameraOrbit_Animator.SetBool("isTopDimension", false);
            if(!cameraOrbit_Animator.enabled)
            {
                cameraOrbit_Animator.enabled = true;
            }
            
        }
            //_gameManager_Script.SetRightDimension();
        
    }

    public void KillPlayer()//(float speedOfWhatKilledPlayer) -> para las partículas?
    {


        //ERA UNA PRUEBA, sólo se llama una vez; no vale
        //gameObject.GetComponentInChildren<TrailRenderer>().gameObject.transform.Translate(new Vector3(0, 0, -speedOfWhatKilledPlayer*Time.deltaTime));

        TrailRenderer trail = gameObject.GetComponentInChildren<TrailRenderer>();
        if(trail != null)
        {
            trail.transform.SetParent(null);
            trail.autodestruct = true;

        }
       //gameObject.GetComponentInChildren<TrailRenderer>().autodestruct = true;
      //gameObject.GetComponentInChildren<TrailRenderer>().gameObject.transform.SetParent(null);

        if(_gameManager_Script.CurrentLives <= 0)
        {
            //GAME OVER
            _gameManager_Script.GameOver();
        }

        Destroy(gameObject);
    }




    public void IncreaseChargeAtMAX() //WHEN AN ENEMY KILLED, or power up taken, charge CounterDimension to max 
    {
        chargeCounter = charge_TimeSpan;
    }


    public void AddSpeed(float speedToAdd)
    {
        if(currentMovementSpeed < maximumMovementSpeed)
        {

            currentMovementSpeed += (speedToAdd); // el 2 es para que vaya restando numeros pequeños, a mayor número donde el 2, resta menos

            if(currentMovementSpeed >= maximumMovementSpeed)
            {
                currentMovementSpeed = maximumMovementSpeed;
            }
        }
        PlayerPrefs.SetFloat("currentMovementSpeed", currentMovementSpeed);
    }
    public void ResetSpeed()
    {
        currentMovementSpeed = ORIGINAL_MOVEMENT_SPEED;
    }

    public void IncreaseChargePower(float chargePowerToIncrease)
    {
        if(chargeMultiplier < chargeMultiplierMaximum)
        {
            chargeMultiplier += chargePowerToIncrease;
            if(chargeMultiplier >= chargeMultiplierMaximum)
            {
                chargeMultiplier = chargeMultiplierMaximum; 
            }
        }
    }


    //CONTROLS


    void OnMove(InputValue joystickValue)
    {
        moveInput = joystickValue.Get<Vector2>();
    }

    void OnChangeDimension(InputValue buttonValue)
    {
        ChangeDimension();

    }

  

    void OnFire(InputValue buttonValue)
    {
       // _fire_Script.Fire();

        if(buttonValue.isPressed)
        {
            //Debug.Log("esto no debería salir con mando"); -> porque llamo a fire desde AIM (para que dispare con el joystick)
            _fire_Script.ShootButtonPressed();
           // _fire_Script.Fire();

           // _fire_Script.isShootButtonPressed 
        }
        else
        {
            _fire_Script.ShootButtonUnpressed();
        }

        
    }

    void OnAim(InputValue joystickValue)
    {
        aimDirection = joystickValue.Get<Vector2>();

        Aim(); //PODRía quitarla de aquí y meterla en el update?

    }

    void OnActivateSuperMode(InputValue buttonValue)
    {
        _fire_Script.ActivateSuperMode();
    }
   
    void OnDash(InputValue buttonValue)
    {
        if(buttonValue.isPressed)
        {
            isDashButtonPressed = true;
            //SLOW MO WHEN DASHING -> Player gets slown down too, pq rB.velocity está basado en DeltaTime
            cameraOrbit_Animator.gameObject.GetComponent<DisableAnimator_Script>().DoSlowMotion(); 
        }
        else
        {
            isDashButtonPressed = false;
        }
    }


    //EL ENABLE Y DISABLE PARA inputActions sirve para que pueda coger ScreenToWorldPoint. Sin ellos, no funciona.
    private void OnEnable()
    {
        inputActions.Enable();

    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

}
