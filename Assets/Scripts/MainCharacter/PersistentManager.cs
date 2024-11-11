using UnityEngine;
                            
                        /* This script is currently being examined */

    // In order to keep _respawnPoint in PlayerDie.cs remains stable, we need Knight should not
    // be destroyed when scenes loaded (E.g: 1stScene -> DashboardScene, DashboardScene -> 1stScene) 

    // This script is applied to Knight and associated objects (E.g: Camera)
namespace MainCharacter
{
    public class PersistentManager : MonoBehaviour
    {
        private static PersistentManager _instance;

        private void Awake()
        {
            if (_instance is not null)
            {
                Destroy(gameObject); // Destroy duplicate instances
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}