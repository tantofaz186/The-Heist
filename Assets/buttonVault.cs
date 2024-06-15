using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.VaultDoor;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class buttonVault : MonoBehaviour
{
    [SerializeField]
    private short digit;

    private static List<short> code = new List<short>();

    private void Start()
    {
        PlayerActionsSingleton.Instance.PlayerInputActions.Player.Use.performed += AtivarBotão;
        startPosition = transform.localPosition;
    }

    private void OnDisable()
    {
        PlayerActionsSingleton.Instance.PlayerInputActions.Player.Use.performed -= AtivarBotão;
    }

    private static void CheckCode()
    {
        if (code.Count != 4) return;
        CodigoFactory factory = FindObjectOfType<CodigoFactory>();
        Debug.Log(factory.CheckCodigo(code.ToArray()));
        code.Clear();
    }

    private void AtivarBotão(InputAction.CallbackContext callbackContext)
    {
        // com a câmera, raycast para frente

        Ray ray = PlayerActionsSingleton.Instance._camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Keypad")))
        {
            if (hit.collider.gameObject == gameObject && !pressed)
            {
                code.Add(digit);
                StartCoroutine(AnimateButton());
                CheckCode();
            }
        }
    }

    private Vector3 startPosition;
    private Vector3 targetPosition;
    bool pressed = false;

    private IEnumerator AnimateButton()
    {
        pressed = true;
        var time = 0f;
        var totalTime = 0.4f;
        targetPosition = startPosition + Vector3.forward * 0.03f;
        while (time < totalTime)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / totalTime);
            time += Time.deltaTime;
            yield return null;
        }

        while (time > 0)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / totalTime);
            time -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPosition;
        pressed = false;
    }
}