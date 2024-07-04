using UnityEngine;

namespace Tetris
{
    public class RegisterRedDot : MonoBehaviour
    {
        public string Path;

        // Start is called before the first frame update
        void Start()
        {
            if (!string.IsNullOrEmpty(Path))
                GameEntry.RedDotManager.RegisterObject(Path, this.gameObject);
        }

        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(Path))
                GameEntry.RedDotManager.RemoveObject(Path, this.gameObject);
        }
    }
}