using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText_Script : MonoBehaviour
{

    [SerializeField] TMPro.TextMeshProUGUI currentScore_Text;
    
    // Start is called before the first frame update
    void Start()
    {
        currentScore_Text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();    
    }

    // Update is called once per frame
    void Update()
    {
        currentScore_Text.text = ("SCORE: " + gameObject.transform.parent.parent.GetComponent<CanvasManager_Script>()._scoreManager.CurrentScore.ToString("0000000"));

    }
}
