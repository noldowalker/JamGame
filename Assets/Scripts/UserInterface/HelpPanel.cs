using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanel : HidablePanel
{
    private void Awake()
    {
        //Debug.Log("AWAKE!");
        var rectTransform = GetComponent<RectTransform>();
        var newWidth = Screen.width - 100;
        var newHeight = Screen.height - 100;
        var rectTransformSizeDelta = rectTransform.sizeDelta;
        rectTransformSizeDelta.x = newWidth;
        rectTransformSizeDelta.y = newHeight;
        rectTransform.sizeDelta = rectTransformSizeDelta;
        
        base.Awake();
    }
}
