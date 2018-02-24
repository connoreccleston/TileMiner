using UnityEngine;

public abstract class SpecialBehaviour : MonoBehaviour
{

}

public abstract class PersistentData
{
    public readonly Vector3Int Location;

    public PersistentData(Vector3Int location)
    {
        Location = location;
        WorldNew.SetSpecial(Location, this);
    }
}