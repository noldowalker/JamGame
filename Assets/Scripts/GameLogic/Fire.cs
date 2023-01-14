using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private float fireDamage;

    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamagable damagable = collision.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.Damage(fireDamage);
            }
        }
    }
}
