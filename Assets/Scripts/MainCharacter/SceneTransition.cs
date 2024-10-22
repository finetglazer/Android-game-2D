using Recorder;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainCharacter
{
    public class SceneTransition : MonoBehaviour
    {
        public string sceneName; 
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            DeathNote.ClearLists();
            SceneManager.LoadScene(sceneName);
        }
    }
}