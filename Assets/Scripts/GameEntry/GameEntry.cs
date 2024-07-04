using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class GameEntry : MonoBehaviour
    {
        /// <summary>
        /// 红点管理器
        /// </summary>
        public static RedDotManager RedDotManager
        {
            get;
            private set;
        }

        void Awake()
        {
            RedDotManager = new RedDotManager();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}