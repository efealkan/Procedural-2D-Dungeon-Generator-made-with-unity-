using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MST
{
    private static List<Corridor> corridors = new List<Corridor>();

    public static List<Corridor> CreateCorridors(List<Room> rooms, Vector2Int chunkSize)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i+1; j < rooms.Count; j++)
            {
                Room r1 = rooms[i];
                Room r2 = rooms[j];
                
                corridors.Add(new Corridor(r1, r2, CalculateDistance(r1, r2)));
            }
        }

        return ApplyMST(rooms.Count, chunkSize);
    }

    private static List<Corridor> ApplyMST(int n, Vector2Int chunkSize)
    {
        UnionFind uf = new UnionFind(n);
        corridors.Sort((x, y) => x.distanceBetweenRooms.CompareTo(y.distanceBetweenRooms));
        
        List<Corridor> corridorsInMst = new List<Corridor>();
        
        for (int i = 0; i < corridors.Count; i++)
        {
            if (uf.Union(corridors[i].room1.id, corridors[i].room2.id))
            {
                corridorsInMst.Add(corridors[i]);
            }
            else if (corridors[i].distanceBetweenRooms < (chunkSize.x + chunkSize.y)/1.5)
            {
                //Even if the edge should not be in mst, there is a chance it can still appear in mst.
                if (Random.Range(0, 10) < 2)
                {
                    uf.Union(corridors[i].room1.id, corridors[i].room2.id);
                    corridorsInMst.Add(corridors[i]);
                }
            }
        }

        return corridorsInMst;
    }

    private static int CalculateDistance(Room r1, Room r2)
    {
        int x = (int) Math.Pow(r1.centrePos.x - r2.centrePos.x, 2);
        int y = (int) Math.Pow(r1.centrePos.y - r2.centrePos.y, 2);
        return (int) Math.Sqrt(x + y);
    }
}

public class UnionFind
{
    public int[] rank;
    public int[] parent;

    public UnionFind(int size)
    {
        rank = new int[size];
        parent = new int[size];
        for (int i = 0; i < size; i++) parent[i] = i;
    }

    public int Find(int i)
    {
        if (parent[i] != i) return parent[i] = Find(parent[i]);
        return parent[i];
    }

    public bool Union(int i, int j)
    {
        int setI = Find(i);
        int setJ = Find(j);

        if (setI == setJ) return false;
        if (rank[setI] > rank[setJ]) {
            parent[setJ] = setI;
            rank[setJ] = rank[setI];
        } else if (rank[setI] < rank[setJ]) {
            parent[setI] = setJ;
            rank[setI] = rank[setJ];
        } else {
            parent[setI] = setJ;
            rank[setI] = rank[setJ];
            rank[setJ]++;
        }
        return true;
    }
}
