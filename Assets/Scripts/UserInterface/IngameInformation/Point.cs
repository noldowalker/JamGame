using System;
using UnityEngine;

namespace UserInterface.IngameInformation
{
    public class Point : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}