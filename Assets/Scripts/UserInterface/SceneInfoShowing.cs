using System;
using System.Collections;
using GameLogic;
using UnityEngine;

namespace UserInterface
{
    public class SceneInfoShowing : MonoBehaviour
    {
        [SerializeField] private bool callHelpOnLoad = false;

        private void Start()
        {
            if (callHelpOnLoad)
                StartCoroutine(showAfterDelay(1f));
        }

        private IEnumerator showAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ObserverWithoutData.FireEvent(Events.HelpPanelCalled);
        }
    }
}