using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : Singleton<AnimalManager>
{
    [SerializeField] private GameObject[] _prefabs;
    private readonly List<GameObject> _animals = new();
    private const int START_ANIMALS_NUMBER = 50;
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = new Vector3(BoardManager.MAP_SIZE_X / 2, BoardManager.MAP_SIZE_Y * 2 / 3, 0);
            pos += (Random.insideUnitCircle * BoardManager.MAP_SIZE_X / 3).ToVector3();
            var obj = Instantiate(_prefabs[Random.Range(0, _prefabs.Length)], transform);
            obj.transform.position = pos;
            _animals.Add(obj);
        }
        PollutionManager.Instance.OnPollutionChanged += PollutionChanged;
    }
    private void PollutionChanged(float newValue)
    {
        float animalsPercentage = _animals.Count / START_ANIMALS_NUMBER;
        if (1 - newValue < animalsPercentage && _animals.Count > 0)
            Destroy(_animals[0]);
    }
}
