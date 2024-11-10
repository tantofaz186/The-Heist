using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class NightVision : BaseItem
{
    bool isNightVisionActive = false;
    bool coolDown = false;
    public Camera cam;
    private Volume volume;
    [SerializeField] VolumeProfile nightVisionProfile;
    [SerializeField] VolumeProfile defaultProfile;
    
    public override void UseItem()
    {
        if (!coolDown)
        {
            if (isNightVisionActive)
            {
                TurnOffNightVision();
            }
            else
            {
                TurnOnNightVision();
            }
            coolDown = true;
            Invoke(nameof(CoolDown),1.5f);
        }
    }

    void Start()
    { 
        base.Start();
        volume = GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>();
    }

    
    void TurnOnNightVision()
    {
        isNightVisionActive = true;
        volume.profile = nightVisionProfile;
        cam.cullingMask = (1 << 19);
    }
    void TurnOffNightVision()
    {
        isNightVisionActive = false;
        volume.profile = defaultProfile;
        cam.cullingMask &= ~(1 << 19);
    }

    void CoolDown()
    {
        coolDown = false;
    }

    public override void OnPick(ulong playerId)
    {
        cam =NetworkManager.SpawnManager.GetPlayerNetworkObject(playerId).GetComponent<PlayerActions>()._camera;
    }
    
    public override void OnDrop()
    {
            TurnOffNightVision();
            cam = null;
    }
}
