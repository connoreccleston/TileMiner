using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

public static class WorldNew
{
    private const int Size = 100;
    private static TileType[,,] Map = new TileType[Size, Size, Size];
    private static Dictionary<Vector3Int, PersistentData> Special = new Dictionary<Vector3Int, PersistentData>();

    public static Vector3Int Origin { get; private set; } //= new Vector3Int(Size / 2, Size / 2, Size / 2);
    private static Direction Direction = Direction.North;

    static WorldNew()
    {
        Origin = new Vector3Int(0, 0, Size / 2); //new Vector3Int(Size / 2, Size / 2, Size / 2);

        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                    Map[x, y, z] = (TileType)((x + y + z) % 2 + 1);
    }

    public static Vector3Int DepthVector
    {
        get
        {
            switch (Direction)
            {
                case Direction.North:
                    return new Vector3Int(0, 0, 1);
                case Direction.East:
                    return new Vector3Int(1, 0, 0);
                case Direction.South:
                    return new Vector3Int(0, 0, -1);
                case Direction.West:
                    return new Vector3Int(-1, 0, 0);
                default:
                    return new Vector3Int(0, 0, 0);
            }
        }
    }

    // STILL DOESN'T WORK REEEEEEEEEEE (or convert might not)
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

    public static bool Move(Vector2 position, bool deeper)
    {
        if (deeper)
        {
            if (GetTileType(Convert(position) + DepthVector) != TileType.Air)
                return false;

            Origin += DepthVector;
        }
        else
        {
            if (GetTileType(Convert(position) - DepthVector) != TileType.Air)
                return false;

            Origin -= DepthVector;
        }
        return true;
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

    public static void SetSpecial(int x, int y, int z, PersistentData pd)
    {
        SetSpecial(new Vector3Int(x, y, z), pd);
    }

    public static void SetSpecial(Vector3Int location, PersistentData pd)
    {
        Special[location] = pd;
    }

    public static TileType GetTileType(Vector3Int position)
    {
        return Map[position.x, position.y, position.z];
    }

    public static TileType GetTileType(Vector2 position)
    {
        int depth;
        return GetTileType(position, out depth);
    }

    public static TileType GetTileType(Vector2 position, out int depth)
    {
        TileType tt;
        return GetTileType(position, out depth, out tt);
    }

    static TileData TileData;
    public static TileType GetTileType(Vector2 position, out int depth, out TileType transType)
    {
        depth = 0;
        transType = TileType.Air;
        Vector3Int pos = Convert(position);
        Util.Load(ref TileData, "Tiles");

        if (!InBounds(pos))
            return TileType.Void;

        TileType type = GetTileType(pos);
        transType = type;

        for (; TileData[type].Sprite == null; depth++)
            type = GetTileType(pos += DepthVector);

        return type;
    }

    public static bool SetTileType(Vector2 position, TileType type)
    {
        return SetTileType(Convert(position), type);
    }

    public static bool SetTileType(Vector3Int position, TileType type)
    {
        if (!InBounds(position))
            return false;

        Map[position.x, position.y, position.z] = type;

        return true;
    }
}
