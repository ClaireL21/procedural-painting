using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public float dragSpeed = 0.01f;
    private Vector3 dragOrigin;
    private Vector3 ResetCamera;
    bool leftClick;
    bool rightClick;


    // Start is called before the first frame update
    void Start()
    {
        ResetCamera = transform.position;
        leftClick = false;
        rightClick = false;
    }

    // Update is called once per frame
    void Update()
    {
        /* Reset camera position on R key */
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = ResetCamera;
        }

        /* Pan camera on left click and drag */
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            leftClick = true;
            return;
        }
        if (leftClick)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(0, pos.y * dragSpeed, pos.x * dragSpeed);
            transform.Translate(move, Space.World);

        }
        if (!Input.GetMouseButton(0))
        {
            leftClick = false;
        }

        /* Zoom camera on right click and drag */
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            rightClick = true;
            return;
        }
        if (rightClick)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
            transform.Translate(move, Space.World);

        }
        if (!Input.GetMouseButton(1))
        {
            rightClick = false;
        }

        /*
        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(0, pos.y * dragSpeed, pos.x * dragSpeed);

        // Zooming in
        //Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);

        */

    }
}
