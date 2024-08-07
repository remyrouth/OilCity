using UnityEngine;
using System.Collections; // Add this for IEnumerator


public class EndingUI : UIState
{
    [SerializeField] private SingleGraphView[] _graphs;
    public override GameState type => GameState.EndingUI;




    private Vector3 startPosition;

    public override void OnEnter()
    {
        base.OnEnter();
        foreach (var graph in _graphs)
            graph.PopulateGraph();
        TimeManager.Instance.TicksPerMinute = 0;
        //StartCoroutine(GameRecorder.Instance.DoRollback());
        startPosition = transform.position;
        transform.position = new Vector3(startPosition.x, startPosition.y + 800f, startPosition.z);
        StartCoroutine(MoveToPosition(startPosition, 2f));
    }


    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 initialPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is set exactly to the target position
        transform.position = targetPosition;
    }
}
