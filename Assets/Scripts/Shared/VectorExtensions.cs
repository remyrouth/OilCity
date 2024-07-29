using UnityEngine;

public static class VectorExtensions 
{
    public static Vector3 ToVector3(this Vector2Int vector) => new Vector3(vector.x, vector.y, 0);
    public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.x, vector.y, 0);
}
