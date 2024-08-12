using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialArrow : Singleton<TutorialArrow>
{
    public void Enable()
    {
        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
    }
    public void Disable()
    {
        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
    }
    
}
