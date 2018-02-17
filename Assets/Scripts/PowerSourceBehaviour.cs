using System.Collections.Generic;
using UnityEngine;

public class PowerSourceBehaviour : MonoBehaviour
{
	private void Start()
	{
        new PowerSource(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)PlayerController.Position.z));
    }
}

public class PowerSource : TileMetadata
{
    public readonly bool Powered = true;

    public PowerSource(Vector3Int position) : base(position)
    {
        // should push its power to pylons when created
    }
}
