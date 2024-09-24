using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Portrait : NetworkBehaviour
{
    private Renderer _renderer;
    private MeshRenderer[] childrenRenderers;
    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        childrenRenderers = GetComponentsInChildren<MeshRenderer>();
        if (IsServer)
        {
            StartCoroutine(MakeChildRendererFollowParentRenderer());
        }
    }

    private IEnumerator MakeChildRendererFollowParentRenderer()
    {
        while (true)
        {
            yield return new WaitUntil(() => !_renderer.enabled);
            EnableChildRendererRpc(false);
            yield return new WaitUntil(() => _renderer.enabled);
            EnableChildRendererRpc(true);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void EnableChildRendererRpc(bool b)
    {
        foreach (Renderer _childRenderer in childrenRenderers)
        {
            _childRenderer.enabled = b;
        }

        ClientRendererRpc(b);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void ClientRendererRpc(bool b)
    {
        foreach (Renderer _childRenderer in childrenRenderers)
        {
            _childRenderer.enabled = b;
        }
    }
}