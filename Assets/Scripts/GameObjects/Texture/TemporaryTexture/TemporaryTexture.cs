using Recorder;
using UnityEngine;

namespace GameObjects.Texture.TemporaryTexture
{
    public class TemporaryTexture : MonoBehaviour
    {
        public bool playerIsOnTexture;
        public float timeOnTexture;
        public bool textureActive = true;
        public float duration = 3f; // Time required for the texture to disappear.

        private Vector3 _initialPosition;
        private bool _addedToDeathNote;
    
        private void Start()
        {
            // Store the initial position of the texture
            _initialPosition = transform.position;
        }
    
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if the player collides with the texture's collider.
            if (collision.collider.CompareTag("Player") && textureActive)
            {
                playerIsOnTexture = true;
            }
        }

        // private void OnCollisionExit2D(Collision2D collision)
        // {
        //     // Reset when the player leaves the texture's collider.
        //     if (collision.collider.CompareTag("Player") && textureActive)
        //     {
        //         playerIsOnTexture = false;
        //         // timeOnTexture = 0f; // Reset the timer
        //     }
        // }

        private void Update()
        {
            if (playerIsOnTexture && textureActive)
            {
                if (!_addedToDeathNote)
                {
                    DeathNote.AddObject(gameObject, _initialPosition);
                    _addedToDeathNote = true;
                }
                timeOnTexture += Time.deltaTime; // Increment the time spent on the texture.
                // print(_timeOnTexture);
                print("Clock starts to run: " + gameObject.name);
                if (!(timeOnTexture >= duration)) return;
            
                print(gameObject.name + " disappeared");
                // Deactivate the texture instead of destroying it
                gameObject.SetActive(false);
                textureActive = false;
            }
        }
    
        public void ResetTexture()
        {   
            print("Resetting texture");
            // Reset the texture's position and activate it again.
            transform.position = _initialPosition;
            gameObject.SetActive(true);
            timeOnTexture = 0f;
            textureActive = true;
            playerIsOnTexture = false;

        }
    }
}
