using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager_Script : MonoBehaviour
{
    //USO DE GET SET

    //Variable privada
    [SerializeField] int currentScore;
    [SerializeField] GameManager_Script _gameManager;


    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
    }

    //Get/Set accesibles desde fuera 
    public int CurrentScore 
    {
        get { return currentScore; }
        //set { currentScore = value; } por ahora sólo tendría que llamarse cuando se reinicie el juego, pero también puede ser un método de éste script.
    } 

/*
    QUEDA COMO TUTORIAL DE GET/SET
    // Variable
    private int _iRandomNumber;
    // Getter and setter
    public int iRandomNumber
    {
        get { return _iRandomNumber; }
        set { _iRandomNumber = value; }

    }
*/


    public void AddScore(int scoreToAdd)
    {
        currentScore += scoreToAdd; //Cambiar a que le manden cuánto tiene que añadirse desde cada objeto (como en StarFoxCV)

        //CADA X PUNTOS, dar una vida
        if (CurrentScore % _gameManager.AddLifePointsSpan == 0)
        {
            _gameManager.AddLife();
            Debug.Log("HE dado una vida");
        }

    }

   
}
