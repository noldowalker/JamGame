using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateTimers : MonoBehaviour
{
    //private MonoBehaviour player;
    
    [SerializeField] private float[] cooldowns = new float[2];
    [SerializeField] private float[] duratations = new float[2];

    private float[] curCooldowns = new float[2];
    private float[] curDuratations = new float[2];
    private bool[] isCooldownTimerOn = new bool[]{false, false};
    private bool[] isDuratationTimerOn = new bool[]{false, false};

    private void Start()
    {
        //player = this.transform.GetComponent<Player>();
    }

    void FixedUpdate()
    {
        for (byte i = 0; i < 2; i++)
        {
            if (isDuratationTimerOn[i])
            {
                if (curDuratations[i] < 0)
                {
                    this.GetComponent<Player>().UltimateEffect(i, false);
                    isDuratationTimerOn[i] = false;
                    Debug.Log($"Duratation for {i} ultimate is up");
                }
                else curDuratations[i] -= Time.deltaTime;
            }
            if (isCooldownTimerOn[i])
            {
                if (curCooldowns[i] < 0)
                {
                    //this.GetComponent<Player>().ultimateUsage[i] = true;
                    isCooldownTimerOn[i] = false;
                    Debug.Log($"Cooldown for {i} ultimate is up");
                }
                else curCooldowns[i] -= Time.deltaTime;
            }
        }
    }

    public void SetUltimateTimer(byte num)
    {
        //this.GetComponent<Player>().ultimateUsage[num] = false;
        curCooldowns[num] = cooldowns[num];
        curDuratations[num] = duratations[num];
        isCooldownTimerOn[num] = true;
        isDuratationTimerOn[num] = true;
    }
}
