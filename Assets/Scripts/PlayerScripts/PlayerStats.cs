using System.Collections;
using CombatReportScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    public GameObject dmgGO;
    private Image dmgImg;
    private bool hit;

    [SerializeField]
    bool injured;

    public bool isSafe;
    public bool canDrop;

    [SerializeField]
    Movement movement;

    CombatReportBehaviour playerCombatReport;
    public int mapPieces;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) enabled = false;
    }

    void Start()
    {
        movement = GetComponent<Movement>();
        playerCombatReport = GetComponent<CombatReportBehaviour>();
        dmgGO = GameObject.Find("DamageImage");
        dmgImg = dmgGO.GetComponent<Image>();
        Debug.Log("Start");
    }

    void SendToPrison()
    {
        transform.position = Prison.instance.prisonTransform.position;
        playerCombatReport.combatReportData.vezesPreso++;
        Prison.instance.AddPrisonerRpc();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hit"))
        {
            Hit();
        }
    }

    void Hit()
    {
        if (!hit)
        {
            StartCoroutine(TakeDamage());
            if (!injured)
            {
                injured = true;
            }
            else
            {
                SendToPrison();
            }
        }
    }

    IEnumerator TakeDamage()
    {
        hit = true;
        playerCombatReport.combatReportData.vezesAtacado++;
        dmgImg.color = new Color(dmgImg.color.r, dmgImg.color.g, dmgImg.color.b, 1f);
        yield return new WaitForSeconds(0.3f);
        dmgImg.color = new Color(dmgImg.color.r, dmgImg.color.g, dmgImg.color.b, 0f);
        yield return new WaitForSeconds(1.7f);
        hit = false;
    }
}