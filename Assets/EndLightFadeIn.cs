using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLightFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasgroup;
    private bool _fadein = false;
    public float TimeToFade;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_fadein)
        {
            if(canvasgroup.alpha < 1)
            {
                canvasgroup.alpha += TimeToFade * Time.deltaTime;
                if(canvasgroup.alpha >= 1) 
                    {
                        _fadein = false;
                    }
            }
        }
    }

    public void FadeIn()
    {
        _fadein = true;
    }
}
