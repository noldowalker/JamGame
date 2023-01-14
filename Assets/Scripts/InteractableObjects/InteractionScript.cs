using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionScript : MonoBehaviour, IKickable
{
    public static UnityEvent<float> hitEvent = new UnityEvent<float>();
    [SerializeField] private Transform pfDestroyDetails;

    [SerializeField] private Transform pfVFXsmoke;


   // private bool playerInTrigger;
    private Rigidbody rigidbody;
    private AudioSource audioSource;
   // Player player;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
       // player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
      //  Player.kickEvent.AddListener(onKick);
    }


    private void OnTriggerEnter(Collider other)
    {
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
                    Instantiate(pfVFXsmoke,transform.position, transform.rotation);
                    SoundHandleScript.Current.PlaySound(SoundEnum.ITEM_DESTRUCTION, audioSource);
                    Destroy(gameObject);
                }
                
                hitEvent.Invoke(hitForce);
            }
        }
    }

    public void Kick(float damage, float force, Vector3 direction)
    {
        rigidbody.AddForce((transform.position - direction) * force);
    }
}
