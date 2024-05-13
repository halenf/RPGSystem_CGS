using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdamAssets
{
    public class EnemyMovement : MonoBehaviour
    {
        [Tooltip("How fast they go")]
        public float speed;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
    }
}