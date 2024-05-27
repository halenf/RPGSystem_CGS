using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class BattleUnitUI : ObjectUI
    {
        protected BattleScene m_battleScene;
        protected BattleUnit m_battleUnit;
        protected HealthBarUI m_currentHealthBar;
        protected bool m_facingLeft;

        [SerializeField] protected TextMeshProUGUI m_unitNameDisplay;
        [SerializeField] protected Image m_unitSpriteDisplay;

        [Header("Turn Actions")]
        protected Button m_button;
        public Transform skillSlotUIContainer;

        public void Initialise(BattleScene battleScene, BattleUnit battleUnit, bool isEnemyUnit)
        {
            m_battleScene = battleScene;
            
            // set BattleUnit
            m_battleUnit = battleUnit;
            m_facingLeft = isEnemyUnit;

            // init button
            m_button = GetComponent<Button>();

            // Init health bar
            m_currentHealthBar = GetComponentInChildren<HealthBarUI>();
            if (m_currentHealthBar)
                m_currentHealthBar.unit = m_battleUnit;

            skillSlotUIContainer.gameObject.SetActive(false);

            UpdateUI();
        }

        protected void SendUnitAsUser()
        {
            m_battleScene.AddUserToAction(m_battleUnit.battleID);
            skillSlotUIContainer.gameObject.SetActive(true);
            Debug.Log(m_battleUnit.displayName + " selected as user.");
        }

        protected void SendUnitAsTarget()
        {
            m_battleScene.AddTargetToAction(m_battleUnit.battleID);
            Debug.Log(m_battleUnit.displayName + " selected as target.");
        }

        public void SetAsAvailableUser()
        {
            m_button.interactable = true;
            m_button.onClick.RemoveAllListeners();
            m_button.onClick.AddListener(SendUnitAsUser);
            skillSlotUIContainer.gameObject.SetActive(false);
        }

        public void SetAsAvailableTarget()
        {
            m_button.interactable = true;
            m_button.onClick.RemoveAllListeners();
            m_button.onClick.AddListener(SendUnitAsTarget);
            skillSlotUIContainer.gameObject.SetActive(false);
        }

        public void SetAsUnavailable()
        {
            m_button.interactable = false;
        }

        public override void UpdateUI()
        {           
            // update health bar display
            if (m_currentHealthBar)
                m_currentHealthBar.UpdateUI();

            // name display set to unit name
            m_unitNameDisplay.text = m_battleUnit.displayName;

            // if the unit should face the other way, flip it by inverting the scale
            if (m_facingLeft)
            {
                Vector3 scale = m_unitSpriteDisplay.rectTransform.localScale;
                m_unitSpriteDisplay.rectTransform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
        }
    }
}

