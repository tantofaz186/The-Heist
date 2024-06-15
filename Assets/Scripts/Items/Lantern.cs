using UnityEngine;

public class Lantern : BaseItem
{   public bool isOn = false;
    public GameObject _light;
    public override void UseItem()
    {
        if (!isOn)
        {
            //light.SetActive(true);
            Debug.Log("Lanterna Ligada");
            isOn = true;
        }
        else
        {
           // light.SetActive(false);
           Debug.Log("Lanterna Desligada");
            isOn = false;
        }
    }
}
