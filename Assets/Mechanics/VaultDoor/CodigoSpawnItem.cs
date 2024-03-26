using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mecanicas.PortaComCodigo
{
    public class CodigoSpawnItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        const string numbers = "_ _ _ _";
        private void Start()
        {
            text.text = "";
        }
        public void setText(int index, short value)
        {
            //o index é dobrado pois o espaço entre os números é um caractere
            text.text = numbers.Remove(2 * index, 1).Insert(2 * index, value.ToString());
        }
    }
}