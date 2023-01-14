using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UserInterface.HotkeysBar;

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
    
    [Header("Ссылки на кнопки для биндов")]
    [SerializeField]
    private HotkeyCell punchHotkeyCell;
    [SerializeField]
    private HotkeyCell kickHotkeyCell;
    [SerializeField]
    private HotkeyCell runHotkeyCell;
    [SerializeField]
    private HotkeyCell jumpHotkeyCell;
    [SerializeField]
    private HotkeyCell ult1HotkeyCell;
    [SerializeField]
    private HotkeyCell ult2HotkeyCell;

    private EnergySpheresContainer energyPoints;
    void Start()
    {
        if(hotkeyCellPrefab == null)
            Debug.LogError("Для панели горячих клавиш не установлен префаб ячейки");
        if(hpImage == null)
            Debug.LogError("Для панели горячих клавиш не указана ссылка на элемент здоровья игрока");
        if(kickHotkeyCell == null)
            Debug.LogError("Для панели горячих клавиш не указана ссылка на хоткей пинка");
        if(punchHotkeyCell == null)
            Debug.LogError("Для панели горячих клавиш не указана ссылка на хоткей удара");
        if(runHotkeyCell == null)
            Debug.LogError("Для панели горячих клавиш не указана ссылка на хоткей бега");
        if(jumpHotkeyCell == null)
            Debug.LogError("Для панели горячих клавиш не указана ссылка на хоткей прыжка");
        
        
        Player.Current.Input.PlayerController.Kick.started += kickHotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Punch.started += punchHotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Run.started += runHotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Jump.started += jumpHotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Ultimate1.started += ult1HotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Ultimate2.started += ult2HotkeyCell.BindedActionForInputSystem;

        ObserverWithoutData.Sub(Events.LargeUltimateActivated, ult1HotkeyCell.BindedAction);
        ObserverWithData<CooldownObservingDTO>.Sub(Events.LargeUltimateActivated, ult1HotkeyCell.SetOnCooldown);
        
        ObserverWithoutData.Sub(Events.DanceUltimateActivated, ult2HotkeyCell.BindedAction);
        ObserverWithData<CooldownObservingDTO>.Sub(Events.DanceUltimateActivated, ult2HotkeyCell.SetOnCooldown);
        
        energyPoints = GetComponentInChildren<EnergySpheresContainer>();
    }

    private void UnsubscribeHotkeys()
    {
        hotkeysCells.ForEach(h => h.Unsubscribe());
        
        if (Player.Current == null)
            return;
            
        Player.Current.Input.PlayerController.Kick.started -= kickHotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Punch.started -= punchHotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Run.started -= runHotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Jump.started -= jumpHotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Ultimate1.started -= ult1HotkeyCell.BindedActionForInputSystem;
        Player.Current.Input.PlayerController.Ultimate2.started -= ult2HotkeyCell.BindedActionForInputSystem;
    }

    private void OnDestroy()
    {
        UnsubscribeHotkeys();
    }
    
    public void ChangeSpheresAmount(int spheresAmount)
    {
        energyPoints.SetShownPointsAmount(spheresAmount);
    }
}
