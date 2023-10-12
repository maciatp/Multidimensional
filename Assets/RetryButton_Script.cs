using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton_Script : MonoBehaviour
{
  
    public void RetryButtonAction()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>().RetryGame();
    }

}
