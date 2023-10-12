using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DashMeter_Script : MonoBehaviour
{
    [SerializeField] Image dashCharge_Image;

    [SerializeField] GameManager_Script _gameManger;

    [SerializeField] Player_Controller _movement_Script;
    public Player_Controller SetMovementScript
    {
        set { _movement_Script = value; }
    }

    private void Start()
    {
        dashCharge_Image = gameObject.GetComponent<Image>();

        _gameManger = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();

        _movement_Script = _gameManger.playerInScene.GetComponent<Player_Controller>();


    }


    private void Update()
    {
        if(dashCharge_Image.fillAmount != (_movement_Script.DashCounter/_movement_Script.DashTimeSpan))
        {
            dashCharge_Image.fillAmount = (_movement_Script.DashCounter) / _movement_Script.DashTimeSpan;

        }

    }
}
