using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionScript : MonoBehaviour, IKickable
{
    public static UnityEvent<float> hitEvent = new UnityEvent<float>();

    [SerializeField] private Transform pfDestroyDetails;
   // private bool playerInTrigger;
    private Rigidbody rigidbody;
   // Player player;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
       // player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
      //  Player.kickEvent.AddListener(onKick);
    }

    void Update()
    {
    }

    //private void onKick(float force)
    //{
    //    if (playerInTrigger)
    //    {
    //        rigidbody.AddForce((transform.position-player.transform.position)*force);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag.Equals("HitArea"))
        //{ 
        //    playerInTrigger = true;
        //}
        if (other.CompareTag("Enemy"))
        {           
            float hitForce = Vector3.Magnitude(rigidbody.velocity);

            if (hitForce > 10)
            {

                IKickable kickable = other.GetComponent<IKickable>();
                if (kickable != null)
                {
                    kickable.Kick(hitForce, hitForce, transform.position);
                    if (pfDestroyDetails != null)
                    {
                      Transform brokenDetails =  Instantiate(pfDestroyDetails, transform.position, transform.rotation);
                        foreach (Transform child in brokenDetails)
                        {
                            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
                            {
                                childRigidbody.AddExplosionForce(hitForce, transform.position, 5f);
                            }
                        }
                    }

                    Destroy(gameObject);
                }
                
                hitEvent.Invoke(hitForce);
            }
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.tag.Equals("HitArea"))
    //    {
    //        playerInTrigger = false;
    //    }
    //}

    public void Kick(float damage, float force, Vector3 direction)
    {
        rigidbody.AddForce((transform.position - direction) * force);
    }
}
