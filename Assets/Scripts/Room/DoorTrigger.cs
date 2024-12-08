using Unity.Netcode;
using UnityEngine;

public class DoorTrigger : NetworkBehaviour
{
   [SerializeField] private Door door;

   private void OnTriggerEnter(Collider other)
   {
      if (other.TryGetComponent(out FOV _))
      {
         if (!door.isOpen.Value)
         {
            door.OpenServerRpc(other.transform.position);
         }
      }
   }
   
   private void OnTriggerExit(Collider other)
   {
      if (other.TryGetComponent(out FOV _))
      {
         if (door.isOpen.Value)
         {
            door.CloseServerRpc();
         }
      }
   }
}
