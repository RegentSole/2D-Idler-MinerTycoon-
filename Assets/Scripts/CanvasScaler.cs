// Добавьте этот скрипт к главному Canvas
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerAdjuster : MonoBehaviour
{
    private CanvasScaler canvasScaler;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        // Для WebGL используйте фиксированное DPI
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = 1;
            canvasScaler.referencePixelsPerUnit = 100;
        }
    }
}