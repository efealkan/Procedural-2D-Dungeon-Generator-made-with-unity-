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

    public Dictionary<List<Vector2Int>, DirectionOfTheCorridor> corridorVertexMap;
    
    public Corridor(Room room1, Room room2, int distanceBetweenRooms)
    {
        this.room1 = room1;
        this.room2 = room2;
        this.distanceBetweenRooms = distanceBetweenRooms;
        
        corridorVertexMap = new Dictionary<List<Vector2Int>, DirectionOfTheCorridor>();
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

            List<Vector2Int> leftMap = new List<Vector2Int>() 
                {new Vector2Int(leftDoorX, leftRoom.centrePos.y), new Vector2Int(middleX, leftRoom.centrePos.y)};
            
            List<Vector2Int> rightMap = new List<Vector2Int>() 
                {new Vector2Int(middleX, rightRoom.centrePos.y), new Vector2Int(rightDoorX, rightRoom.centrePos.y)};
            
            List<Vector2Int> middleMap = new List<Vector2Int>() 
                {new Vector2Int(middleX, leftRoom.centrePos.y), new Vector2Int(middleX, rightRoom.centrePos.y)};

            if (leftRoom.centrePos.y > rightRoom.centrePos.y)
            {
                middleMap = new List<Vector2Int>() 
                    {new Vector2Int(middleX, rightRoom.centrePos.y), new Vector2Int(middleX, leftRoom.centrePos.y)};
            }
            
            corridorVertexMap.Add(leftMap, DirectionOfTheCorridor.horizontal);
            corridorVertexMap.Add(rightMap, DirectionOfTheCorridor.horizontal);
            corridorVertexMap.Add(middleMap, DirectionOfTheCorridor.vertical);
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
                {new Vector2Int(bottomRoom.centrePos.x, bottomDoorY), new Vector2Int(bottomRoom.centrePos.x, middleY)};
            
            List<Vector2Int> upMap = new List<Vector2Int>() 
                {new Vector2Int(upRoom.centrePos.x, middleY), new Vector2Int(upRoom.centrePos.x, upDoorY)};
            
            List<Vector2Int> middleMap = new List<Vector2Int>() 
                {new Vector2Int(bottomRoom.centrePos.x, middleY), new Vector2Int(upRoom.centrePos.x, middleY)};
            
            if (bottomRoom.centrePos.x > upRoom.centrePos.x)
            {
                middleMap = new List<Vector2Int>() 
                    {new Vector2Int(upRoom.centrePos.x, middleY), new Vector2Int(bottomRoom.centrePos.x, middleY)};
            }
            
            corridorVertexMap.Add(bottomMap, DirectionOfTheCorridor.vertical);
            corridorVertexMap.Add(upMap, DirectionOfTheCorridor.vertical);
            corridorVertexMap.Add(middleMap, DirectionOfTheCorridor.horizontal);
        }
    }

    public void SetCorridorWidth(int corridorWidth) => this.corridorWidth = corridorWidth;
}

public enum DirectionOfTheCorridor
{
    vertical,
    horizontal
}
