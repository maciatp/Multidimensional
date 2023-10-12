using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager_Script : MonoBehaviour
{
    public Camera mainCamera;
    //public GameObject playerInScene; -> Accedo al de Gamemanager

    [SerializeField] GameManager_Script _gameManager;
    [SerializeField]public ScoreManager_Script _scoreManager;





    [SerializeField] GameObject gamePLAYscreen;
    [SerializeField] GameObject gameOVERscreen;
    ////TODOS LOS OBJETOS DE LA UI -> PARA PODER TENERLOS DESACTIVADOS EN EL EDITOR Y QUE SE ACTIVEN AL DARLE A PLAY (más abajo se activan)
    //[SerializeField] GameObject UI_ChargeRing;
    //[SerializeField] GameObject UI_ScoreText;
    //[SerializeField] GameObject UI_PowerMeters;
    //[SerializeField] GameObject UI_Advisor;
    //[SerializeField] GameObject UI_Level;


    
   

    // Start is called before the first frame update
    void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
        _scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager_Script>();

    }

    private void Start()
    {
        mainCamera = _gameManager.mainCamera;
        //playerInScene = _gameManager.playerInScene.gameObject; //mejor tener otra variable para el player, o acceder directamente desde _gameManager.playerInScene desde donde se necesite en UIChargeRing FixedUpdate?


        ////UI ELEMENTS SETTING -> //FIND (CHILD) LO ENCUENTRA AUNQUE ESTÉ INACTIVO, GameObject.Find NO.     -> Puedo buscar UI_GAMEPLAYSCREEn y activarlo, en una línea de código. antes tenía cada elemento por sepadado
        gamePLAYscreen = gameObject.transform.Find("UI_GameplayScreen").gameObject;
        gameOVERscreen = gameObject.transform.Find("UI_GameOverScreen").gameObject;
        
       
        ////UI ELEMENTS ACTIVATION 
        ///GAMEPLAY SCREEN DEBE ACTIVARSE AL INICIARSE
        if(!gamePLAYscreen.activeSelf)
        {
            gamePLAYscreen.SetActive(true);
        }

        //GAME OVER SCREEn debe desactivarse si está activo
        if(gameOVERscreen.activeSelf)
        {
            gameOVERscreen.SetActive(false);
        }




        


        
    }



    //LOS PREPARO PARA LLAMARLOS DESDE EL GAMEMANAGER, mejor ahí donde llamarlas o crear un método aquí de SetUpGameplayScreen, que desactive todo lo que no se necesite durante el gameplay? (y otra para gameOver,etc.) ?

    public void ActivateGAMEPLAYScreen()
    {
        gamePLAYscreen.SetActive(true);
    }

    public void DeactivateGAMEPLAYscreen()
    {
        gamePLAYscreen.SetActive(false);
    }



    public void ActivateGameOVERscreen()
    {
        gameOVERscreen.SetActive(true);
    }
   
    public void DeactivateGameOVERscreen()
    { 
        gameOVERscreen.SetActive(false);
    }

}
