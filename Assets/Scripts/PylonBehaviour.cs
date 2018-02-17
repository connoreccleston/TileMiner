using System.Collections.Generic;
using UnityEngine;

public class PylonBehaviour : MonoBehaviour
{

	private void Start()
	{

	}
	
	private void Update()
	{
		
	}
}

public class PylonData : TileMetadata
{
    public bool Powered { get; private set; }

    public readonly Vector3Int Location;
    public readonly int Radius;

    private List<PylonData> Attached;
    private List<PowerSource> Sources;

    public PylonData(Vector3Int location, int radius)
    {
        Location = location;
        Radius = radius;

        World.SetSpecial(Location, this);

        FindConnected();
    }

    private void FindConnected()
    {
        bool anyPowered = false;

        for (int x = Location.x - Radius; x <= Location.x + Radius; x++)
        {
            for (int y = Location.y - Radius; y <= Location.y + Radius; y++)
            {
                for (int z = Location.z - Radius; z <= Location.z + Radius; z++)
                {
                    TileMetadata tile = World.GetSpecial(x, y, z);

                    PylonData pd = tile as PylonData;
                    if (pd != null && pd != this)
                    {
                        Attached.Add(pd);
                        if (pd.Powered)
                            anyPowered = true;
                    }
                    else
                    {
                        PowerSource ps = tile as PowerSource;
                        if (ps != null)
                        {
                            Sources.Add(ps);
                            if (ps.Powered)
                                anyPowered = true;
                        }
                    }
                }
            }
        }

        if (anyPowered)
            PropagatePower();
    }

    private void PropagatePower()
    {
        Powered = true;

        foreach (PylonData pd in Attached)
            if (!pd.Powered)
                pd.PropagatePower();
    }

    //~PylonData()
    //{

    //}

    //private void Remove()
    //{

    //}
}

