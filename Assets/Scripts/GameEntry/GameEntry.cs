using UnityEngine;

namespace Chanto
{
    public class GameEntry : MonoBehaviour
    {
        /// <summary>
        /// 红点管理器
        /// </summary>
        public static RedDotManager RedDot
        {
            get;
            private set;
        }

        void Awake()
        {
            RedDot = new RedDotManager();
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