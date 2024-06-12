using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : Singleton<MonoBehaviour>
{
   [SerializeField] TextMeshProUGUI timerText;
   [SerializeField] float remainingTime;

   private void Start()
   {
       timerText.gameObject.SetActive(true);
   }

   void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime-= Time.deltaTime;
            if(remainingTime<=60)
            {
                OneMinute();
            }
            
        }
        else
        {
            StopGame();            
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
   
   void OneMinute()
   {
       timerText.color = Color.red;
   }
   

     void StopGame()
    {
        Debug.Log("Stop Game");
        remainingTime= 0;
    }
}
