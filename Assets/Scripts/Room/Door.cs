using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;
    [SerializeField] bool isRotatingDoor = true;
    [SerializeField] private float speed = 1f;

    [Header("Rotation Configs")] 
    [SerializeField] private float rotationAmount = 90f;

    [SerializeField] private float forwardDirection = 0;
    
    
    [Header("Sliding Configs")] 
    [SerializeField] Vector3 slideDirection  = Vector3.back;
    [SerializeField] float slideAmount = 1.9f;
    
    
    private Vector3 StartRotation;
    private Vector3 Forward;
    private Vector3 StartPosition;

    private Coroutine AnimationCoroutine;

    private void Awake()
    {
        StartRotation = transform.rotation.eulerAngles;
        Forward = transform.right;
        StartPosition = transform.position;
        
    }

  public  void Open(Vector3 UserPosition)
    { 
        if (!isOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (isRotatingDoor)
            {   
                float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized);
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
        if(forwardAmount>=forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0,StartRotation.y-rotationAmount,0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0,StartRotation.y+rotationAmount,0));
        }

        isOpen = true;
        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
            
        }
    }

    public void Close()
    {
        
        if (isOpen)
        {
            if(AnimationCoroutine!=null)
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

    IEnumerator SlideOpen()
    {
        Vector3 endPosition = StartPosition + slideAmount * slideDirection;
        Vector3 startPosition = transform.position;
        isOpen = true;
        float time = 0;
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
        isOpen = false;
        float time = 0;
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
        isOpen = false;
        float time = 0;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }
}
