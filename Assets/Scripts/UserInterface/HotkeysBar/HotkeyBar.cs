using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.UI;

public class HotkeyBar : MonoBehaviour
{
    public Image PlayerHpBar => hpImage;

    [SerializeField]
    private HotkeyCell hotkeyCellPrefab;
    [SerializeField]
    private Transform containerTransform;
    [SerializeField]
    private List<HotkeyCell> hotkeysCells = new List<HotkeyCell>();
    [SerializeField]
    private Image hpImage;

    // Start is called before the first frame update
    void Start()
    {
        if(hotkeyCellPrefab == null)
            Debug.LogError("Для панели горячих клавиш не установлен префаб ячейки");
        if(hpImage == null)
            Debug.LogError("Для панели горячих клавиш не указана ссылка на элемент здоровья игрока");
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
