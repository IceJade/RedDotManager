using UnityEngine;

namespace Chanto
{
    public class RegisterRedDot : MonoBehaviour
    {
        public string Path;

        // Start is called before the first frame update
        void Start()
        {
            if (!string.IsNullOrEmpty(Path))
                GameEntry.RedDot.RegisterObject(Path, this.gameObject);
        }

        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(Path))
                GameEntry.RedDot.RemoveObject(Path, this.gameObject);
        }
    }
}