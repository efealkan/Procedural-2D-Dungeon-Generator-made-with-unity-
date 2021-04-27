using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorData : MonoBehaviour
{
    [Range(2, 100)] public int numberOfRoomsMin;
    [Range(2, 100)] public int numberOfRoomsMax;

    [Min(2)] public int roomDistanceSample;
    [Min(6)] public int averageRoomSize;
    [Range(2, 5)] public int corridorWidth;

    [Range(0, 10)] public int oldnessLevel;
}
