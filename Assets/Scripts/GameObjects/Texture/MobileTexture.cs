using UnityEngine;

namespace GameObjects.Texture
{
    public class MobileTexture : MonoBehaviour
    {
        public float xVelocity = 1;     // Can < 0
        public float yVelocity;         // Can < 0
        public float moveTime = 0.5f;
        public float clock;             // Start on the left side
        private const int GroundLayer = 3;
        
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

        // private void OnCollisionEnter2D(Collision2D other)
        // {
        //     if (other.gameObject.layer.Equals(GroundLayer)) return;
        //     
        //     var obj = other.gameObject;
        //     obj.AddComponent<MobileTexture>();
        //     var objController = obj.GetComponent<MobileTexture>();
        //     objController.xVelocity = xVelocity;
        //     objController.yVelocity = yVelocity;
        //     objController.moveTime = moveTime;
        //     objController.clock = clock;
        // }
        //
        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     print("aa");
        //     if (other.gameObject.layer.Equals(GroundLayer)) return;
        //     
        //     var obj = other.gameObject;
        //     obj.AddComponent<MobileTexture>();
        //     var objController = obj.GetComponent<MobileTexture>();
        //     objController.xVelocity = xVelocity;
        //     objController.yVelocity = yVelocity;
        //     objController.moveTime = moveTime;
        //     objController.clock = clock;
        // }
    }
}