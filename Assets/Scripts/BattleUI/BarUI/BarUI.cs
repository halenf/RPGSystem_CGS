using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public abstract class BarUI : ObjectUI
    {
        [SerializeField] protected Slider m_slider;
        [SerializeField] protected TextMeshProUGUI m_textValueDisplay;
    }
}

