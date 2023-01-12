using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.HotkeysBar
{
    public class EnergySphere: MonoBehaviour
    {
        public float lastDegree;

        [SerializeField] 
        private Image sphereImage;
        
        private void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}