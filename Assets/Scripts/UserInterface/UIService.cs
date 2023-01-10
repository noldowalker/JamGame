using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
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
    
    private float testHp = 100;
    
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

    // ToDo: Бинды чисто для теста, после реализации контроллера персонажа и управления - удалить.
    // Бинды прокидывать из нормального места
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) 
            FireSkillEvent(Events.Button1Pressed);
        if (Input.GetKeyUp(KeyCode.Alpha2)) 
            FireSkillEvent(Events.Button2Pressed, 5f);
        if (Input.GetKeyUp(KeyCode.Alpha3)) 
            FireSkillEvent(Events.Button3Pressed, 7f);
        if (Input.GetKeyUp(KeyCode.F1)) 
            ObserverWithoutData.FireEvent(Events.HelpPanelCalled);
        
    }
    
    public void FireSkillEvent(Events skillEvent, float cooldown = 0)
    {
        Debug.Log($@"skillEvent {skillEvent}");
        StartCoroutine(SkillUsed(skillEvent, cooldown));
    }
    
    private IEnumerator SkillUsed(Events skillEvent, float cooldown = 0)
    {
        ObserverWithoutData.FireEvent(skillEvent);
        yield return new WaitForSeconds(0.1f);

        testHp -= 10;
        var hpData = new NewHPPercentObservingDTO()
        {
            NewPercent = (testHp / 100)
        };
        ObserverWithData<NewHPPercentObservingDTO>.FireEvent(Events.HPLossPercent, hpData);
        
        if (cooldown <= 0)
            yield break;

        var data = new CooldownObservingDTO()
        {
            InitialCooldownValue = cooldown
        };
            
        ObserverWithData<CooldownObservingDTO>.FireEvent(skillEvent, data);
    }

    private void TurnOnGameInterface()
    {
        if (gameCanvas == null)
            Debug.LogError("Не установлен основной игровой канвас для сервиса пользовательского интерфейса");
        if (hotKeyPrefab == null)
            Debug.LogError("Не установлен префаб для панели горячих клавиш");
        if (helpPanelPrefab == null)
            Debug.LogError("Не установлен префаб для панели-подсказки");

        var helpHotkey = AddHotkey(Events.HelpPanelCalled, "F1", helpIcon);
        helpHotkey.AlignToCoords(0, 0, Align.LeftBottom);
        
        _hotKeyPanel = Instantiate(hotKeyPrefab, gameCanvas.transform);
        
        _helpPanel = Instantiate(helpPanelPrefab, gameCanvas.transform);
        ObserverWithoutData.Sub(Events.HelpPanelCalled, _helpPanel.SwitchPanelVisibility);
    }
    
    public HotkeyCell AddHotkey(Events eventForPress, string hotkeyChar, Sprite icon)
    {
        var containerTransform = gameCanvas.transform;
        var hotkey = Instantiate(hotkeyCellPrefab, containerTransform);
        hotkey.SetNewHotkey(hotkeyChar);
        hotkey.SetEventSubscription(eventForPress);
        hotkey.SetIcon(icon);
        
        return hotkey;
    }
}
