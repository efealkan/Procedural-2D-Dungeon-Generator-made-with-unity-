using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tilebase;
    
    private DungeonGeneratorData GeneratorData;
    
    private int numberOfRooms;
    
    private List<Room> rooms = new List<Room>();

    private void Awake()
    {
        GeneratorData = GetComponent<DungeonGeneratorData>();
        numberOfRooms = Random.Range(GeneratorData.numberOfRoomsRange.x, GeneratorData.numberOfRoomsRange.y);

        GenerateRoomCentrePoints();
    }

    private void GenerateRoomCentrePoints()
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            var centre = GetRandomPointInCircle();
            var roomSizeX = RandomInt(GeneratorData.roomSizeXRange.x, GeneratorData.roomSizeXRange.y);
            var roomSizeY = RandomInt(GeneratorData.roomSizeYRange.x, GeneratorData.roomSizeYRange.y);

            Room room = new Room(i, centre, new Vector2Int(roomSizeX, roomSizeY));
            rooms.Add(room);
            DrawRoom(room);
        }
    } 

    private Vector2Int GetRandomPointInCircle()
    {
        float radius = GeneratorData.radius;
        float angle = RandomFloat() * Mathf.PI * 2;
        float rad = (float) Math.Sqrt(RandomFloat()) * radius;
        float x = rad * Mathf.Cos(angle);
        float y = rad * Mathf.Sin(angle);
        return new Vector2Int((int)x, (int)(y/1.5));
    }

    private void DrawRoom(Room room)
    {
        Vector2Int centre = room.roomCentrePos;
        Vector2Int size = room.size;

        int topLeftPosX = centre.x - size.x / 2;
        int topLeftPosY = centre.y + size.y / 2;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int pos = new Vector3Int(topLeftPosX + x, topLeftPosY + y, 0);
                tilemap.SetTile(pos, tilebase);
            }
        }
    }

    private float RandomFloat(float min = 0f, float max = 1f) => Random.Range(min, max);
    private int RandomInt(float min, float max) => (int) Random.Range(min, max);
}
