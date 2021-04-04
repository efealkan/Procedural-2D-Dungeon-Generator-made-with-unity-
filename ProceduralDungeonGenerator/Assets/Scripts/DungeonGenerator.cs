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

    private Basemap basemap;
    private List<Chunk> chunks = new List<Chunk>();
    private List<Room> rooms = new List<Room>();
    
    private Vector2Int roomSizeRange;
    private int numberOfRooms;

    private void Awake()
    {
        GeneratorData = GetComponent<DungeonGeneratorData>();
        numberOfRooms = RandomInt(GeneratorData.numberOfRoomsRange.x, GeneratorData.numberOfRoomsRange.y);
        roomSizeRange = new Vector2Int(GeneratorData.averageRoomSize - 5, GeneratorData.averageRoomSize + 5);

        GenerateBasemap();
        GenerateChunks();
        GenerateRooms();
    }

    private void GenerateBasemap()
    {
        int numberRows = 1;
        int numberCols = numberOfRooms;

        if (numberOfRooms != 1)
        {
            do {
                numberCols = Mathf.CeilToInt((float)numberCols/2);
                numberRows *= 2;
            } while (numberRows * 2 < numberCols);
        }

        Debug.Log("Number of rooms: " + numberOfRooms);
        Debug.Log("Rows: " + numberRows + " Cols: " + numberCols);

        int sizeX = numberCols * (roomSizeRange.y + GeneratorData.roomDistanceSample);
        int sizeY = numberRows * (roomSizeRange.y + GeneratorData.roomDistanceSample);

        Debug.Log("Basemap size x: " + sizeX + " Basemap size y: " + sizeY);

        basemap = new Basemap(numberRows, numberCols, new Vector2Int(sizeX, sizeY));
    }

    private void GenerateChunks()
    {
        int chunkSizeX = Mathf.CeilToInt((float)basemap.size.x / basemap.numberOfCols);
        int chunkSizeY = Mathf.CeilToInt((float)basemap.size.y / basemap.numberOfRows);
        
        Debug.Log("Chunk size x: " + chunkSizeX + " Chunk size y: " + chunkSizeY);
        
        int curY = 0;
        int count = 0;
        
        for (int r = 0; r < basemap.numberOfRows; r++)
        {
            if (r != 0) curY -= chunkSizeY;
            int curX = 0;
            
            for (int c = 0; c < basemap.numberOfCols; c++)
            {
                if (c != 0) curX += chunkSizeX;
                
                Vector2Int topLeft = new Vector2Int(curX, curY);
                Vector2Int topRight = new Vector2Int(curX + chunkSizeX, curY);
                Vector2Int bottomLeft = new Vector2Int(curX, curY - chunkSizeY);
                
                Chunk chunk = new Chunk(count, new Vector2Int(chunkSizeX, chunkSizeY), topLeft, topRight, bottomLeft);
                chunks.Add(chunk);
                count++;
            }
        }
    }

    private void GenerateRooms()
    {
        List<Chunk> availableChunks = chunks;
        
        for (int i = 0; i < numberOfRooms; i++)
        {
            int index = RandomInt(0, availableChunks.Count);
            Chunk chunk = availableChunks[index];
            chunks.RemoveAt(index);

            int roomSizeX = RandomInt(roomSizeRange.x, roomSizeRange.y);
            int roomSizeY = RandomInt(roomSizeRange.x, roomSizeRange.y);

            int roomSizeXHalf = Mathf.CeilToInt((float) roomSizeX / 2);
            int roomSizeYHalf = Mathf.CeilToInt((float) roomSizeY / 2);

            int xLeft = chunk.topLeftPos.x + roomSizeXHalf;
            int xRight = chunk.topRightPos.x - roomSizeXHalf;
            int yTop = chunk.topLeftPos.y - roomSizeYHalf;
            int yBot = chunk.bottomLeftPos.y + roomSizeYHalf;

            int centreX = RandomInt(xLeft, xRight);
            int centreY = RandomInt(yBot, yTop);
            
            Room room = new Room(i, new Vector2Int(centreX, centreY), new Vector2Int(roomSizeX, roomSizeY));
            rooms.Add(room);
            chunk.SetRoom(room);
            
            DrawRoom(room);
        }
    }

    private void DrawRoom(Room room)
    {
        Vector2Int centre = room.centrePos;
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

    private Vector3Int Vector2IntTo3(Vector2Int v) => new Vector3Int(v.x, v.y, 0);
    private float RandomFloat(float min = 0f, float max = 1f) => Random.Range(min, max);
    private int RandomInt(float min, float max) => (int) Random.Range(min, max);
}
