using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{ 
    public static int RandomInt(float min, float max) => (int) Random.Range(min, max);
    
    public static Vector3 Vector2IntToVector3(Vector2Int v) => new Vector3(v.x, v.y, 0);
    public static Vector3Int Vector2IntToVector3Int(Vector2Int v) => new Vector3Int(v.x, v.y, 0);
    public static Vector2Int AddNumberToV2Int(Vector2Int v, int x, int y) => new Vector2Int(v.x + x, v.y + y);
}
