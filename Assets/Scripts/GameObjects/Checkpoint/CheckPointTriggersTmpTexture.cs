using GameObjects.Texture;
using UnityEngine;

namespace GameObjects.Checkpoint
{
    public class CheckPointTriggersTmpTexture : MonoBehaviour
    {
        public TemporaryTexture temporaryTexture;
        private const string PlayerTag = "Player";
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(PlayerTag)) return;
            temporaryTexture.playerIsOnTexture = true;
        }
    }
}