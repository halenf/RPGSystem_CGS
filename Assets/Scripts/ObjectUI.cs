using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem
{
    public abstract class ObjectUI : MonoBehaviour
    {
        /// <summary>
        /// Updates the graphics on the object.
        /// </summary>
        public abstract void UpdateUI();
    }
}
