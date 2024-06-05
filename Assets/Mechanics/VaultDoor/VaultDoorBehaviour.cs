using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Mechanics.VaultDoor
{
    public class VaultDoorBehaviour: NetworkBehaviour
    {
        public event Action OnAlarmTrigger;
        public void Awake()
        {
            StartCoroutine(WaitToSpawn());
        }
        private void CodeCheck(bool isCorrectCode)
        {
            if (isCorrectCode) Open();
            else Alarm();
        }

        private IEnumerator WaitToSpawn()
        {
            yield return new WaitUntil(() => CodigoFactory.Instance != null);
            CodigoFactory.Instance.OnCodeChecked += CodeCheck;

        }

        private void Alarm()
        {
            Debug.Log("Alarme disparado");
            OnAlarmTrigger?.Invoke();
        }

        private void Open()
        {
            Debug.Log("Porta aberta");
            OpenRpc();
            CodigoFactory.Instance.OnCodeChecked -= CodeCheck;
        }
        [Rpc(SendTo.Everyone,RequireOwnership = false)]
        private void OpenRpc()
        { 
            StartCoroutine(RotateDoor());
        }

        private IEnumerator RotateDoor()
        {

            int totalAngle = 90;
            while (totalAngle > 0)
            {
                int angle = -3;
                gameObject.transform.Rotate(Vector3.up, angle);
                totalAngle += angle;
                yield return null;
            }
        }

        private void OnDisable()
        {
            CodigoFactory.Instance.OnCodeChecked -= CodeCheck;
        }
    }
}