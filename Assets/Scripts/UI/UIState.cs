using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class UIState : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        
        public abstract GameState GetGameState();
    } 
}


