using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface.HotkeysBar
{
    public class CooldownDisplayComponent : MonoBehaviour
    {
        [SerializeField]
        private Image background;
        [SerializeField]
        private TextMeshProUGUI cooldownTextField;

        public void ShowCooldown(float initialTimerValue)
        {
            background.gameObject.SetActive(true);
            cooldownTextField.gameObject.SetActive(true);
            cooldownTextField.text = initialTimerValue.ToString();
        }

        public void UpdateTimerValue(float newTimerValue)
        {
            cooldownTextField.text = (newTimerValue <= 0) ? "" : newTimerValue.ToString();
        }

        public void HideCooldown()
        {
            cooldownTextField.text = "";
            cooldownTextField.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
        }
    }
}