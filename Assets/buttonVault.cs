using System.Collections;
using System.Collections.Generic;
using Mechanics.VaultDoor;
using UnityEngine;

public class buttonVault : MonoBehaviour
{
    [SerializeField] private short digit;

    private static List<short> code = new List<short>();

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Coisar();
        }
    }
    private static void CheckCode ()
    {
        if (code.Count != 4) return;
        CodigoFactory factory = FindObjectOfType<CodigoFactory>();
        Debug.Log(factory.CheckCodigo(code.ToArray()));
        code.Clear();
    }

    private void Coisar()
    {
        // com a câmera, raycast para frente
        foreach (var _camera in Camera.allCameras)
        {
            Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 5))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    code.Add(digit);
                    StartCoroutine(AnimateButton());
                    CheckCode();
                }
            }
        }
    }

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private IEnumerator AnimateButton()
    {
        var time = 0f;
        var totalTime = 0.4f;
        targetPosition = startPosition + transform.forward * 0.03f;
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
    }
}
