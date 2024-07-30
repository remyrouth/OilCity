using System.Collections;
using UnityEngine;

namespace Game.Tutorial
{
    public class TutorialSpawnTutorial : TutorialStep
    {
        private bool _canContinue;
        private new void OnEnable()
        {
            base.OnEnable();
            _canContinue = false;
            TimeManager.Instance.TicksPerMinute = 60;
            StartCoroutine(Wait());
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(5);
            _canContinue = true;
        }

        private void Update()
        {
            if (_canContinue && Input.GetMouseButtonDown(0))
            {
                FinishStep();
            }
        }
    }
}