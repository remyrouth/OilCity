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
            if (PlayerPrefs.HasKey("CameraInversion"))
            {
                cameraInversionToggle.isOn = PlayerPrefs.GetInt("CameraInversion") == 1;
            }
        }

        public void ToggleCameraMovementInversion()
        {
            CameraController.Instance.invert = cameraInversionToggle.isOn;
            PlayerPrefs.SetInt("CameraInversion", CameraController.Instance.invert ? 1 : 0);
        }
    }
}
