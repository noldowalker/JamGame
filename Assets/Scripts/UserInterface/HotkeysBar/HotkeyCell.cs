using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UserInterface.Enums;
using UserInterface.HotkeysBar;

public class HotkeyCell : MonoBehaviour
{
    [SerializeField]
    private Image border;
    [SerializeField]
    private Image background;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI hotkeyCharTextField;
    [SerializeField]
    private CooldownDisplayComponent cooldownComponent;
    
    [SerializeField]
    private Sprite backgroundInactiveSprite;
    [SerializeField]
    private Sprite backgroundReadySprite;
    [SerializeField]
    private Sprite backgroundOnActivationSprite;
    
    private Coroutine _activatedAnimation;
    private Coroutine _cooldownAnimation;
    private Events _subscribedEvent;
    
    void Start()
    {
        SetBackground(backgroundInactiveSprite);
    }

    void Update()
    {
        
    }

    public void SetBorder(Sprite sprite) => border.sprite = sprite;
    public void SetBackground(Sprite sprite) => background.sprite = sprite;
    public void SetIcon(Sprite sprite) => icon.sprite = sprite;
    public void SetNewHotkey(string hotkey) => hotkeyCharTextField.text = hotkey;
    public void SetEventSubscription(Events newEvent)
    {
        _subscribedEvent = newEvent;
        Unsubscribe();
        ObserverWithoutData.Sub(newEvent, BindedAction);
        ObserverWithData<CooldownObservingDTO>.Sub(newEvent, SetOnCooldown);
    }

    public void Unsubscribe()
    {
        if (_subscribedEvent == null)
            return;
        
        ObserverWithoutData.Unsub(_subscribedEvent, BindedAction);
        ObserverWithData<CooldownObservingDTO>.Unsub(_subscribedEvent, SetOnCooldown);
    }

    public void BindedAction()
    {
        if (_activatedAnimation != null)
            return;
        if (_cooldownAnimation != null)
            return;
        
        _activatedAnimation = StartCoroutine(PlayActivationAnimation());
    }

    public void SetOnCooldown(CooldownObservingDTO cooldownData)
    {
        if (_cooldownAnimation != null)
            return;
        
        var cooldownTime = cooldownData.InitialCooldownValue;
        _cooldownAnimation = StartCoroutine(PlayCooldownAnimation(cooldownTime));
    }

    public void CooldownRemove()
    {
        if (_cooldownAnimation == null)
            return;
            
        StopCoroutine(_cooldownAnimation);
        cooldownComponent.HideCooldown();
        SetBackground(backgroundReadySprite);
    }

    public void ActivatedRemove()
    {
        if (_activatedAnimation != null)
            StopCoroutine(_activatedAnimation);
    }

    public void AlignToCoords(float x, float y, Align align)
    {
        var rectTransform = GetComponent<RectTransform>();

        switch (align)
        {
            case Align.LeftBottom:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 0f);
                break;
        }
    }

    private IEnumerator PlayActivationAnimation()
    {
        Debug.Log(@$"Нажата кнопка {hotkeyCharTextField.text}");
        SetBackground(backgroundOnActivationSprite);
        yield return new WaitForSeconds(0.1f);
        SetBackground(backgroundReadySprite);
        _activatedAnimation = null;
    }

    private IEnumerator PlayCooldownAnimation(float cooldownTime)
    {
        Debug.Log(@$"Кнопка {hotkeyCharTextField.text} ушла в кулдаун");
        SetBackground(backgroundInactiveSprite);
        cooldownComponent.ShowCooldown(cooldownTime);   
        
        var timeLeft = cooldownTime;
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
            cooldownComponent.UpdateTimerValue((float)Math.Round(timeLeft));
        }
        
        cooldownComponent.HideCooldown();
        SetBackground(backgroundReadySprite);
        
        _cooldownAnimation = null;
    }

    private void OnDestroy()
    {
        CooldownRemove();
        ActivatedRemove();
    }
    
    
}
