using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionScript : MonoBehaviour
{
    public static UnityEvent<float> hitEvent = new UnityEvent<float>();
    
    private bool playerInTrigger;
    private Rigidbody rigidbody;
    Player player;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Player.kickEvent.AddListener(onKick);
    }

    void Update()
    {
    }

    private void onKick(float force)
    {
        if (playerInTrigger)
        {
            rigidbody.AddForce((transform.position-player.transform.position)*force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("HitArea"))
        { 
            playerInTrigger = true;
        }
        if (other.tag.Equals("Enemy"))
        {
            float hitForce = Vector3.Magnitude(rigidbody.velocity);
            print(hitForce);
            if (hitForce > 10)
            {
                print("HIT!");
                hitEvent.Invoke(hitForce);
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals("HitArea"))
        {
            playerInTrigger = false;
        }
    }
}
