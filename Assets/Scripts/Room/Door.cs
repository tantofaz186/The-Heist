using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour, Interactable
{
    public NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);

    [SerializeField]
    bool isRotatingDoor = true;

    [SerializeField]
    private float speed = 1f;

    [Header("Rotation Configs")]
    [SerializeField]
    private float rotationAmount = 90f;

    [SerializeField]
    private float forwardDirection = 0;

    [Header("Sliding Configs")]
    [SerializeField]
    Vector3 slideDirection = Vector3.back;

    [SerializeField]
    float slideAmount = 1.9f;

    private Vector3 StartRotation;
    private Vector3 Forward;
    private Vector3 StartPosition;

    private Coroutine AnimationCoroutine;

    private void Awake()
    {
        StartRotation = transform.rotation.eulerAngles;
        Forward = -transform.forward;
        StartPosition = transform.position;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void OpenServerRpc(Vector3 UserPosition)
    {
        OpenRpc(UserPosition);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void CloseServerRpc()
    {
        CloseRpc();
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void OpenRpc(Vector3 UserPosition)
    {
        if (!isOpen.Value)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (isRotatingDoor)
            {
                float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized);
                Debug.Log($"Dot: {dot.ToString("N3")}");
                AnimationCoroutine = StartCoroutine(RotateDoor(dot));
            }
            else
            {
                AnimationCoroutine = StartCoroutine(SlideOpen());
            }
        }
    }

    IEnumerator RotateDoor(float forwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;
        if (forwardAmount >= forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - rotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + rotationAmount, 0));
        }

        ServerOpenRpc();
        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void CloseRpc()
    {
        if (isOpen.Value)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (isRotatingDoor)
            {
                AnimationCoroutine = StartCoroutine(RotateClose());
            }
            else
            {
                AnimationCoroutine = StartCoroutine(SlideClose());
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void ServerCloseRpc()
    {
        isOpen.Value = false;
    }

    [Rpc(SendTo.Server)]
    private void ServerOpenRpc()
    {
        isOpen.Value = true;
    }

    IEnumerator SlideOpen()
    {
        Debug.Log("tentei");
        Vector3 endPosition = StartPosition + slideAmount * slideDirection;
        Vector3 startPosition = transform.position;
        float time = 0;
        ServerOpenRpc();
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    IEnumerator RotateClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);
        float time = 0;
        ServerCloseRpc();
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    IEnumerator SlideClose()
    {
        Vector3 endPosition = StartPosition;
        Vector3 startPosition = transform.position;
        ServerCloseRpc();
        float time = 0;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    public string getDisplayText()
    {
        return isOpen.Value ? "Close " : "Open ";
    }

    public void Interact()
    {
        if (isOpen.Value)
        {
            CloseServerRpc();
        }
        else
        {
            OpenServerRpc(transform.position);
        }
    }

    #region Editor

    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Door))]
    public class DoorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Door door = (target as Door)!;
            if (GUILayout.Button("Open"))
            {
                door.OpenServerRpc(door.Forward);
            }

            if (GUILayout.Button("Close"))
            {
                door.CloseServerRpc();
            }
        }
    }

    #endif

    #endregion
}