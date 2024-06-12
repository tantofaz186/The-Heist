using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeLocation : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>().isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>().isSafe = false;
        }
    }
}
