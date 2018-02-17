using System.Collections.Generic;
using UnityEngine;

public abstract class TileMetadata : ScriptableObject { }

public class PowerSource : TileMetadata
{
    public readonly bool Powered = true;
}
