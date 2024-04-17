using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.VaultDoor
{
    public class VaultDoorBehaviour: MonoBehaviour
    {
        [SerializeField] private CodigoFactory codigoFactory;
        public event Action OnAlarmTrigger;
        public void Awake()
        {
            codigoFactory = FindObjectOfType<CodigoFactory>();
            codigoFactory.OnCodeChecked += CodeCheck;
        }
        private void CodeCheck(bool isCorrectCode)
        {
            if (isCorrectCode) Open();
            else Alarm();
        }

        private void Alarm()
        {
            Debug.Log("Alarme disparado");
            OnAlarmTrigger?.Invoke();
        }

        private void Open()
        {
            Debug.Log("Porta aberta");
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
    }
}