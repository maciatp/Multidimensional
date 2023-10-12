using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ButtonChangeDimension_Script : MonoBehaviour
{
    [SerializeField] Player_Controller _movement_Script;

    private void Start()
    {
        _movement_Script = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>().playerInScene.gameObject.GetComponent<Player_Controller>();
    }

    public void UI_Button_ChangeDimension()
    {
        _movement_Script.ChangeDimension();
    }

}
