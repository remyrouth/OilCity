using UnityEngine;

public class AnimalManager : Singleton<AnimalManager>
{
    [SerializeField] private GameObject[] _prefabs;


    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(_prefabs[Random.Range(0, _prefabs.Length)]
                , new Vector3(BoardManager.MAP_SIZE_X / 2, BoardManager.MAP_SIZE_Y * 2 / 3, 0)
                , Quaternion.identity);
        }
    }




}
