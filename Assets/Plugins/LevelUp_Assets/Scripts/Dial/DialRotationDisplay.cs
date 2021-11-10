using UnityEngine;
using TMPro;

namespace LevelUP.Dial
{
    public class DialRotationDisplay : MonoBehaviour, IDial
    {
        [SerializeField] TextMeshProUGUI textBox;

        public void DialChanged(float dialValue)
        {
            textBox.text = dialValue.ToString("f2");
        }
    }
}
