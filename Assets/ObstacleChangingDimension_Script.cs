using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleChangingDimension_Script : MonoBehaviour
{

    [SerializeField] bool isRotating = false;
    public bool IsRotating
    {
        get { return isRotating; }
    }


    [SerializeField] bool hasToRotate = false;
    [SerializeField] float changingCounter = 0;
    [SerializeField] float changingTimeSpan = 2.5f;

    
    Animator obstacle_Animator;


    // Start is called before the first frame update
    void Start()
    {
        
        //si está en la root es que no es Obstacle5_changing (la cruz)
        if(transform.parent == null)
        {
            obstacle_Animator = gameObject.GetComponent<Animator>();

        }
        else
        {
            obstacle_Animator = gameObject.GetComponentInParent<Animator>();
        }

        
        //PARO EL ANIMATOR PARA QUE NO SE MUEVA
        obstacle_Animator.enabled = false;

        isRotating = false;
        hasToRotate = false;



        
    }

    // Update is called once per frame
    void Update()
    {

        if(!isRotating && changingCounter < changingTimeSpan)
        {
            changingCounter += Time.deltaTime;

            if(changingCounter>= changingTimeSpan)
            {
                if(!obstacle_Animator.enabled)
                {
                    //ROTATE AT ENABLE
                    obstacle_Animator.enabled = true;
                    changingCounter = 0;
                }
                else
                {
                    //ROTATE 2nd time and beyond
                    obstacle_Animator.SetBool("hasToRotate", true);
                    changingCounter = 0;


                }
            }

        }
                        
    }

    //Empiza. cuando se pasa el contador se activa el animator, que hace que se gire hacia RIGHT (en el obstacle5), activando el bool del animator, dejando que gire.
    //Luego, en el último frame de animación, se llama a Deactivate is rotating que hace que se quede en pausa hasta que se pase el contador otra vez.

    public void DeactivateIsRotating()
    {
        isRotating = false;
    }

    public void DeactivateHAStoRotate()
    {
        obstacle_Animator.SetBool("hasToRotate", false);
        isRotating = true;
       
    }



}
