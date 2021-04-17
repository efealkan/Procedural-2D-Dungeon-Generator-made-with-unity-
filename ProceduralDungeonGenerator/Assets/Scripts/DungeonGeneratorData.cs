using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorData : MonoBehaviour
{
    public Vector2Int numberOfRoomsRange;
    
    [Min(2)] public int roomDistanceSample;
    [Min(6)] public int averageRoomSize;
    [Range(2, 5)] public int corridorWidth;
}
