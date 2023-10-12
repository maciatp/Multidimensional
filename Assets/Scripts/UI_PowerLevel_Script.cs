using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PowerLevel_Script : MonoBehaviour
{

    [SerializeField] Image chargeImage;
    [SerializeField] TMPro.TextMeshProUGUI powerLevel_Text;
    [SerializeField] Animator _animator;

    [SerializeField] GameManager_Script _gameManager;
    //[SerializeField] CanvasManager_Script _canvasManager;
    [SerializeField] Fire_Script _fire_Script;
    public Fire_Script _FireScript
    {
        get { return _fire_Script; }
        set { _fire_Script = value; }
    }

    private void Start()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();
        chargeImage = gameObject.transform.Find("UI_Power_Meter").GetComponent<Image>();
        powerLevel_Text = gameObject.transform.Find("PowerLevel_Text").GetComponent<TMPro.TextMeshProUGUI>();

        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();

        //_canvasManager = gameObject.transform.parent.GetComponent<CanvasManager_Script>();
        _fire_Script = _gameManager.playerInScene.gameObject.GetComponent<Fire_Script>();
    }


    private void Update()
    {
        if(_gameManager.playerInScene != null)
        {

            //POWER METER

            //LEVELS 1-4
            if(_fire_Script.CurrentPowerLevel != _fire_Script.MaxPowerLevelIs)
            {


                if(chargeImage.fillAmount != (float)_fire_Script.PowerUpsCollected / (float)_fire_Script.UpgradesLevelsSpan)
                {
                    chargeImage.fillAmount = (float)_fire_Script.PowerUpsCollected / (float)_fire_Script.UpgradesLevelsSpan;


                }



                

            }
            //LEVEL 5 - SUPER MODE
            else
            {
                //ACTIVATE AND DEACTIVATE POWER METER ANIMATION
                if (chargeImage.fillAmount == 1 && !_animator.GetBool("IsFull"))
                {
                    _animator.SetBool("IsFull", true);
                }
                else if (chargeImage.fillAmount != 1 && _animator.GetBool("IsFull"))
                {

                    _animator.SetBool("IsFull", false);
                }

                //SE VA VACIANDO SEGÚN EL COUNTER DE FIRE_SCRIPT. 1 - tal para que vaya bajando de 1
                chargeImage.fillAmount = 1 - (_fire_Script.SuperModeCounterIs / _fire_Script.SuperModeTimeSpanIs);
            }


            //POWER TEXT
            if(powerLevel_Text.text != _fire_Script.CurrentPowerLevel.ToString())
            {

                powerLevel_Text.text = _fire_Script.CurrentPowerLevel.ToString();

            }
        }
    }




}
