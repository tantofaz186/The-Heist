using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]
    float force;

    [SerializeField]
    private float up;

    [SerializeField]
    EnemyRoman enemyRoman;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            enemyRoman.EndAttack(other, force);
            foreach (var relic in other.GetComponentsInChildren<PickupObject>())
            {
                relic.DropRelic(default);
            }
        }
    }
}