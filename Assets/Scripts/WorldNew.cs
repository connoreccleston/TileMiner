using System.Collections.Generic;
using UnityEngine;

public static class WorldNew
{
    private const int Size = 100;
    private static TileType[,,] Map = new TileType[Size, Size, Size];
    private static Dictionary<Vector3Int, PersistentData> Special = new Dictionary<Vector3Int, PersistentData>();

    private static Vector3Int Origin = new Vector3Int(Size / 2, Size / 2, Size / 2);
    private static Direction Direction = Direction.North;

    static WorldNew()
    {
        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                    Map[x, y, z] = (TileType)((x + y + z) % 2 + 1);
    }

    public static void Turn(Vector2 position, bool left)
    {
        Vector3Int pos = Convert(position);
        if (left)
        {
            Origin = Vector3Int.RoundToInt(Quaternion.Euler(0, -90, 0) * (Origin - pos) + pos);
            Direction = (Direction)(((int)Direction + 3) % 4);
        }
        else
        {
            Origin = Vector3Int.RoundToInt(Quaternion.Euler(0, 90, 0) * (Origin - pos) + pos);
            Direction = (Direction)(((int)Direction + 1) % 4);
        }
    }

    public static void Move(bool deeper)
    {
        Vector3Int offset = new Vector3Int();
        switch (Direction)
        {
            case Direction.North:
                offset = new Vector3Int(0, 0, 1);
                break;
            case Direction.East:
                offset = new Vector3Int(1, 0, 0);
                break;
            case Direction.South:
                offset = new Vector3Int(0, 0, -1);
                break;
            case Direction.West:
                offset = new Vector3Int(-1, 0, 0);
                break;
        }

        if (!deeper)
            offset *= -1;

        Origin += offset;
    }

    public static Vector3Int Convert(Vector2 position)
    {
        Vector2Int pos = Vector2Int.RoundToInt(position);
        int x, y, z;
        x = y = z = -1;
        switch (Direction)
        {
            case Direction.North:
                x = pos.x + Origin.x;
                y = pos.y + Origin.y;
                z = Origin.z;
                break;
            case Direction.East:
                x = Origin.x;
                y = pos.y - Origin.y;
                z = pos.x + Origin.z;
                break;
            case Direction.South:
                x = -pos.x + Origin.x;
                y = pos.y + Origin.y;
                z = Origin.z;
                break;
            case Direction.West:
                x = Origin.x;
                y = pos.y + Origin.y;
                z = -pos.x + Origin.z;
                break;
        }
        return new Vector3Int(x, y, z);
    }

    public static bool InBounds(int x, int y, int z)
    {
        return !(x < 0 || y < 0 || z < 0 || x >= Size || y >= Size || z >= Size);
    }

    public static bool InBounds(Vector3Int v)
    {
        return InBounds(v.x, v.y, v.z);
    }

    public static PersistentData GetSpecial(int x, int y, int z)
    {
        return GetSpecial(new Vector3Int(x, y, z));
    }

    public static PersistentData GetSpecial(Vector3Int location)
    {
        if (Special.ContainsKey(location))
            return Special[location];

        return null;
    }

    public static void SetSpecial(int x, int y, int z, PersistentData tmd)
    {
        SetSpecial(new Vector3Int(x, y, z), tmd);
    }

    public static void SetSpecial(Vector3Int location, PersistentData tmd)
    {
        Special[location] = tmd;
    }

    public static TileType GetTileType(Vector3Int position)
    {
        return Map[position.x, position.y, position.z];
    }

    static TileData TileData;
    public static TileType GetTileType(Vector2 position, out int depth)
    {
        depth = 0;
        Vector3Int pos = Convert(position);
        Util.Load(ref TileData, "Tiles");

        if (!InBounds(pos))
            return TileType.Air;

        TileType type = GetTileType(pos);

        Vector3Int offset = new Vector3Int();
        switch (Direction)
        {
            case Direction.North:
                offset = new Vector3Int(0, 0, 1);
                break;
            case Direction.East:
                offset = new Vector3Int(1, 0, 0);
                break;
            case Direction.South:
                offset = new Vector3Int(0, 0, -1);
                break;
            case Direction.West:
                offset = new Vector3Int(-1, 0, 0);
                break;
        }

        for (; TileData[type].Sprite == null; depth++)
            type = GetTileType(pos += offset); // lol

        return type;
    }

    public static bool SetTileType(Vector2 position, TileType type)
    {
        Vector3Int pos = Convert(position);

        if (!InBounds(pos))
            return false;

        Map[pos.x, pos.y, pos.z] = type;
        return true;
    }
}
