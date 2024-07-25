using Game.Managers;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;


namespace UIScripts.Settings
{
    public class CameraInversionToggle : MonoBehaviour
    {
        [SerializeField] private Toggle cameraInversionToggle;
        
        public void OnEnable()
        {
            if (CameraController.Instance.invert)
            {
                cameraInversionToggle.isOn = true;
                CameraController.Instance.invert = true;
            }
        }

        public void ToggleCameraMovementInversion()
        {
            SettingsManager.Instance.ToggleCameraMovementInversion();
        }
    }
}
