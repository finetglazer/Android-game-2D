using UnityEngine;

namespace GameObjects.Background
{
    public class Parallax : MonoBehaviour
    {
        private float _length, _startPos;
        public GameObject cam;
        public float parallaxEffect;
        private void Start()
        {
            _startPos = transform.position.x;
            _length = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        
        private void Update()
        {
            float temp = (cam.transform.position.x * (1 - parallaxEffect));
            float dist = (cam.transform.position.x * parallaxEffect);
        
            transform.position = new Vector3(_startPos + dist, transform.position.y, transform.position.z);
        
            if (temp > _startPos + _length) _startPos += _length;
            else if (temp < _startPos - _length) _startPos -= _length;
        }

    }
}