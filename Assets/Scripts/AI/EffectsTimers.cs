using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsTimers : MonoBehaviour
{
    private float [] effectsTimer = new float[1];
    private bool[] isEffectTimerOn = { false };
    
    void Update()
    {
        //for () //если будут еще какие либо другие эффекты на врагах
        if (isEffectTimerOn[0])
        {
            if (effectsTimer[0] < 0)
            {
                this.GetComponent<EnemyAIControllerScript>().Dance(false, -1.0f);
                isEffectTimerOn[0] = false;
                
                //Debug.Log($"Duratation for {i} ultimate is up");
            }
            else effectsTimer[0] -= Time.deltaTime;
        }
    }

    public void SetEffectTimer(int effect, float duratation)
    {
        effectsTimer[effect] = duratation;
        isEffectTimerOn[effect] = true;
    }
}
