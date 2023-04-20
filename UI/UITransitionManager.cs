using System.Collections;
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

    public void TogglePanel(GameObject panel)
    {
        StartCoroutine(TogglePanelCoroutine(panel));
    }

    private IEnumerator TogglePanelCoroutine(GameObject panel)
    {
        yield return new WaitForSeconds(0.4f);
        panel.SetActive(!panel.activeSelf);
    }
}