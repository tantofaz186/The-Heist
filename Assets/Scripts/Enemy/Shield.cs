using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]float force;
    [SerializeField] private float up;
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {     
         Vector3 direction = transform.right * force;
         direction.y +=up;
         other.GetComponent<Rigidbody>().AddForce(direction, ForceMode.VelocityChange);
         foreach (var relic in other.GetComponentsInChildren<PickupObject>())
         {
            relic.DropRelic(default);
         }
      }
   }
}
