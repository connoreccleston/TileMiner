using System.Collections.Generic;
using UnityEngine;

public abstract class TileMetadata // : ScriptableObject { }
{
    public readonly Vector3Int Location;

    public TileMetadata(Vector3Int location)
    {
        Location = location;
        World.SetSpecial(Location, this);
    }
}