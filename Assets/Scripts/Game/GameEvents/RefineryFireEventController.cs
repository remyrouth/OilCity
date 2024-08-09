using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RefineryFireEventController : SingleEventController
{
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private int ticksToStop;

    protected void OnEnable()
    {
        if (!BoardManager.Instance.tileDictionary.Values.Any(e => e is RefineryController))
            return;
        var refineries = BoardManager.Instance.tileDictionary.Values.OfType<RefineryController>().ToList();
        var random = refineries[Random.Range(0, refineries.Count)];
        random.StopWorkingTimer = ticksToStop;
        Instantiate(firePrefab, random.Anchor.ToVector3(), Quaternion.identity)
            .GetComponent<RefineryFireEffect>()
            .Initialize(8);
        CameraController.Instance.TargetPosition = random.Anchor.ToVector3() 
            + Vector3.forward * CameraController.Instance.TargetPosition.z;
    }
}
