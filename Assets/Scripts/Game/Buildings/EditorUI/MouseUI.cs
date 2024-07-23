using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OilCity {
    public class MouseUI : MonoBehaviour{
        
        private CanvasMonitor canvasMonitor = null;

        private void Start() {
            canvasMonitor = GetComponentInParent<CanvasMonitor>();
        }
    }
}