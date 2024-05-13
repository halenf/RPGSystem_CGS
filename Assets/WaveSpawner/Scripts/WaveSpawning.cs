using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdamAssets
{
    public class WaveSpawning : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        public List<Wave> waves = new List<Wave>();

        int m_waveIndex;

        [System.Serializable]
        public class Wave
        {
            public GameObject enemy;
            public int count;
            public float seperation;
            public float cooldown;

            int m_index;
            float m_timer;
            float m_cooldownTimer;

            /// <summary>
            /// Spawn timer for this wave
            /// </summary>
            /// <param name="spawnPoint"></param>
            /// <returns></returns>
            public bool Update(Transform spawnPoint)
            {
                //Enemy spawn timer
                if (m_index < count)
                    m_timer += Time.deltaTime;
                else
                    m_cooldownTimer += Time.deltaTime;

                //Spawn enemy
                if (m_timer > seperation)
                {
                    m_timer -= seperation;
                    m_index++;
                    Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
                }

                //Next wave
                return m_cooldownTimer > cooldown;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //If we're not on the last wave, move to the next wave
            if (m_waveIndex < waves.Count)
            {
                if (waves[m_waveIndex].Update(spawnPoint))
                {
                    m_waveIndex++;
                }
            }
        }
    }
}
