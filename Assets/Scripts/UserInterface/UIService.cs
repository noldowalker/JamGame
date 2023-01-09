using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;


public class UIService : MonoBehaviour
{
    public static UIService Current; 
    
    [SerializeField] 
    private Canvas gameCanvas;
    [SerializeField] 
    private HotkeyBar hotKeyPrefab;
    
    private void Awake()
    {
        if (Current != null)
            Debug.LogError("Создано больше 1 сервиса для интерфейса!");

        Current = this;
    }

    void Start()
    {
        if (gameCanvas == null)
            Debug.LogError("Не установлен основной игровой канвас для сервиса пользовательского интерфейса");
        if (hotKeyPrefab == null)
            Debug.LogError("Не установлен префаб для панели горячих клавиш");

        Instantiate(hotKeyPrefab, gameCanvas.transform);
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

        if (cooldown <= 0)
            yield break;

        var data = new CooldownObservingDTO()
        {
            InitialCooldownValue = cooldown
        };
            
        ObserverWithData<CooldownObservingDTO>.FireEvent(skillEvent, data);
    }
}
