using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [HideInInspector] public static DungeonGenerator instance;
    
    private DungeonGeneratorData GeneratorData;
    private DrawComponents DrawHandler;

    private Basemap basemap;
    
    private List<Chunk> chunks = new List<Chunk>();
    private List<Room> rooms = new List<Room>();
    private List<Corridor> corridors = new List<Corridor>();
    
    private HashSet<Vector2Int> groundTilePositions = new HashSet<Vector2Int>();

    private Room entrance;
    private Room exit;
    
    private Vector2Int roomSizeRange;
    
    private int numberOfRooms;

    private bool has_entrance = true;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
        
        GeneratorData = GetComponent<DungeonGeneratorData>();
        DrawHandler = GetComponent<DrawComponents>();

        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        ResetDungeon();
        
        if (GeneratorData.numberOfRoomsMin > GeneratorData.numberOfRoomsMax)
        {
            Debug.LogError("Number Rooms Min IS BIGGER than Number Room MAX: ERROR!");
            return;
        }
        numberOfRooms = Utils.RandomInt(GeneratorData.numberOfRoomsMin, GeneratorData.numberOfRoomsMax);
        
        roomSizeRange = new Vector2Int(GeneratorData.averageRoomSize - 2, GeneratorData.averageRoomSize + 5);
        
        GenerateBasemap();
        GenerateChunks();
        GenerateRooms();
        GenerateCorridors();
        GenerateDungeonEntrance();
        DrawCall();
    }

    public void SetDungeonEntrance(bool boolean)
    {
        has_entrance = boolean;

        if (has_entrance)
        {
            DrawHandler.DrawEntrance(entrance);
            DrawHandler.DrawEntrance(exit);
        }
        else
        {
            DrawHandler.RemoveEntrance(entrance);
            DrawHandler.RemoveEntrance(exit);
        }
    } 
    
    private void ResetDungeon()
    {
        chunks = new List<Chunk>();
        rooms = new List<Room>();
        corridors = new List<Corridor>();
        groundTilePositions = new HashSet<Vector2Int>();
        
        DrawHandler.ClearAllTiles();
        MST.Clear();
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
                Debug.Log("Generating Chunks");

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
        List<Chunk> availableChunks = chunks.ToList();

        for (int i = 0; i < numberOfRooms; i++)
        {
            Debug.Log("Generating Rooms");

            int index = Utils.RandomInt(0, availableChunks.Count);
            Chunk chunk = availableChunks[index];
            availableChunks.RemoveAt(index);
            
            int distanceSample = Mathf.CeilToInt((float) GeneratorData.roomDistanceSample / 2);

            int roomSizeX = Utils.RandomInt(roomSizeRange.x, roomSizeRange.y);
            int roomSizeY = Utils.RandomInt(roomSizeRange.x, roomSizeX);
            
            //Make the size odd
            if (roomSizeX % 2 == 0) roomSizeX += 1;
            if (roomSizeY % 2 == 0) roomSizeY += 1;

            int roomSizeXHalf = Mathf.CeilToInt((float) roomSizeX / 2) + distanceSample;
            int roomSizeYHalf = Mathf.CeilToInt((float) roomSizeY / 2) + distanceSample;
            
            int xLeft = chunk.topLeftPos.x + roomSizeXHalf;
            int xRight = chunk.topRightPos.x - roomSizeXHalf;
            int yTop = chunk.topLeftPos.y - roomSizeYHalf;
            int yBot = chunk.bottomLeftPos.y + roomSizeYHalf;

            int centreX = Utils.RandomInt(xLeft, xRight);
            int centreY = Utils.RandomInt(yBot, yTop);
            
            Room room = new Room(i, new Vector2Int(centreX, centreY), new Vector2Int(roomSizeX, roomSizeY));
            rooms.Add(room);
            chunk.SetRoom(room);
            
            //Collect all ground tiles here!
            foreach (var pos in room.roomTiles)
            {
                groundTilePositions.Add(pos);
            }
        }
    }

    private void GenerateCorridors()
    {
        corridors = MST.CreateCorridors(rooms, chunks[0].size);
        DrawHandler.SetupCorridorRays(corridors);

        foreach (var corridor in corridors)
        {
            corridor.SetCorridorWidth(GeneratorData.corridorWidth);
            corridor.CreateActualCorridor();
            
            //Collect all ground tiles here!
            foreach (var pos in corridor.corridorTiles)
            {
                groundTilePositions.Add(pos);
            }
        }
    }

    private void GenerateDungeonEntrance()
    {
        entrance = rooms[Random.Range(0, rooms.Count)];

        int maxDist = 0;
        foreach (var room in rooms)
        {
            if (room.Equals(entrance)) continue;
            int dist = Utils.CalculateDistance(entrance, room);
            if (dist > maxDist)
            {
                maxDist = dist;
                exit = room;
            }
        }
    }

    private void DrawCall()
    {
        DrawHandler.Draw(groundTilePositions, rooms);

        if (has_entrance)
        {
            DrawHandler.DrawEntrance(entrance);
            DrawHandler.DrawEntrance(exit);
        }
    }
}
