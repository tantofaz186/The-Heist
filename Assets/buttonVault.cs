using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        startPosition = transform.localPosition;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Use.performed += AtivarBotão;

    }
    private PlayerInputActions playerInputActions;

    private void OnDisable()
    {
        playerInputActions.Player.Use.performed -= AtivarBotão;
        playerInputActions.Disable();
    }

    private static void CheckCode()
    {
        CodigoFactory factory = FindObjectOfType<CodigoFactory>();
        factory.CheckCodigo(code.ToArray());
        code.Clear();
    }

    private void AtivarBotão(InputAction.CallbackContext callbackContext)
    {
        // com a câmera, raycast para frente

        Camera __camera = (from pa in FindObjectsByType<PlayerActions>(FindObjectsSortMode.None) where pa.IsOwner select pa._camera).FirstOrDefault();
        if(__camera == null) return;
        Ray ray = __camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (!Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Keypad"))) return;
        if (hit.collider.gameObject != gameObject || pressed) return;
        switch (digit)
        {
            case -1:
                code.Clear();
                break;
            case 10:
                CheckCode();
                break;
            default:
                code.Add(digit);
                StartCoroutine(AnimateButton());
                break;
                    
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