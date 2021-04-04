using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorData : MonoBehaviour
{
    public Vector2Int numberOfRoomsRange;
    [Min(6)]
    public int averageRoomSize;
    public int roomDistanceSample;
}
