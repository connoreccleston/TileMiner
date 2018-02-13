using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private void Awake()
    {
        Camera.main.transform.position = transform.position - transform.forward * 10;
    }

    private void Update()
    {
        bool dirtyWorld = false;

        if (Input.GetKeyDown(KeyCode.W))
            transform.position += transform.up;
        if (Input.GetKeyDown(KeyCode.S))
            transform.position -= transform.up;
        if (Input.GetKeyDown(KeyCode.A))
            transform.position -= transform.right;
        if (Input.GetKeyDown(KeyCode.D))
            transform.position += transform.right;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //transform.position -= transform.forward;
            foreach (var tile in SpriteManager.AllTiles)
                tile.transform.position -= transform.forward;
            transform.position -= transform.forward;
            Camera.main.transform.position -= transform.forward;
            dirtyWorld = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //transform.position += transform.forward;
            foreach (var tile in SpriteManager.AllTiles)
                tile.transform.position += transform.forward;
            transform.position += transform.forward;
            Camera.main.transform.position += transform.forward;
            dirtyWorld = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //transform.Rotate(new Vector3(0, -90, 0));
            foreach (var tile in SpriteManager.AllTiles)
                tile.transform.RotateAround(transform.position, Vector3.up, -90);
            transform.RotateAround(transform.position, Vector3.up, -90);
            Camera.main.transform.RotateAround(transform.position, Vector3.up, -90);
            dirtyWorld = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //transform.Rotate(new Vector3(0, 90, 0));
            foreach (var tile in SpriteManager.AllTiles)
                tile.transform.RotateAround(transform.position, Vector3.up, 90);
            transform.RotateAround(transform.position, Vector3.up, 90);
            Camera.main.transform.RotateAround(transform.position, Vector3.up, 90);
            dirtyWorld = true;
        }

        if (dirtyWorld)
            TileBehaviour.ResetAll();

        Vector3 target = transform.position - transform.forward * 10;
        Vector3 velocity = new Vector3();
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, target, ref velocity, 0.05f);
    }
}
