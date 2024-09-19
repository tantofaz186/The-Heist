using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CombatReportScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerCombatReportUI : MonoBehaviour
{
    public CombatReportType type;
    public List<TextMeshProUGUI> texts;
    public List<Image> images;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            texts.Add(transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>());
            images.Add(transform.GetChild(i).GetComponentInChildren<Image>());
        }
    }

    public void Start()
    {
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].text = i.ToString();
        }

        for (int i = 0; i < images.Count; i++)
        {
            images[i].color = Random.ColorHSV();
        }
    }

    public void Hide()
    {
        transform.parent.gameObject.SetActive(false);
    }

    public void Show()
    {
        transform.parent.gameObject.SetActive(true);
    }

    public void Apply(List<CombatReportData> combatReports)
    {
        List<Tuple<Color, string>> dataOrdered = GetRelevantData(combatReports);

        ApplyColors(dataOrdered.Select(x => x.Item1).ToList());
        ApplyTexts(dataOrdered.Select(x => x.Item2).ToList());
    }

    private List<Tuple<Color, string>> GetRelevantData(List<CombatReportData> combatReports)
    {
        List<Tuple<Color, string>> dataOrdered = new List<Tuple<Color, string>>();
        switch (type)
        {
            case CombatReportType.PRISOES:
                combatReports.Sort((cr1, cr2) => cr1.vezesPreso.CompareTo(cr2.vezesPreso));
                for (int i = 0; i < combatReports.Count; i++)
                {
                    dataOrdered.Add(new Tuple<Color, string>(combatReports[i].playerColor,
                        combatReports[i].playerName.IsEmpty ? "" : $"{combatReports[i].playerName} : {combatReports[i].vezesPreso}"));
                }
                break;
            case CombatReportType.DISTANCIA:
                combatReports.Sort((cr1, cr2) => cr2.distanciaPercorrida.CompareTo(cr1.distanciaPercorrida));
                for (int i = 0; i < combatReports.Count; i++)
                {
                    dataOrdered.Add(new Tuple<Color, string>(combatReports[i].playerColor,
                        combatReports[i].playerName.IsEmpty ? "" : $"{combatReports[i].playerName} : {combatReports[i].distanciaPercorrida:F2} Metros"));
                }

                break;
            case CombatReportType.ITEMS:
                combatReports.Sort((cr1, cr2) => cr2.itensColetados.CompareTo(cr1.itensColetados));
                for (int i = 0; i < combatReports.Count; i++)
                {
                    dataOrdered.Add(new Tuple<Color, string>(combatReports[i].playerColor,
                        combatReports[i].playerName.IsEmpty ? "" : $"{combatReports[i].playerName} : {combatReports[i].itensColetados}"));
                }

                break;
            case CombatReportType.MONEY:
                combatReports.Sort((cr1, cr2) => cr2.dinheiroRecebido.CompareTo(cr1.dinheiroRecebido));
                for (int i = 0; i < combatReports.Count; i++)
                {
                    dataOrdered.Add(new Tuple<Color, string>(combatReports[i].playerColor,
                        combatReports[i].playerName.IsEmpty ? "" : $"{combatReports[i].playerName} : {combatReports[i].dinheiroRecebido}"));
                }
                break;
            default:
                combatReports.Sort((cr1, cr2) => cr1.playerID.CompareTo(cr2.playerID));
                for (int i = 0; i < combatReports.Count; i++)
                {
                    dataOrdered.Add(new Tuple<Color, string>(combatReports[i].playerColor,
                        combatReports[i].playerName.IsEmpty ? "" : $"{combatReports[i].playerName} : {combatReports[i].playerID}"));
                }

                break;
        }

        return dataOrdered;
    }

    public void ApplyTexts(List<string> sortedTexts)
    {
        for (int i = 0; i < sortedTexts.Count; i++)
        {
            texts[i].gameObject.SetActive(true);
            texts[i].text = sortedTexts[i];
        }

        for (int i = sortedTexts.Count; i < texts.Count; i++)
        {
            texts[i].gameObject.SetActive(false);
        }
    }

    public void ApplyColors(List<Color> colors)
    {
        for (int i = 0; i < colors.Count; i++)
        {
            images[i].gameObject.SetActive(true);
            images[i].color = colors[i];
        }

        for (int i = colors.Count; i < images.Count; i++)
        {
            images[i].gameObject.SetActive(false);
        }
    }
}

public enum CombatReportType
{
    PRISOES,
    DISTANCIA,
    ITEMS,
    MONEY,
}