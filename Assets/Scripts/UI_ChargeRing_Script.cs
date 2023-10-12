using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChargeRing_Script : MonoBehaviour
{

    [SerializeField] Image chargeRing_Image;

    [SerializeField] bool isGoingToDisappear = false;

    [SerializeField] CanvasManager_Script _canvas_Script;

    [SerializeField] Vector2 offset = new Vector2(-0.03f, -0.08f);
    public Player_Controller _movement_Script;


    [SerializeField] Color lowChargeColor = Color.red;
    [SerializeField] Color medChargeColor = Color.cyan;
    [SerializeField] Color highChargeColor = Color.green;

    private void Awake()
    {
        
    }

    //DESCOMENTAR LO COMENTADO SI HAY ALGÚN FALLO. He desactivado que coja el player desde Canvas, y lo coja desde Game manager, a ver si en la build funciona el anillo. (Funciona, pero no se coloca bien) -> el fallo tiene que venir de WorldToViewport. Es mejor WorldToScreen?

    // Start is called before the first frame update
    void Start()
    {
        chargeRing_Image = gameObject.GetComponent<Image>();
        chargeRing_Image.fillAmount = 0;
        _canvas_Script = gameObject.transform.parent.parent.GetComponent<CanvasManager_Script>();
        _movement_Script = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Script>().playerInScene.gameObject.GetComponent<Player_Controller>();// _canvas_Script.playerInScene.gameObject.GetComponent<Movement_Script>();
    }
    

    // Update is called once per frame
    void Update()
    {
    }
    private void LateUpdate() //Está en el LateUpdate para que el UI se calcule todo después del update del juego (sino el anillo se veía perdido un frame por ahí)
    {
        chargeRing_Image.fillAmount = _movement_Script.ChargeCounter / _movement_Script.Charge_TimeSpan;
        if (_movement_Script != null)//(_canvas_Script.playerInScene != null)
        {

            //WORLD TO VIEWPORT transforma una posición del mundo a un rango de (0,0) a (1,1) 
            Vector2 pos = _canvas_Script.mainCamera.WorldToScreenPoint(_movement_Script.gameObject.transform.position);//_canvas_Script.playerInScene.transform.position);// playerTransform.position);
            //VIEWPORT TO SCREEN POINT transforma de (0,0) (1,1) a píxeles
            chargeRing_Image.transform.position = pos + offset; //_canvas_Script.mainCamera.ViewportToScreenPoint(pos + offset);

        }
        else
        {
            chargeRing_Image.enabled = false;
        }

        if (chargeRing_Image.fillAmount < 0.35f)
        {
            chargeRing_Image.color = lowChargeColor;
        }
        else if(chargeRing_Image.fillAmount >= 0.35f && chargeRing_Image.fillAmount < 0.75f)
        {
            chargeRing_Image.color = medChargeColor;
        }
        else if(chargeRing_Image.fillAmount == 1)
        {
            chargeRing_Image.color = highChargeColor;

            if(!isGoingToDisappear)
            {
                StartCoroutine(MakeRingInvisible());

            }
        }

    }

    IEnumerator MakeRingInvisible()
    {
        isGoingToDisappear = true;
        yield return new WaitForSeconds(1);
        //MAKE INVISIBLE
        if(chargeRing_Image.fillAmount == 1)
        {

            chargeRing_Image.enabled = false;
            chargeRing_Image.transform.GetChild(0).gameObject.SetActive(false); //DEACTIVATE RING BACKGROUND
        }
    }

    public void MakeRingVisible()
    {
        isGoingToDisappear = false;
        chargeRing_Image.enabled = true;
        chargeRing_Image.transform.GetChild(0).gameObject.SetActive(true); //ACTIVATE RING BACKGROUND

    }
}
