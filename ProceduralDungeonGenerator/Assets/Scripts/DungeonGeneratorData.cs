using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorData : MonoBehaviour
{
    [Range(2, 100)] public int numberOfRoomsMin;
    [Range(2, 100)] public int numberOfRoomsMax;

    [Range(2, 15)] public int roomDistanceSample;
    [Range(6, 50)] public int averageRoomSize;
    [Range(2, 5)] public int corridorWidth;

    [Range(0, 10)] public int oldnessLevel;
}
