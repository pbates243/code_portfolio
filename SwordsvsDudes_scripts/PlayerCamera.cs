using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    float smoothening = 0.1f;
    public Vector2 minimumLimits = new Vector3(4, 9f);
    public Vector2 maximumLimits = new Vector2(9999, 9999);

    Vector3 offset = new Vector3(0, 0.01f, -10);

    Transform player;

    private void Start ()
    {
        player = transform.parent;
        transform.SetParent(null);
    }

    private void FixedUpdate ()
    {
        Vector3 newPos = Vector3.Lerp(transform.position, player.position + offset, smoothening);

        // Reset the Z to always be at its offset
        newPos.z = offset.z;

        // And check if the camera is dipping below its minimum Y, if so bring it back up
        if(newPos.y < minimumLimits.y)
        {
            newPos.y = minimumLimits.y;
        }
        if (newPos.y > maximumLimits.y)
        {
            newPos.y = maximumLimits.y;
        }
        if(newPos.x < minimumLimits.x)
        {
            newPos.x = minimumLimits.x;
        }
        if(newPos.x > maximumLimits.x)
        {
            newPos.x = maximumLimits.x;
        }

        transform.position = newPos;
    }

}
