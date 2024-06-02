using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGSystem
{
    public class MyBattleUnitUI : ObjectUI
    {
        private MyBattleScene m_battleScene;
        private MyBattleUnit m_battleUnit;
        private bool m_facingLeft;

        [SerializeField] private TextMeshProUGUI m_unitNameDisplay;
        [SerializeField] private Image m_unitSpriteDisplay;
        [SerializeField] private HealthBarUI m_healthBar;

        [Header("Turn Actions")]
        private Button m_button;
        public Transform skillSlotUIContainer;

        public void Initialise(MyBattleScene battleScene, MyBattleUnit battleUnit, bool isEnemyUnit)
        {
            m_battleScene = battleScene;
            
            // set BattleUnit
            m_battleUnit = battleUnit;
            m_facingLeft = isEnemyUnit;

            // init button
            m_button = GetComponent<Button>();

            // Init health bar
            m_healthBar.Initialise(m_battleUnit);

            skillSlotUIContainer.gameObject.SetActive(false);

            UpdateUI();
        }

        private void SendUnitAsUser()
        {
            m_battleScene.AddUserToCurrentAttackAction(m_battleUnit.battleID);
            skillSlotUIContainer.gameObject.SetActive(true);
            Debug.Log(m_battleUnit.displayName + " selected as user.");
        }

        private void SendUnitAsTarget()
        {
            m_battleScene.AddTargetToCurrentAttackAction(m_battleUnit.battleID);
            Debug.Log(m_battleUnit.displayName + " selected as target.");
        }

        public void SetAsAvailableUser()
        {
            m_button.interactable = true;
            m_button.onClick.RemoveAllListeners();
            m_button.onClick.AddListener(SendUnitAsUser);
        }

        public void SetAsAvailableTarget()
        {
            m_button.interactable = true;
            m_button.onClick.RemoveAllListeners();
            m_button.onClick.AddListener(SendUnitAsTarget);
        }

        public void SetAsUnavailable()
        {
            m_button.interactable = false;
        }

        public override void UpdateUI()
        {           
            // update health bar display
            if (m_healthBar)
                m_healthBar.UpdateUI();

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

