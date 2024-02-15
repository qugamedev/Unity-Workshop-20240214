using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrapper : MonoBehaviour
{
    public float margin = 0.1f;

    void Update()
    {
       Vector2 dim = Game.GetScreenDim();
       float screenWidth = dim.x + margin;
       float screenHeight = dim.y + margin;

        Vector2 newPosition = transform.position;

        if (transform.position.x > screenWidth / 2)
        {
            newPosition.x = -screenWidth / 2;
        }

        if (transform.position.x < -screenWidth / 2)
        {
            newPosition.x = screenWidth / 2;
        }

        if (transform.position.y > screenHeight / 2)
        {
            newPosition.y = -screenHeight / 2;
        }

        if (transform.position.y < -screenHeight / 2)
        {
            newPosition.y = screenHeight / 2;
        }

        transform.position = newPosition;
    }
}
