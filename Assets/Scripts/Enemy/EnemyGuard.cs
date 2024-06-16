/*
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyGuard : NetworkBehaviour
{
    private Enemy enemy;

    private Transform prision;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.OnAttack += OnAttack;
    }

    private void OnAttack(GameObject obj)
    {
        SendToPrision(obj);
    }

    private void SendToPrision(GameObject obj)
    {
        if (prision == null)
        {
            prision = GameObject.Find("PrisãoSpawn").transform;
        }
        obj.transform.position = prision.position;
    }
    private void OnDisable()
    {
        enemy.OnAttack -= OnAttack;
    }
}
*/

