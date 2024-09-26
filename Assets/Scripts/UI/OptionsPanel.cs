using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
   [SerializeField] private GameObject optionsPanel;
   [SerializeField] private Button NoButton;
   [SerializeField] private Button YesButton;

   private void Awake()
   {
         NoButton.onClick.AddListener(() =>
         {
            optionsPanel.gameObject.SetActive(false);
         });
         YesButton.onClick.AddListener(() =>
         {
            Application.Quit();
            Debug.Log("Quit");
         });
   }
   
   private void Start()
   {
      optionsPanel.SetActive(false);
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
         optionsPanel.gameObject.SetActive(true);
      }
   }
}
