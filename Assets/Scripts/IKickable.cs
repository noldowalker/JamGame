using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKickable 
{
    void Kick(float damage, float force, Vector3 direction);

}