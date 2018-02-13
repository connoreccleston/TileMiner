using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Direction Facing = Direction.North;
    public Vector3Int Position = Vector3Int.zero;

    private bool dirtyWorld = false;

    private void Update()
    {
        Vector3Int aVector, dVector, qVector, eVector;
        aVector = dVector = qVector = eVector = new Vector3Int();

        switch (Facing)
        {
            case Direction.North:
                aVector = Vector3Int.left;
                dVector = Vector3Int.right;
                qVector = Vector3Int.RoundToInt(Vector3.back);
                eVector = Vector3Int.RoundToInt(Vector3.forward);
                break;
            case Direction.East:
                aVector = Vector3Int.RoundToInt(Vector3.forward); 
                dVector = Vector3Int.RoundToInt(Vector3.back); 
                qVector = Vector3Int.left;
                eVector = Vector3Int.right;
                break;
            case Direction.South:
                aVector = Vector3Int.right;
                dVector = Vector3Int.left;
                qVector = Vector3Int.RoundToInt(Vector3.forward);
                eVector = Vector3Int.RoundToInt(Vector3.back);
                break;
            case Direction.West:
                aVector = Vector3Int.RoundToInt(Vector3.back);
                dVector = Vector3Int.RoundToInt(Vector3.forward);
                qVector = Vector3Int.right;
                eVector = Vector3Int.left;
                break;
        }

        if (Input.GetKeyDown(KeyCode.W))
            Position += Vector3Int.up;
        if (Input.GetKeyDown(KeyCode.S))
            Position += Vector3Int.down;
        if (Input.GetKeyDown(KeyCode.A))
            Position += aVector;
        if (Input.GetKeyDown(KeyCode.D))
            Position += dVector;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Position += qVector;
            dirtyWorld = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Position += eVector;
            dirtyWorld = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Facing = (Direction)(((int)Facing - 1 + 4) % 4);
            dirtyWorld = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Facing = (Direction)(((int)Facing + 1) % 4);
            dirtyWorld = true;
        }
    }

    private void LateUpdate()
    {
        if (dirtyWorld)
        {
            TileBehaviour.ResetAll();
            dirtyWorld = false;
        }

        //Camera.main.transform.position = Vector3.SmoothDamp()
    }
}

public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}
