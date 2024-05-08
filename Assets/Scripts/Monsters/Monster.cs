using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public class SkillSlot
    {
        public SkillSlot(Skill _skill, int _turnTimer)
        {
            m_skill = _skill;
            m_turnTimer = _turnTimer;
        }

        [SerializeField] private Skill m_skill;
        [SerializeField] private int m_turnTimer;

        public Skill skill
        {
            get { return m_skill; }
        }
        public int turnTimer
        {
            get { return m_turnTimer; }
            set
            {
                if (value >= 0)
                    m_turnTimer = value;
            }
        }

        public void ResetTimer()
        {
            m_turnTimer = 0;
        }

        public void ClearSlot()
        {
            m_skill = null;
            m_turnTimer = 0;
        }
    }

    [Serializable]
    public class StatusSlot
    {
        public StatusSlot(Status status, int turnTimer)
        {
            m_status = status;
            m_turnTimer = turnTimer;
        }

        [SerializeField] private Status m_status;
        [SerializeField] private int m_turnTimer;

        public Status status
        {
            get { return m_status; }
        }
        public int turnTimer
        {
            get { return m_turnTimer; }
            set
            {
                if (value >= 0)
                    m_turnTimer = value;
            }
        }

        public void OnApply(Monster target)
        {
            foreach (SkillStatusEffect effect in m_status.onApply)
            {
                effect.Effect(m_status.user, new Monster[] { target });
            }
        }

        public void OnTurnEnd(Monster target)
        {
            foreach (SkillStatusEffect effect in m_status.onTurnEnd)
            {
                effect.Effect(m_status.user, new Monster[] { target });
            }
        }

        public void OnClear(Monster target)
        {
            // if clear effects on this target should not fail
            if ((target.triggeredEffects & TriggeredEffect.FailStatusClearEffects) == 0)
            {
                foreach (SkillStatusEffect effect in m_status.onClear)
                {
                    effect.Effect(m_status.user, new Monster[] { target });
                }
            }
            else
                target.ToggleTriggeredEffect(TriggeredEffect.FailStatusClearEffects);
        }

        public void ClearSlot()
        {
            m_status = null;
            m_turnTimer = 0;
        }
    }

    [Serializable]
    [Flags] public enum TriggeredEffect
    {
        None = 0,
        FailNextSkillEffect = 1,
        FailStatusClearEffects = 2, // Implemented
        DamageOnSkillUse = 4,
        DebuffImmunity = 8, // Implemented
        Lifesteal = 16 // Implemented
    }
    
    [CreateAssetMenu(fileName = "Monster", menuName = "RPGSystem_SO/Monsters/Monster", order = 1)]
    public class Monster : ScriptableObject
    {
        // monster data
        [SerializeField] private MonsterData m_monsterData;
        public MonsterData monsterData
        {
            get { return m_monsterData; }
        }

        [SerializeField] private string m_monsterName;
        public string monsterName
        {
            get { return m_monsterName; }
        }

        // skill and status
        [SerializeField] private SkillSlot[] m_skillSlots = new SkillSlot[3];
        [SerializeField] private List<StatusSlot> m_statusSlots = new List<StatusSlot>();

        public SkillSlot[] skillSlots
        {
            get
            {
                return m_skillSlots;
            }
        }
        public List<StatusSlot> statusSlots
        {
            get
            {
                return m_statusSlots;
            }
        }

        // tracks stat modifiers from buffs
        [SerializeField] private float[] m_statModifiers = new float[4] { 1, 1, 1, 1 } ;

        // bitwise enum for effects with triggers
        [SerializeField] private TriggeredEffect m_triggeredEffects;
        public TriggeredEffect triggeredEffects
        {
            get
            {
                return m_triggeredEffects;
            }
        }

        // exp/level
        [SerializeField] private int m_exp;
        public int exp
        {
            get
            {
                return m_exp;
            }
        }
        public int level
        {
            get
            {
                float curveScalar = Mathf.Log((float)monsterData.levelCurve * 10.0f / 3.0f) / 2 * Mathf.Log(10);
                return (int)MathF.Pow(10.0f / 3.0f * m_exp, 1 / curveScalar);
            }
            set
            {
                float curveScalar = Mathf.Log((float)monsterData.levelCurve * 10.0f / 3.0f) / 2 * Mathf.Log(10);
                m_exp = (int)(3.0f * Mathf.Pow(value, curveScalar) / 10.0f);
            }
        }

        public bool GainExp(int value)
        {
            // monster should not be able to gain more exp than the max of their level curve
            if (m_exp >= (int)monsterData.levelCurve)
            {
                m_exp = (int)monsterData.levelCurve;
                return false;
            }
            else
            {
                m_exp += value;
                return true;
            }
        }

        public int ExpWorth()
        {
            float curveScalar = Mathf.Log((int)monsterData.levelCurve * 10.0f / 3.0f) / 2 * Mathf.Log(10);
            return (int)Mathf.Pow(level, curveScalar / 1.7f);
        }

        // base stat accessors
        public int health
        {
            get
            {
                return monsterData.baseStats.health;
            }
        }

        public int strength
        {
            get
            {
                return monsterData.baseStats.strength;
            }
        }

        public int fortitude
        {
            get
            {
                return monsterData.baseStats.fortitude;
            }
        }

        public int agility
        {
            get
            {
                return monsterData.baseStats.agility;
            }
        }

        // volatile stat accessors
        private int m_currentHP;
        public int maxHP
        {
            get
            {
                return (int)(health * Mathf.Pow(1000 * (level + 1), 0.4f));
            }
        }
        public int currentHP
        {
            get
            {
                return m_currentHP;
            }
        }
        public int attack
        {
            get
            {
                return (int)((30 * strength * (level + 1) / 100 + 10) * m_statModifiers[(int)MonsterBaseStats.Strength]);
            }
        }
        public int defence
        {
            get
            {
                return (int)((30 * fortitude * (level + 1) / 100 + 10) * m_statModifiers[(int)MonsterBaseStats.Fortitude]);
            }
        }
        public int speed
        {
            get
            {
                return (int)((30 * agility * (level + 1)/ 100 + 10) * m_statModifiers[(int)MonsterBaseStats.Agility]);
            }
        }

        // public methods for battle scene
        public void BattleResetMonster()
        {
            // clear stat buffs/debuffs
            for (int i = 1; i < 4; i++)
                m_statModifiers[i] = 1;

            // reset HP
            m_currentHP = maxHP;
            
            // reset all skill cooldowns
            foreach (SkillSlot skillSlot in m_skillSlots)
            {
                skillSlot.ResetTimer();
            }

            // clear statuses
            m_statusSlots.Clear();

            // clear triggered effects
            m_triggeredEffects = 0;
        }

        // public methods for skill effects
        public void TakeDamage(int damage)
        {
            // reduce HP
            m_currentHP -= damage;

            if (m_currentHP <= 0)
            {
                // kill TODO
            }
        }

        public void RestoreHealth(int value)
        {
            // increase HP, don't allow monster to have HP over the maximum
            m_currentHP += value;

            if (m_currentHP > maxHP)
                m_currentHP = maxHP;
        }

        public void ChangeStat(MonsterBaseStats baseStat, int value)
        {
            // apply percentage multiplier to specified stat (compounding with other boosts)
            // note: negative values represent a debuff

            // turns value into a 1.X multiplier where X is value
            // or 1 / 1.X value, if debuff
            float _value;
            if (value < 0)
                _value = -1.0f / (value / 100.0f + 1); // make positive, get reciprocal
            else
                _value = value / 100.0f + 1;
            
            m_statModifiers[(int)baseStat] *= _value;
        }

        public void GainStatus(Status status)
        {
            // if target does not have debuff immunity
            if ((m_triggeredEffects & TriggeredEffect.DebuffImmunity) == 0)
            {
                // add new status to status list, then call its on apply affects
                m_statusSlots.Add(new StatusSlot(status, status.turnTimer));
                m_statusSlots.Last().OnApply(this);
            }
            // if target does have debuff immunity, then don't apply the status
        }

        public void ChangeSkillCooldown(int index, int value)
        {
            // increase/decrease the turn timer of the skill at the indicated index
            m_skillSlots[index].turnTimer += value;
        }

        public void ChangeStatusTimer(int index, int value)
        {
            // increase/decrease the turn timer of the status at the indicated index
            m_statusSlots[index].turnTimer += value;
        }

        public void ToggleTriggeredEffect(TriggeredEffect effect)
        {
            m_triggeredEffects ^= effect;
        }

        // functional methods
        [ContextMenu("Set Level 16")]
        public void SetLevel()
        {
            level = 16;
        }

        [ContextMenu("Reset Monster")]

        public void ResetMonster()
        {
            m_monsterData = null;
            m_monsterName = string.Empty;
            m_skillSlots = null;
            m_statusSlots = null;
            m_statModifiers = null;
            m_triggeredEffects = 0;
            m_exp = 0;
            m_currentHP = 0;
        }
    }
}
