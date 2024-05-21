using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    [Serializable]
    public abstract class SkillStatusEffect : ScriptableObject
    {
        // Integer data for the skill effect
        [SerializeField] protected int m_value;
        public int value
        {
            get
            {
                return m_value;
            }
        }

        public abstract void Effect(BattleMonster user, BattleMonster[] targets);
    }

    public enum DamageType
    {
        Light = 3,
        Medium = 5,
        Heavy = 8,
        Severe = 11
    }

    [Serializable]
    [CreateAssetMenu(fileName = "DamageEffect", menuName = "RPGSystem/Effects/Damage", order = 1)]
    public class DamageSkillEffect : SkillStatusEffect
    {
        [SerializeField] private DamageType m_damageType;
        public DamageType damageType
        {
            get
            {
                return m_damageType;
            }
        }

        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            foreach (BattleMonster target in targets)
            {
                int damage = (int)m_damageType / (target.defence + 10);
                int targetHP = target.currentHP;
                target.TakeDamage(damage);

                // if lifesteal is active, heal the user for the damage dealt
                if ((user.triggeredEffects & TriggeredEffect.Lifesteal) != 0)
                {
                    // If damage > targetHP, then there will be overkill damage
                    // Don't restore from overkill damage
                    if (damage > targetHP)
                        user.RestoreHealth(targetHP);
                    else
                        user.RestoreHealth(damage);
                }
            }
        }
    }

    public enum HealType
    {
        Target = 0,
        User = 1,
        Value
    }

    [Serializable]
    [CreateAssetMenu(fileName = "HealEffect", menuName = "RPGSystem/Effects/Heal", order = 1)]
    public class HealSkillEffect : SkillStatusEffect
    {
        [SerializeField] private HealType m_healType;
        public HealType healType
        {
            get
            {
                return m_healType;
            }
        }

        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            foreach (BattleMonster target in targets)
            {
                // Restores for a percentage of the target's maxHP, percentage of the user's maxHP, or a flat value
                float _value;
                switch (m_healType)
                {
                    case HealType.Target:
                        _value = m_value * target.maxHP / 100.0f;
                        break;
                    case HealType.User:
                        _value = m_value * user.maxHP / 100.0f;
                        break;
                    default:
                        _value = m_value;
                        break;
                }
                target.RestoreHealth((int)_value);
            }
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "Status", menuName = "RPGSystem/Effects/ApplyStatus", order = 1)]
    public class ApplyStatus : SkillStatusEffect
    {
        [SerializeField] private Status m_status;
        public Status status
        {
            get { return m_status; }
        }

        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            foreach (BattleMonster target in targets)
            {
                target.GainStatus(m_status);
            }
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "SkillCooldownEffect", menuName = "RPGSystem/Effects/SkillCooldown", order = 1)]
    public class SkillCooldownSkillEffect : SkillStatusEffect
    {
        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            foreach (BattleMonster target in targets)
            {
                for (int i = 0; i < 3; i++)
                {
                    target.ChangeSkillCooldown(i, m_value);
                }
            }
        }
    }
    [Serializable]
    [CreateAssetMenu(fileName = "StatusTimerEffect", menuName = "RPGSystem/Effects/StatusTimer", order = 1)]

    public class StatusTimerSkillEffect : SkillStatusEffect
    {
        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            foreach (BattleMonster target in targets)
            {
                for (int i = 0; i < target.statusSlots.Count; i++)
                {
                    target.ChangeStatusTimer(i, m_value);
                }
            }
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "StatEffect", menuName = "RPGSystem/Effects/Stat", order = 1)]
    public class StatSkillEffect : SkillStatusEffect
    {
        // note: HP cannot be boosted via this method
        [SerializeField] private MonsterBaseStats m_baseStat;
        public MonsterBaseStats baseStat
        {
            get { return m_baseStat; }
        }

        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            if (m_baseStat == MonsterBaseStats.Health)
                return;

            foreach (BattleMonster target in targets)
            {
                target.ApplyStatModifier(m_baseStat, m_value);
            }
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "EnableTriggeredEffect", menuName = "RPGSystem/Effects/EnableTriggeredEffect", order = 1)]
    public class EnableTriggeredEffect : SkillStatusEffect
    {
        // the effects to be enabled
        [SerializeField] private TriggeredEffect m_effect;
        public TriggeredEffect effect
        {
            get { return m_effect; }
        }

        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            foreach (BattleMonster target in targets)
                target.EnableTriggeredEffect(effect);
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "DisableTriggeredEffect", menuName = "RPGSystem/Effects/DisableTriggeredEffect", order = 1)]
    public class DisableTriggeredEffect : SkillStatusEffect
    {
        // the effects to be disabled
        [SerializeField] private TriggeredEffect m_effect;
        public TriggeredEffect effect
        {
            get { return m_effect; }
        }

        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            foreach (BattleMonster target in targets)
                target.DisableTriggeredEffect(m_effect);
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "ToggleTriggeredEffect", menuName = "RPGSystem/Effects/ToggleTriggeredEffect", order = 1)]
    public class ToggleTriggeredEffect : SkillStatusEffect
    {
        // the effects to be toggled
        [SerializeField] private TriggeredEffect m_effect;
        public TriggeredEffect effect
        {
            get { return m_effect; }
        }

        public override void Effect(BattleMonster user, BattleMonster[] targets)
        {
            foreach (BattleMonster target in targets)
                target.ToggleTriggeredEffect(m_effect);
        }
    }
}
