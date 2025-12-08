using UnityEngine;
using Unity.Cinemachine;

public class CameraZoomSwitcher : MonoBehaviour
{
    public Transform player;

    [Tooltip("Kamera z mniejszym zoomem.")]
    public CinemachineCamera zoomOutCamera;

    [Tooltip("Kamera z wiêkszym zoomem.")]
    public CinemachineCamera zoomInCamera;

    [Tooltip("Powy¿ej tej wartoœci w³¹czany jest Zoom Out.")]
    public float zoomOutThreshold = 1.5f;

    [Tooltip("Poni¿ej tej wartoœci w³¹czany jest Zoom In.")]
    public float zoomInThreshold = -2;

    [Header("Priorytety")]
    private const int BasePriority = 10;
    private const int ActivePriority = 11;

    private bool isZoomInActive = true;


    void Start()
    {
        if (zoomOutThreshold <= zoomInThreshold)
        {
            Debug.LogError("Zoom Out musi byæ wy¿szy ni¿ Zoom In.");
        }

        InitializeCameraPriority();
    }

    void InitializeCameraPriority()
    {
        if (player == null || zoomOutCamera == null || zoomInCamera == null)
        {
            Debug.LogError("Brak przypisanych celów.");
            return;
        }

        if (player.position.y >= zoomOutThreshold)
        {
            zoomOutCamera.Priority = ActivePriority;
            zoomInCamera.Priority = BasePriority;
            isZoomInActive = false;
        }
        else
        {
            zoomOutCamera.Priority = BasePriority;
            zoomInCamera.Priority = ActivePriority;
            isZoomInActive = true;
        }
    }


    void Update()
    {
        if (player == null) return;

        float playerY = player.position.y;

        if (isZoomInActive)
        {
            if (playerY > zoomOutThreshold)
            {
                zoomOutCamera.Priority = ActivePriority;
                zoomInCamera.Priority = BasePriority;
                isZoomInActive = false;
            }
        }
        else if (playerY < zoomInThreshold)
        {
                zoomOutCamera.Priority = BasePriority;
                zoomInCamera.Priority = ActivePriority;
                isZoomInActive = true;
        }
    }
}