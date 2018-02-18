// Modified from https://github.com/tasaboia/Procedural-Rope-Generator

using UnityEngine;

public static class Catenary
{
    //private static float CosH(float t)
    //{
    //    return (Mathf.Exp(t) + Mathf.Exp(-t)) / 2;
    //}

    private static float CalculateCatenary(float a, float x)
    {
        return a * (float)System.Math.Cosh(x / a);
    }

    public static void Generate(LineRenderer line, Vector3 point1, Vector3 point2, float wireCatenary = 10, float wireRes = 1)
    {
        float distance = Vector3.Distance(point1, point2);
        int nPoints = (int)(distance / wireRes + 1);

        wireRes = distance / (nPoints - 1);

        Vector3[] wirePoints = new Vector3[nPoints];
        wirePoints[0] = point1;
        wirePoints[nPoints - 1] = point2;

        Vector3 dir = (point2 - point1).normalized;
        float offset = CalculateCatenary(wireCatenary, -distance / 2);

        for (int i = 1; i < nPoints - 1; ++i)
        {
            Vector3 wirePoint = point1 + i * wireRes * dir;

            float x = i * wireRes - distance / 2;
            wirePoint.y = wirePoint.y - (offset - CalculateCatenary(wireCatenary, x));

            wirePoints[i] = wirePoint;
        }

        GenerateWithLine(line, wirePoints);
    }

    private static void GenerateWithLine(LineRenderer line, Vector3[] wirePoints)
    {
        line.positionCount = wirePoints.Length;

        for (int i = 0; i < wirePoints.Length; ++i)
        {
            line.SetPosition(i, wirePoints[i]);
        }
    }
}
