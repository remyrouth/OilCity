using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupValuesPool : Singleton<PopupValuesPool>
{
    private const int MAX_POOL_SIZE = 10;
    private readonly Dictionary<PopupValueType, Queue<SinglePopupValue>> pool = new();
    [SerializeField] private GameObject[] _prefabs;

    public T GetFromPool<T>(PopupValueType type)
    where T : SinglePopupValue
    {
        if (!pool.ContainsKey(type))
            pool.Add(type, new Queue<SinglePopupValue>());
        var queue = pool[type];
        if (queue.Count == 0)
        {
            var newObj = Instantiate(_prefabs.First(e => e.GetComponent<SinglePopupValue>().type == type));
            queue.Enqueue(newObj.GetComponent<T>());
        }
        if (queue.Peek().type != type)
        {
            Debug.LogError("Different popup types!");
            return null;
        }
        return queue.Dequeue() as T;
    }

    public void GiveAwayToPool(SinglePopupValue popup)
    {
        popup.gameObject.SetActive(false);
        if (!pool.ContainsKey(popup.type))
            pool.Add(popup.type, new Queue<SinglePopupValue>());
        if (pool[popup.type].Count >= MAX_POOL_SIZE)
            Destroy(popup.gameObject);
        else
            pool[popup.type].Enqueue(popup);
    }

    public enum PopupValueType { OilMade, KeroseneMade, PaidMoney, EarnedMoney, MadPeople }
}
