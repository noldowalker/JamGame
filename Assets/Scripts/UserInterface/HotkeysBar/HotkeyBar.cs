using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;

public class HotkeyBar : MonoBehaviour
{
    [SerializeField]
    private HotkeyCell hotkeyCellPrefab;
    [SerializeField]
    private Transform containerTransform;
    [SerializeField]
    private List<HotkeyCell> hotkeysCells = new List<HotkeyCell>();

    // Start is called before the first frame update
    void Start()
    {
        if(hotkeyCellPrefab == null)
            Debug.LogError("Для панели горячих клавиш не установлен префаб ячейки");
    }

    private void UnsubscribeHotkeys()
    {
        hotkeysCells.ForEach(h => h.Unsubscribe());
    }

    private void OnDestroy()
    {
        UnsubscribeHotkeys();
    }
}
