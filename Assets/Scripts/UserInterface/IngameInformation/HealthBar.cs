using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer barBackground;
    [SerializeField]
    private SpriteRenderer barFill;
    [SerializeField]
    private SpriteRenderer barBorder;
    [SerializeField]
    private Color highHpColor;
    [SerializeField]
    private Color middleHpColor;
    [SerializeField]
    private Color lowHpColor;

    private float _currentHealthPercent;
    private bool _isHit;
    private Coroutine _showBarCoroutine;
    
    void Start()
    {
        _currentHealthPercent = 1f;
        barFill.color = highHpColor;
        _isHit = false;
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void ChangeHealthPercent(float newPercent)
    {
        if (newPercent > 1f)
            newPercent = 1f;
        
        if (newPercent < 0f)
            newPercent = 0f;

        _currentHealthPercent = newPercent;

        barFill.transform.localScale = new Vector3(
            newPercent,
            barFill.transform.localScale.y,
            barFill.transform.localScale.z
        );

        barFill.color = newPercent switch
        {
            > .75f => highHpColor,
            > .35f => middleHpColor,
            _ => lowHpColor
        };

        _isHit = true;
        if (_showBarCoroutine == null)
            _showBarCoroutine = StartCoroutine(ShowHpBar(3f));
    }

    private IEnumerator ShowHpBar(float showtime)
    {
        SetVisible(true);
        
        do {
            _isHit = false;
            yield return new WaitForSeconds(showtime);
        } while (_isHit);

        SetVisible(false);
    }

    private void SetVisible(bool isVisible)
    {
        barBackground.gameObject.SetActive(isVisible);
        barFill.gameObject.SetActive(isVisible);
        barBorder.gameObject.SetActive(isVisible);
    }

    private void OnDestroy()
    {
        if (_showBarCoroutine != null)
            StopCoroutine(_showBarCoroutine);
    }
}
