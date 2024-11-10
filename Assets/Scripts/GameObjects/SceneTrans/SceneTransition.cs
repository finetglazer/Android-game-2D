﻿using Recorder;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameObjects.SceneTrans
{
    public class SceneTransition : MonoBehaviour
    {
        public string sceneName; 
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            DeathNote.ClearLists();
            
            //TODO: Add updateFinishTime and updateScenePoint APIs
            SceneManager.LoadScene(sceneName);
        }
    }
}