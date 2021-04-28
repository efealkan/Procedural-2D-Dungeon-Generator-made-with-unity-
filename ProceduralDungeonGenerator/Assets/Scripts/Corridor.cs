using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Corridor
{
    public Room room1;
    public Room room2;

    public int distanceBetweenRooms;
    public int corridorWidth;

    public Vector2Int topLeft;
    public Vector2Int topRight;
    public Vector2Int botLeft;
    public Vector2Int botRight;
    public Vector2Int midTopLeft;
    public Vector2Int midBotLeft;
    public Vector2Int midTopRight;
    public Vector2Int midBot;
    
    public HashSet<Vector2Int> corridorTiles = new HashSet<Vector2Int>();

    public Corridor(Room room1, Room room2, int distanceBetweenRooms)
    {
        this.room1 = room1;
        this.room2 = room2;
        this.distanceBetweenRooms = distanceBetweenRooms;
    }

    public void CreateActualCorridor()
    {
        int diffX = Math.Abs(room1.centrePos.x - room2.centrePos.x);
        int diffY = Math.Abs(room1.centrePos.y - room2.centrePos.y);

        if (diffX > diffY) //Horizontal corridor
        {
            Room leftRoom = room1;
            Room rightRoom = room2;
            
            if (room1.centrePos.x > room2.centrePos.x)
            {
                leftRoom = room2;
                rightRoom = room1;
            }

            int leftDoorX = leftRoom.centrePos.x + Mathf.CeilToInt((float)leftRoom.size.x / 2);
            int rightDoorX = rightRoom.centrePos.x - Mathf.CeilToInt((float)rightRoom.size.x / 2);
            
            leftRoom.SetDoor(new Vector2Int(leftDoorX-1, leftRoom.centrePos.y), corridorWidth, false);
            rightRoom.SetDoor(new Vector2Int(rightDoorX+1, rightRoom.centrePos.y), corridorWidth, false);

            int middleX = leftDoorX + (rightDoorX - leftDoorX) / 2;
            
            topLeft = new Vector2Int(leftDoorX, leftRoom.centrePos.y + corridorWidth/2);
            topRight = new Vector2Int(rightDoorX, rightRoom.centrePos.y + corridorWidth/2);
            midTopLeft = new Vector2Int(middleX, leftRoom.centrePos.y + corridorWidth/2);
            midBotLeft = new Vector2Int(middleX, rightRoom.centrePos.y + corridorWidth/2);

            botLeft = new Vector2Int(leftDoorX, leftRoom.centrePos.y - corridorWidth/2);
            botRight = new Vector2Int(rightDoorX, rightRoom.centrePos.y - corridorWidth/2);

            List<Vector2Int> leftMap = new List<Vector2Int>() 
                {new Vector2Int(leftDoorX, leftRoom.centrePos.y), new Vector2Int(middleX+1, leftRoom.centrePos.y)};
            
            List<Vector2Int> rightMap = new List<Vector2Int>() 
                {new Vector2Int(middleX, rightRoom.centrePos.y), new Vector2Int(rightDoorX, rightRoom.centrePos.y)};
            
            List<Vector2Int> middleMap = new List<Vector2Int>() 
                {new Vector2Int(middleX, leftRoom.centrePos.y), new Vector2Int(middleX, rightRoom.centrePos.y)};

            if (leftRoom.centrePos.y > rightRoom.centrePos.y)
            {
                middleMap = new List<Vector2Int>() 
                    {new Vector2Int(middleX, rightRoom.centrePos.y), new Vector2Int(middleX, leftRoom.centrePos.y)};
            }

            FillCorridorTilesSet(leftMap, DirectionOfTheCorridor.horizontal);
            FillCorridorTilesSet(rightMap, DirectionOfTheCorridor.horizontal);
            FillCorridorTilesSet(middleMap, DirectionOfTheCorridor.vertical);
        }
        else //Vertical corridor
        {
            Room bottomRoom = room1;
            Room upRoom = room2;
            
            if (room1.centrePos.y > room2.centrePos.y)
            {
                bottomRoom = room2;
                upRoom = room1;
            }
            
            int bottomDoorY = bottomRoom.centrePos.y + Mathf.CeilToInt((float)bottomRoom.size.y / 2);
            int upDoorY = upRoom.centrePos.y - Mathf.CeilToInt((float)upRoom.size.y / 2);
            
            bottomRoom.SetDoor(new Vector2Int(bottomRoom.centrePos.x, bottomDoorY-1), corridorWidth, true);
            upRoom.SetDoor(new Vector2Int(upRoom.centrePos.x, upDoorY+1), corridorWidth, true);

            int middleY = bottomDoorY + (upDoorY - bottomDoorY) / 2;
            
            List<Vector2Int> bottomMap = new List<Vector2Int>() 
                {new Vector2Int(bottomRoom.centrePos.x, bottomDoorY), new Vector2Int(bottomRoom.centrePos.x, middleY+1)};
            
            List<Vector2Int> upMap = new List<Vector2Int>() 
                {new Vector2Int(upRoom.centrePos.x, middleY-1), new Vector2Int(upRoom.centrePos.x, upDoorY)};
            
            List<Vector2Int> middleMap = new List<Vector2Int>() 
                {new Vector2Int(bottomRoom.centrePos.x, middleY), new Vector2Int(upRoom.centrePos.x, middleY)};
            
            if (bottomRoom.centrePos.x > upRoom.centrePos.x)
            {
                middleMap = new List<Vector2Int>() 
                    {new Vector2Int(upRoom.centrePos.x, middleY), new Vector2Int(bottomRoom.centrePos.x, middleY)};
            }

            FillCorridorTilesSet(bottomMap, DirectionOfTheCorridor.vertical);
            FillCorridorTilesSet(upMap, DirectionOfTheCorridor.vertical);
            FillCorridorTilesSet(middleMap, DirectionOfTheCorridor.horizontal);
        }
    }

    public void SetCorridorWidth(int corridorWidth) => this.corridorWidth = corridorWidth;

    private void FillCorridorTilesSet(List<Vector2Int> map, DirectionOfTheCorridor direction)
    {
        if (direction == DirectionOfTheCorridor.horizontal)
        {
            for (int i = 0; i <= map[1].x - map[0].x; i++)
            {
                corridorTiles.Add(new Vector2Int(map[0].x + i, map[0].y));

                if (corridorWidth % 2 != 0)
                {
                    for (int j = 1; j <= corridorWidth/2; j++)
                    {
                        corridorTiles.Add(new Vector2Int(map[0].x + i, map[0].y + j));
                        corridorTiles.Add(new Vector2Int(map[0].x + i, map[0].y - j));
                    }   
                }
                else
                {
                    for (int j = 0; j <= corridorWidth/2; j++)
                    {
                        corridorTiles.Add(new Vector2Int(map[0].x + i, map[0].y + j));
                    }   
                    for (int j = 1; j < corridorWidth/2; j++)
                    {
                        corridorTiles.Add(new Vector2Int(map[0].x + i, map[0].y - j));
                    }   
                }

            }
        }
        else
        {
            for (int i = 0; i <= map[1].y - map[0].y; i++)
            {
                corridorTiles.Add(new Vector2Int(map[0].x, map[0].y + i));

                if (corridorWidth % 2 != 0)
                {
                    for (int j = 1; j <= corridorWidth / 2; j++)
                    {
                        corridorTiles.Add(new Vector2Int(map[0].x + j, map[0].y + i));
                        corridorTiles.Add(new Vector2Int(map[0].x - j, map[0].y + i));
                    }
                }
                else
                {
                    for (int j = 0; j <= corridorWidth / 2; j++)
                    {
                        corridorTiles.Add(new Vector2Int(map[0].x + j, map[0].y + i));
                    }
                    for (int j = 1; j < corridorWidth / 2; j++)
                    {
                        corridorTiles.Add(new Vector2Int(map[0].x - j, map[0].y + i));
                    }
                }
            }
        }
    }
}

public enum DirectionOfTheCorridor
{
    vertical,
    horizontal
}