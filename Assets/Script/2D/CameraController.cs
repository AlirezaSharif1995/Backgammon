using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public SpriteRenderer background;
    public SpriteRenderer backFrame;

    void Start()
    {
        AdjustCameraSize();
        SetBackgroundSize();
    }

    void AdjustCameraSize()
    {
        float spriteWidth = backFrame.bounds.size.x;
        float spriteHeight = backFrame.bounds.size.y;

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = spriteWidth / spriteHeight;

        if (screenAspect >= targetAspect)
        {
            Camera.main.orthographicSize = spriteHeight / 2;
        }
        else
        {
            float differenceInSize = targetAspect / screenAspect;
            Camera.main.orthographicSize = spriteHeight / 2 * differenceInSize;
        }
    }
    
    void SetBackgroundSize()
    {
        Vector2 spriteSize = background.sprite.bounds.size;

        float screenHeight = Camera.main.orthographicSize * 2.0f;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        Vector3 scale = background.transform.localScale;
        scale.x = screenWidth / spriteSize.x;
        scale.y = screenHeight / spriteSize.y;
        background.transform.localScale = scale;
    }
}
