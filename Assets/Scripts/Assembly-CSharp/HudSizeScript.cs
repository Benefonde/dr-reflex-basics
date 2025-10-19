using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudSizeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }

    // Update is called once per frame
    void Update()
    {
        canvasScaler.scaleFactor = Mathf.FloorToInt((Screen.height - 1) / 240);
        if (canvasScaler.scaleFactor <= 0.01)
        {
            canvasScaler.scaleFactor = 0.65f;
        }
        print(Screen.height / 240);
    }

    CanvasScaler canvasScaler;
}
