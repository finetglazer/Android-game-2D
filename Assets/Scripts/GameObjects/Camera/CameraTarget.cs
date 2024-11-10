using UnityEngine;

namespace GameObjects.Camera
{
    public class CameraTarget : MonoBehaviour
    {
        public GameObject player; // object in the scene
        public float yOffset; // offset between camera and gameObject
        public float speed; // speed of the camera
 
        // Update is called once per frame
        void Update()
        {
            if (player is null) return;
            Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y + yOffset, -10f);
            transform.position = Vector3.Slerp(transform.position, target, speed * Time.deltaTime);
        }
    }
}