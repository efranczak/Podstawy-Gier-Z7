using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SnakeTensionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private SnakeScript _snakeScript;
    [SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private Volume _globalVolume;

    [Header("Zoom Settings (zalezne od dystansu)")]
    [Tooltip("Dystans, przy którym kamera jest normalna")]
    [SerializeField] private float _zoomSafeDistance = 45f;
    [Tooltip("Dystans, przy którym kamera jest maksymalnie oddalona")]
    [SerializeField] private float _zoomDangerDistance = 15f;

    [SerializeField] private float _normalOrthoSize = 5f;
    [SerializeField] private float _zoomedOutSize = 5.5f;

    [Header("Vignette Settings (zalezne od czasu)")]
    [Tooltip("Jesli wezowi zajeloby wiecej niz _safeTime sekund zeby dostac sie do gracza - bezpiecznie")]
    [SerializeField] private float _safeTime = 5f;

    [Tooltip("Jesli wezowi zajeloby _dangerTime sekund (lub mniej) zeby dostac sie do gracza - maksymalnie niebezpiecznie")]
    [SerializeField] private float _dangerTime = 1.0f;

    [Range(0f, 1f)]
    [SerializeField] private float _maxVignetteIntensity = 0.5f;

    private Vignette _vignette;
    private CinemachinePositionComposer _composer;
    private float _initialScreenY;

    private void Start()
    {
        if (_globalVolume.profile.TryGet(out Vignette vig))
        {
            _vignette = vig;
            _vignette.active = true;
            _vignette.intensity.overrideState = true;
        }

        if (_virtualCamera != null)
        {
            _composer = _virtualCamera.GetComponent<CinemachinePositionComposer>();
            if (_composer != null)
            {
                _initialScreenY = _composer.Composition.ScreenPosition.y;
            }
        }
    }

    private void LateUpdate()
    {
        if (_player == null || _snakeScript == null) return;

        float currentDistance = _player.position.x - _snakeScript.transform.position.x;
        float zoomT = Mathf.InverseLerp(_zoomSafeDistance, _zoomDangerDistance, currentDistance);

        if (_virtualCamera != null && _composer != null)
        {
            float newSize = Mathf.Lerp(_normalOrthoSize, _zoomedOutSize, zoomT);
            _virtualCamera.Lens.OrthographicSize = newSize;
        }

        if (_vignette != null)
        {
            float vignetteIntensity = 0f;

            float realSnakeSpeedPerSecond = _snakeScript.velocity / Time.fixedDeltaTime;

            if (realSnakeSpeedPerSecond > 0.01f)
            {
                float timeToImpact = currentDistance / realSnakeSpeedPerSecond;

                float timeT = Mathf.InverseLerp(_safeTime, _dangerTime, timeToImpact);

                vignetteIntensity = Mathf.Lerp(0f, _maxVignetteIntensity, timeT);
            }

            _vignette.intensity.value = vignetteIntensity;
        }
    }
}