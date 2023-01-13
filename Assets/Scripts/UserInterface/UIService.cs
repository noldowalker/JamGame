using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.UI;
using UserInterface.Enums;
using UserInterface.IngameInformation;


public class UIService : MonoBehaviour
{
    public static UIService Current; 
    
    [Header("Включить игровой интерфейс")]
    [SerializeField] 
    private bool showGameLayout;
    
    [Space]
    [Header("Необходимые для игрового интерфейса ссылки")]
    [SerializeField] 
    private Canvas gameCanvas;
    [Header("Префабы")]
    [SerializeField] 
    private HotkeyBar hotKeyPrefab;
    [SerializeField] 
    private HelpPanel helpPanelPrefab;
    [SerializeField]
    private HotkeyCell hotkeyCellPrefab;
    [Header("Спрайты")]
    [SerializeField]
    private Sprite helpIcon;

    private HotkeyBar _hotKeyPanel;
    private HelpPanel _helpPanel;
    private float _testHp = 100;
    
    private void Awake()
    {
        if (Current != null)
            Debug.LogError("Создано больше 1 сервиса для интерфейса!");

        Current = this;
    }

    void Start()
    {
        if (showGameLayout)
            TurnOnGameInterface();
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I)) 
            ObserverWithoutData.FireEvent(Events.HelpPanelCalled);
    }

    public Image GetPlayerHpBarImage() => _hotKeyPanel.PlayerHpBar;
    
    public HotkeyCell AddHotkey(Events eventForPress, string hotkeyChar, Sprite icon)
    {
        var containerTransform = gameCanvas.transform;
        var hotkey = Instantiate(hotkeyCellPrefab, containerTransform);
        hotkey.SetNewHotkey(hotkeyChar);
        hotkey.SetEventSubscription(eventForPress);
        hotkey.SetIcon(icon);
        
        return hotkey;
    }
    
    private void TurnOnGameInterface()
    {
        if (gameCanvas == null)
            Debug.LogError("Не установлен основной игровой канвас для сервиса пользовательского интерфейса");
        if (hotKeyPrefab == null)
            Debug.LogError("Не установлен префаб для панели горячих клавиш");
        if (helpPanelPrefab == null)
            Debug.LogError("Не установлен префаб для панели-подсказки");

        var helpHotkey = AddHotkey(Events.HelpPanelCalled, "I", helpIcon);
        helpHotkey.AlignToCoords(50, 50, Align.LeftBottom);
        
        _hotKeyPanel = Instantiate(hotKeyPrefab, gameCanvas.transform);
        
        _helpPanel = Instantiate(helpPanelPrefab, gameCanvas.transform);
        ObserverWithoutData.Sub(Events.HelpPanelCalled, _helpPanel.SwitchPanelVisibility);
    }

    private void OnDestroy()
    {
        Current = null;
    }
}
