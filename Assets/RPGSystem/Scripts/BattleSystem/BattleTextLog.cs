using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEditor;

namespace RPGSystem
{
    public class BattleTextLog : MonoBehaviour
    {
        // singleton/static accessor
        private static BattleTextLog m_instance;
        public static BattleTextLog Instance { get { return m_instance; } }
        
        // prefabs
        [SerializeField] protected TextMeshProUGUI m_linePrefab;
        [SerializeField] protected Transform m_lineContainer;

        // member variables
        private static List<string> m_linesToAdd = new List<string>();
        protected TextMeshProUGUI m_currentLine;

        protected void Awake()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(gameObject);
            }
            else
                m_instance = this;
        }

        public void AddLine(string text)
        {
            TextMeshProUGUI line = Instantiate(m_linePrefab, m_lineContainer);
            DateTime now = DateTime.Now;
            string currentTime =
                (now.Hour.ToString().Length > 1 ? now.Hour : "0" + now.Hour) + ":" +
                (now.Minute.ToString().Length > 1 ? now.Minute : "0" + now.Minute) + ":" +
                (now.Second.ToString().Length > 1 ? now.Second : "0" + now.Second);
            line.text = currentTime + " | " + text;
            m_currentLine = line;
        }
        public void ClearText()
        {
            for (int i = 0; i < m_lineContainer.childCount; i++)
                Destroy(m_lineContainer.GetChild(i).gameObject);
            m_currentLine = null;
        }

    }
}
