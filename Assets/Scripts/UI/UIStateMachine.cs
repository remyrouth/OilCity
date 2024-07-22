using UnityEngine;

namespace UI
{
    public class UIStateMachine : Singleton<UIStateMachine>
    {
        [SerializeField] private GameObject[] availableStates;
        private GameObject _currentState;

        public void ChangeState(int stateIndex)
        {
            
        }
    }
}