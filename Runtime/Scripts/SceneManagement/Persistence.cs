using UnityEngine;

namespace SceneManagement
{
    public class Persistence : MonoBehaviour
    {
        public static Persistence Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}