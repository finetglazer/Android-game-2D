using UnityEngine;

namespace GameObjects.Background
{
    public class BackgroundMovement:MonoBehaviour
    {
        private Material mat;
        private float distance;
        [Range(0f,100f)]
        public float speed;

        private void Start()
        {
            mat = GetComponent<Renderer>().material;
        }
        
        void Update()
        {
            distance = Time.deltaTime * speed;
            mat.SetTextureOffset("_MainTex", Vector2.right * distance); 
        }
    }
}