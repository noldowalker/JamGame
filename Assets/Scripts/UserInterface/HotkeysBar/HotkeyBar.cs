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
    
    private List<HotkeyCell> _hotkeysCells;
    
    
    private void Awake()
    {
        _hotkeysCells = new List<HotkeyCell>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(hotkeyCellPrefab == null)
            Debug.LogError("Для панели горячих клавиш не установлен префаб ячейки");


        AddHotkey(1);
        AddHotkey(2);
        AddHotkey(3);
    }
    
    public void AddHotkey(int hotkeyChar)
    {
        var hotkey = Instantiate(hotkeyCellPrefab, containerTransform);
        hotkey.SetNewHotkey(hotkeyChar.ToString());
        _hotkeysCells.Add(hotkey);

        Debug.Log(@$"hotkeyChar {hotkeyChar}");
        switch (hotkeyChar)
        {
            case 1 :
                hotkey.SetEventSubscription(Events.Button1Pressed);
                break;
            case 2 :
                hotkey.SetEventSubscription(Events.Button2Pressed);
                break;
            case 3 :
                hotkey.SetEventSubscription(Events.Button3Pressed);
                break;
        }
    }

    private void UnsubscribeHotkeys()
    {
        _hotkeysCells.ForEach(h => h.Unsubscribe());
    }

    private void OnDestroy()
    {
        UnsubscribeHotkeys();
    }
}
