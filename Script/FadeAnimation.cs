using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAnimation : MonoBehaviour
{
    private CanvasGroup canvasGroupObj;
    [SerializeField]
    private float fadeSpeed = 0.7f;
    private bool fadeIn = false;
    private bool fadeOut = false;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroupObj=this.gameObject.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn) {
            if (canvasGroupObj.alpha < 1)
            {
                canvasGroupObj.alpha += fadeSpeed * Time.deltaTime;
                if (canvasGroupObj.alpha >= 1) {
                    fadeIn = false;
                }
                    
                    
            }
        } else if (fadeOut) {
            if (canvasGroupObj.alpha >= 0)
            {
                canvasGroupObj.alpha -= fadeSpeed * Time.deltaTime;
                if (canvasGroupObj.alpha <= 0) {
                    fadeOut = false;
                    this.gameObject.SetActive(false);
                }
                    
            }
        }
            
            

    }

    public void FadeIn() {
        //Debug.Log("Fade in");
        this.gameObject.SetActive(true);
        fadeIn = true;
    }
    public void FadeOut()
    {
        //Debug.Log("Fade out");
        fadeOut = true;
    }
}
