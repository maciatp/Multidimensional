using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Level_Script : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI level_Text;
    [SerializeField] GameManager_Script _gameManager;



    private void Start()
    {
        level_Text = gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();

        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
    }

    private void Update()
    {

        if (level_Text.text != _gameManager.CurrentLevel.ToString())
        {
            level_Text.text = _gameManager.CurrentLevel.ToString("Level "+  "0#");
           
        }

    }



}
