using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealObject : MonoBehaviour, IKickable, IPunchable
{

    [SerializeField] private Transform pfDestroyDetails;
    [SerializeField] private Transform pfVFXsmoke;
    [SerializeField] private Transform healObject;

    private float explosionForce;
    private Rigidbody rigidbody;

    IHealable healable;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Kick(float damage, float force, Vector3 direction)
    {
        explosionForce = damage;

        Heal();
    }

    public void Punch(float damage)
    {
        explosionForce = damage;
        Heal();
    }


    private void Heal()
    {
        if (pfDestroyDetails != null)
        {
            Transform brokenDetails = Instantiate(pfDestroyDetails, transform.position, transform.rotation);
            foreach (Transform child in brokenDetails)
            {
                if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
                {
                    childRigidbody.AddExplosionForce(explosionForce, transform.position, 5f);
                }
            }
        }
        Instantiate(pfVFXsmoke, transform.position, transform.rotation);
        Instantiate(healObject, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
