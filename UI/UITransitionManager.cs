using UnityEngine;
using Cinemachine;

public class UITransitionManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _currentCamera;

    // Start is called before the first frame update
    private void Start()
    {
        _currentCamera.Priority = 11;
    }

    public void UpdateCamera(CinemachineVirtualCamera targetCamera)
    {
        _currentCamera.Priority--;

        _currentCamera = targetCamera;

        _currentCamera.Priority++;
    }
}