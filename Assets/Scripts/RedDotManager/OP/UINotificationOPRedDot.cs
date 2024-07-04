using UnityEngine;


namespace Tetris
{
    // 红点通知操作：显隐Object
    public class UINotificationOPRedDot : IUINotificationOP
    {
        public int instanceID;
        public string path;
        public int nodeHash;
        public GameObject gameObject;

        public UINotificationOPRedDot(int nodeHash, GameObject gameObject)
        {
            this.nodeHash = nodeHash;
            this.gameObject = gameObject;
            if (gameObject != null)
            {
                instanceID = gameObject.GetInstanceID();
            }
        }

        public override void OnNotification(string path, int nodeHash, bool show)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(show);
            }
        }
    }
}
