using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Advisor_Script : MonoBehaviour
{
    [SerializeField] bool isVisible = false;
    public bool IsVisible
    {
        get { return isVisible; }
    }
    [SerializeField] Image advisor_Image;
    [SerializeField] TMPro.TextMeshProUGUI advisor_Text;

    [SerializeField] Vector2 offset;
    [SerializeField] GameManager_Script _gameManager;
    [SerializeField] CanvasManager_Script _canvasManager;
    

    // Start is called before the first frame update
    void Start()
    {
        //DESCOMENTAR CUANDO TENGA IMAGEN
        //advisor_Image = gameObject.GetComponent<Image>();
        advisor_Text = gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>();
        _canvasManager = gameObject.transform.parent.parent.GetComponent<CanvasManager_Script>();

        //DEACTIVATE AT START -> Tiene que estar activado entonces
        DeactivateAdvisorImage();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isVisible && _gameManager.playerInScene != null) //cambiar a image al tenerla
        {
            //CALCULO NUEVA POSICIÓN A PARTIR DE LA DEL PLAYER con WorldToScreenPoint
            Vector2 pos = _canvasManager.mainCamera.WorldToScreenPoint(_gameManager.playerInScene.transform.position);

            advisor_Text.transform.position = pos + offset;
        }


    }

    public void ActivateAdvisorImage()
    {
        isVisible = true;

        //advisor_Image.enabled = true;
        advisor_Text.enabled = true;
    }

    public void DeactivateAdvisorImage()
    {
        isVisible = false;
        //advisor_Image.enabled = false;
        advisor_Text.enabled = false;
    }


}
