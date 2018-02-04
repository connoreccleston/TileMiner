using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // Make things static?
    public TileData Tiles;
    public Dictionary<TileType, TileDatum> TileTable = new Dictionary<TileType, TileDatum>();

    private TileType[,,] World;
    public int Size = 100;
    private int slice = 0;
    private Direction direction = Direction.North;

	private void Awake()
	{
        Camera.main.transform.position = new Vector3(Size / 2, Size / 2, -10);

        World = new TileType[Size, Size, 1];

		foreach (TileDatum temp in Tiles.Tiles)
            TileTable.Add(temp.Type, temp);

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < 1; k++)//
                    World[i, j, k] = (TileType)((i + j + k) % 2 + 1);
	}
	
	public TileType GetTileType(GameObject GO)
    {
        int x = (int)GO.transform.position.x;
        int y = (int)GO.transform.position.y;

        if (x < 0 || y < 0 || x >= Size || y >= Size)
            return TileType.Air;

        switch (direction)
        {
            case Direction.North:
                return World[x, y, slice];
            case Direction.East:
                return World[-slice, y, x];
            case Direction.South:
                return World[-x, y, -slice];
            case Direction.West:
                return World[slice, y, -x];
            default:
                return TileType.Air;
        }
    }

    public bool SetTileType(GameObject GO, TileType Type)
    {
        int x = (int)GO.transform.position.x;
        int y = (int)GO.transform.position.y;

        if (x < 0 || y < 0 || x >= Size || y >= Size)
            return false;

        switch (direction)
        {
            case Direction.North:
                World[x, y, slice] = Type;
                break;
            case Direction.East:
                World[-slice, y, x] = Type;
                break;
            case Direction.South:
                World[-x, y, -slice] = Type;
                break;
            case Direction.West:
                World[slice, y, -x] = Type;
                break;
        }

        return true;
    }

    private enum Direction
    {
        North,
        East,
        South,
        West
    }
}
