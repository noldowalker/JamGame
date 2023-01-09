using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    private CharacterController controller;
    private Quaternion currentRotation = new Quaternion();
    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        Vector3 velocity = controller.velocity;
        
        if (velocity != Vector3.zero)
        {
            currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(velocity).eulerAngles.y, 0);
        } else
        {
            transform.rotation = currentRotation;
        }
    }
}
