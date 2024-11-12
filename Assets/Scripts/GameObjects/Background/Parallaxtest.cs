using System;
using UnityEngine;

namespace GameObjects.Background
{
    public class Parallaxtest : MonoBehaviour
    {
        public Transform cam;
        private float _startPos, _length;
        public float ParallaxEffect; 
            
        private void Start()
        {
            _startPos = transform.position.x;
            _length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void Update()
        {
            // dist + temp
            float dist = cam.transform.position.x * ParallaxEffect;
            float temp = cam.transform.position.x * (1 - ParallaxEffect);
            
            // 
            transform.position = new Vector3(_startPos + dist, transform.position.y, transform.position.z);
            // cap nhat vi tris khi qua khoir man
            if (temp > _startPos + _length)
                _startPos += _length;
            else if (temp < _startPos - _length)
                _startPos -= _length;
        }   
    }
}