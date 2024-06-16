using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorOn : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
