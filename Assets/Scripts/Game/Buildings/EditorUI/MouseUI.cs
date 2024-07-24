using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OilCity {
    public class MouseUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        
        private CanvasMonitor canvasMonitor = null;

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        private void Start() {
            canvasMonitor = GetComponentInParent<CanvasMonitor>();
        }
    }
}