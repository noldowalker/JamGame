using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [SerializeField] private float healPoints;

  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IHealable healable = other.GetComponent<IHealable>();
            if (healable != null)
            {
                healable.Heal(healPoints);
                Destroy(gameObject);
            }
        }
        
    }

}
