using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.VaultDoor;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class buttonVault : MonoBehaviour
{
    [SerializeField] private short digit;

    private static List<short> code = new List<short>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Coisar();
        }
    }

    private static void Blarg ()
    {
        if (code.Count == 4)
        {
            CodigoFactory factory = FindObjectOfType<CodigoFactory>();
            Debug.Log(factory.CheckCodigo(code.ToArray()));
            code.Clear();
        }
    }

    private void Coisar()
    {
        Debug.Log("coisou");
        // com a câmera, raycast para frente
        foreach (var VARIABLE in Camera.allCameras)
        {
            Ray ray = VARIABLE.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10))
            {
                Debug.Log("coisou2");

                // se atingir esse objeto, ativa um evento de abrir a porta
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("coisou3");
                    code.Add(digit);
                    Blarg();
                }
            }
        }
    }
}
