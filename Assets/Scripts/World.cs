using System.Collections.Generic;
using UnityEngine;

public static class WorldOld
{
    public const int Size = 100;
    private static TileType[,,] Map = new TileType[Size, Size, Size];
    private static Dictionary<Vector3Int, PersistentData> Special = new Dictionary<Vector3Int, PersistentData>();

    static WorldOld()
	{

        for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                for (int z = 0; z < Size; z++)
                    Map[x, y, z] = (TileType)((x + y + z) % 2 + 1);
    }

    public static bool InBounds(int x, int y, int z)
    {
        return !(x < 0 || y < 0 || z < 0 || x >= Size || y >= Size || z >= Size);
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

    public static TileType GetTileType(Vector3Int Position)
    {
        return Map[Position.x, Position.y, Position.z];
    }

    public static TileType GetTileType(Vector3 Position, out int Depth)
    {
        Depth = 0;

        int x = -1;
        int y = (int)Position.y;
        int z = -1;
        //switch (PlayerController.Facing)
        //{
        //    case Direction.North: // World[x, y, Slice]                
        //        x = (int)Position.x;
        //        z = (int)Position.z;
        //        break;
        //    case Direction.East: // World[-Slice, y, x]
        //        x = (int)-Position.z;
        //        z = (int)Position.x;
        //        break;
        //    case Direction.South: // World[-x, y, -Slice]
        //        x = (int)-Position.x;
        //        z = (int)-Position.z;
        //        break;
        //    case Direction.West: // World[Slice, y, -x]
        //        x = (int)Position.z;
        //        z = (int)-Position.x;
        //        break;
        //}

        if (!InBounds(x, y, z))
            return TileType.Air;

        TileType type =  Map[x, y, z];

        while (type == TileType.Air)
        {
            //switch (PlayerController.Facing)
            //{
            //    case Direction.North:
            //        z++;
            //        break;
            //    case Direction.East:
            //        x--;
            //        break;
            //    case Direction.South:
            //        z--;
            //        break;
            //    case Direction.West:
            //        x++;
            //        break;
            //}

            if (!InBounds(x, y, z))
                return TileType.Air;

            type = Map[x, y, z];
            Depth++;
        }

        return type;
    }

    public static bool SetTileType(Vector3 Position, TileType Type)
    {
        int x = (int)Position.x;
        int y = (int)Position.y;
        int z = (int)Position.z;

        //Debug.Log(x + " " + y + " " + z);

        if (!InBounds(x, y, z))
            return false;

        //switch (PlayerController.Facing)
        //{
        //    case Direction.North:
        //        Map[x, y, z] = Type;
        //        break;
        //    case Direction.East:
        //        Map[-z, y, x] = Type;
        //        break;
        //    case Direction.South:
        //        Map[-x, y, -z] = Type;
        //        break;
        //    case Direction.West:
        //        Map[z, y, -x] = Type;
        //        break;
        //}

        return true;
    }
}
