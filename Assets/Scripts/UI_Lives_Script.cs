using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Lives_Script : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI lives_Text;

    [SerializeField] GameManager_Script _gameManager;

    private void Start()
    {
        lives_Text = gameObject.transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();

    }


    private void Update()
    {
        if (lives_Text.text != "X " + _gameManager.CurrentLives.ToString())
        {
            lives_Text.text = "X " + _gameManager.CurrentLives.ToString();
        }
    }

}
