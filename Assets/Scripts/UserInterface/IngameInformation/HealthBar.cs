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

    private float _currentHealthPercent;
    
    void Start()
    {
        _currentHealthPercent = 1f;
        ObserverWithData<NewHPPercentObservingDTO>.Sub(Events.HPLossPercent, ChangeHealthPercentByEvent);
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void ChangeHealthPercent(float newPercent)
    {
       // Debug.Log($@"New percent = {newPercent}");
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
    }

    public void ChangeHealthPercentByEvent(NewHPPercentObservingDTO data)
    {
        ChangeHealthPercent(data.NewPercent);
    }
}
