using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static void Find(ref GameObject go, string name)
    {
        if (go == null)
            go = GameObject.Find(name);
    }

    public static void Load<T>(ref T resource, string path)
    {
        if (resource == null)
            resource = (T)(object)Resources.Load(path, typeof(T));
    }

    public static float TaxiDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
