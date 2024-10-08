using UnityEngine;

namespace GameObjects.Texture.MobileTexture
{
    public class Movement : MonoBehaviour
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
    }
}