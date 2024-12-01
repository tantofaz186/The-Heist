using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Netcode;

public class NightVision : BaseItem
{
    bool isNightVisionActive = false;
    bool coolDown = false;
    public Camera cam;
    private Volume volume;

    [SerializeField]
    VolumeProfile nightVisionProfile;

    [SerializeField]
    VolumeProfile defaultProfile;

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
            Invoke(nameof(CoolDown), 1.5f);
        }
    }

    protected override void Start()
    {
        base.Start();
        volume = GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>();
    }

    void TurnOnNightVision()
    {
        if (cam == null) return;

        isNightVisionActive = true;
        volume.profile = nightVisionProfile;
        cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Cam", "Water", "UI", "Ground", "PlayerLayer",
            "Keypad", "Door", "PlayerDontSee", "Obstacle", "VaultRoom", "Roof", "Bullet", "PostProcessing", "Enemy", "NightVision");
    }

    void TurnOffNightVision()
    {
        if (cam == null) return;
        isNightVisionActive = false;
        volume.profile = defaultProfile;
        cam.cullingMask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", "Cam", "Water", "UI", "Ground", "PlayerLayer",
            "Keypad", "Door", "Item", "PlayerDontSee", "Obstacle", "VaultRoom", "Roof", "Bullet", "PostProcessing", "Enemy");
    }

    void CoolDown()
    {
        coolDown = false;
    }

    public override void OnPick(ulong playerId)
    {
        base.OnPick(playerId);

        cam = NetworkManager.SpawnManager.GetPlayerNetworkObject(playerId).GetComponent<PlayerActions>()._camera;
    }

    public override void OnDrop()
    {
        TurnOffNightVision();
    }
}