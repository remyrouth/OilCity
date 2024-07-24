using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OilCity {
    public class CanvasMonitor : MonoBehaviour {

        // In case we want more "tabs" or options other than selecting buildings
        public enum ToolboxPanels {
            BuildingsPanel
        }

        public bool isPointerOverGUI { get; set; }

        [SerializeField] private CanvasGroup BuildingsPanel = null;

        private ToolboxPanels activePanel = ToolboxPanels.BuildingsPanel;

        public ToolboxPanels GetActivePanel() {
            return activePanel;
        }

        public void SetActivePanel(ToolboxPanels newActivePanel) {
            BuildingsPanel.gameObject.SetActive(false);

            switch (newActivePanel) {
                case ToolboxPanels.BuildingsPanel:
                BuildingsPanel.gameObject.SetActive(true);
                    break;
            }
        }
        
    }
}