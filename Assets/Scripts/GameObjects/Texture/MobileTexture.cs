using System;
using UnityEngine;

namespace GameObjects.Texture
{
    public class MobileTexture : MonoBehaviour
    {
        public float xVelocity = 1;     // Can < 0
        public float yVelocity;         // Can < 0
        public float moveTime = 0.5f;
        public float clock;             // Start on the left side
        
        
        private void Update()
        {
            clock += Time.deltaTime;
            if (clock < moveTime)
            {
                transform.Translate(new Vector2(xVelocity * Time.deltaTime, yVelocity * Time.deltaTime));
            }
            else if (clock >= moveTime && clock < 2 * moveTime)
            {
                transform.Translate(new Vector2(-xVelocity * Time.deltaTime, -yVelocity * Time.deltaTime));
            }
            else
            {
                clock = 0;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var player = other.gameObject;
                player.AddComponent<MobileTexture>();
                var playerController = player.GetComponent<MobileTexture>();
                playerController.xVelocity = xVelocity;
                playerController.yVelocity = yVelocity;
                playerController.moveTime = moveTime;
                playerController.clock = clock;
            }
        }
    }
}