using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public Camera[] playerCameras;
    int currentCameraIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        foreach(Camera c in playerCameras)
        {
            c.gameObject.SetActive(false);
        }
        playerCameras[currentCameraIndex].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame) SwitchCamera();
    }
    void SwitchCamera()
    {
        playerCameras[currentCameraIndex].gameObject.SetActive(false);
        currentCameraIndex = (currentCameraIndex + 1) % playerCameras.Length;
        playerCameras[currentCameraIndex].gameObject.SetActive(true);
    }
    public Camera GetCurrentCamera()
    {
        return playerCameras[currentCameraIndex];
    }
}
