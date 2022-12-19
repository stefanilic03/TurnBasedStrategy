using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;

    [SerializeField] private float cameraSpeed = 10f;
    [SerializeField] private float cameraRotationSpeed = 100f;
    [SerializeField] private float cameraZoomSpeed = 10f;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    private const float MIN_ZOOM_Y = 2f;
    private const float MAX_ZOOM_Y = 12f;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        CameraMovement();
        CameraRotation();
        CameraZoom();
    }

    private void CameraMovement()
    {
        Vector3 inputMoveDirection = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.z = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.z = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x = 1f;
        }

        Vector3 moveVector = transform.forward * inputMoveDirection.z + transform.right * inputMoveDirection.x;
        transform.position += moveVector * cameraSpeed * Time.deltaTime;
    }

    private void CameraRotation()
    {
        Vector3 rotationVector = new Vector3();
        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y = 1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y = -1f;
        }
        transform.eulerAngles += rotationVector * cameraRotationSpeed * Time.deltaTime;
    }

    private void CameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= cameraZoomSpeed;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += cameraZoomSpeed;
        }
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_ZOOM_Y, MAX_ZOOM_Y);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, cameraZoomSpeed * Time.deltaTime);
    }
}
