using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.UI
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField]
        Image _fillingImage;
        [SerializeField]
        TMP_Text _text;
        public string Text
        {
            get => _text.text;
            set
            {
                _fillingImage.enabled = value == null || value.Length == 0;
                _text.text = value;
            }
        }
    }
}
