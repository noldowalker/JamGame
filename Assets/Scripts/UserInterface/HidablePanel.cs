using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidablePanel : MonoBehaviour
{
    private Vector3 _hidePoint;
    private Vector3 _showPoint;
    private bool _panelStatus;

    protected virtual void Awake()
    {
        var currentPoint = transform.position;

        _hidePoint = new Vector3(
            -Screen.width,
            -Screen.height,
            currentPoint.z
        );

        _showPoint = new Vector3(
            Screen.width / 2,
            Screen.height / 2,
            currentPoint.z
        );
    }

    private void Start()
    {
        HidePanel();
    }

    public void HidePanel()
    {
        transform.position = _hidePoint;
        _panelStatus = false;
    }

    public void ShowPanel()
    {
        transform.position = _showPoint;
        _panelStatus = true;
    }

    public void SwitchPanelVisibility()
    {
        if (_panelStatus)
            HidePanel();
        else
            ShowPanel();
    }
}
