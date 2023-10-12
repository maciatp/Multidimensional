using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimator_Script : MonoBehaviour
{
    [SerializeField] Animator cameraOrbit_Animator;
    [SerializeField] public GameObject player = null;
    [SerializeField] public GameManager_Script _gameManager;

    public bool movingCameraAtStart = false;

    

    public bool mustDoSlowMo = false;

    //How strong the slowmo is
    public float slowdownFactor = 0.05f;
    //How long is the transition to slowmo and inverse
    public float slowdownLenth = 2f;

    private void Awake()
    {
        cameraOrbit_Animator = gameObject.GetComponent<Animator>();

        if(movingCameraAtStart) //esto es por comodidad, así si en algún momento interesa que la cámara orbite justo cuando empieza la partida, ya está montado (activar bool en el editor, desactivado es SIN ORBITAR AL INICIO)
        {
            cameraOrbit_Animator.enabled = true;
        }
        else
        {

            cameraOrbit_Animator.enabled = false; //la dejo en disabled para que cuando el jugador pulse se active por primera vez y ya funcione todo.
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
    }

    private void Update()
    {
        Time.timeScale += (1f / slowdownLenth) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);

        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void DoSlowMotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

    }

    //LA llamo desde ANIMATION EVENT
    public void CameraMoving()
    {
        if(player != null)
        {
           _gameManager.isCameraMoving = true;
        }
        
    }

    //LA llamo desde ANIMATION EVENT
    public void CameraNotMoving()
    {
        if(player != null)
        {
           _gameManager.isCameraMoving = false;
        }
    }


    //LA llamo desde ANIMATION EVENT para que las hitpoints sólo se muevan una vez terminado el movimiento de la cámara
    public void SetTopDimension()
    {
        _gameManager.SetTopDimension();
    }

    //LA llamo desde ANIMATION EVENT
    public void SetRightDimension()
    {
        _gameManager.SetRightDimension();
    }



}
