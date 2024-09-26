using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatReportScripts;
using UnityEngine;

public class CombatReportUI : MonoBehaviour
{
    [SerializeField]
    private List<PlayerCombatReportUI> combatReport;
    bool ready = false;
    private void Awake()
    {
        combatReport = FindObjectsOfType<PlayerCombatReportUI>().ToList();
    }

    public void SetUI(List<CombatReportData> data)
    {
        combatReport.ForEach((cr) => cr.Apply(data));
        ready = true;
    }

    private void Start()
    {
        StartCoroutine(ShowInList());
    }

    private IEnumerator ShowInList()
    {
        yield return new WaitUntil(() => ready);
        foreach (var combatReportUI in combatReport)
        {
            combatReportUI.Hide();
        }

        while (true)
        {
            foreach (var c in combatReport)
            {
                c.Show();
                yield return new WaitForSeconds(3f);
                c.Hide();
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}