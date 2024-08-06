using UnityEngine;
// This script was made before "Poland In Pixels" project
// Original author: Piotr Trze�niowski
public class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance) return _instance;
            return _instance = FindObjectOfType<T>();
        }
    }
}
