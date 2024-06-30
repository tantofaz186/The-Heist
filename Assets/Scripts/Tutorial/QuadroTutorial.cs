using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadroTutorial : MonoBehaviour
{
    private int index;
    [SerializeField] private GameObject[] tutorialPanels;
    
    void Start()
    {
        index = 0;
        
        tutorialPanels[index].SetActive(true);
    }
    
    public void NextPanel()
    {    tutorialPanels[index].SetActive(false);
        index++;
        if (index >= tutorialPanels.Length)
        {   
            index = 0;
        }
        tutorialPanels[index].SetActive(true);
        
    }
    
    public void PreviousPanel()
    {
        tutorialPanels[index].SetActive(false);
        index--;
        if (index < 0)
        {
            index = tutorialPanels.Length - 1;
        }
        tutorialPanels[index].SetActive(true);
    }
    
}
