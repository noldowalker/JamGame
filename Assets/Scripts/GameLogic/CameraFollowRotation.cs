using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowRotation : MonoBehaviour
{
    public GameObject player;
    public float RotationSpeed;

    private Quaternion _lookRotation;
    private Vector3 _direction;

    void Update()
    {
        Transform target = player.transform;
        //find the vector pointing from our position to the target
        _direction = (target.position - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation(_direction);

        //rotate us over time according to speed until we are in the required rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
    }
}
