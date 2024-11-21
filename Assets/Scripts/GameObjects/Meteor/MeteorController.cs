using UnityEngine;

namespace GameObjects.Meteor
{
    public class MeteorController : MonoBehaviour
    {
        public GameObject[] meteors;
        public float initialHeight = 1f;
        public float startPositionX;
        public float endPositionX = 1f;
        public float fallAngle = 30f; // In degrees
        public float fallSpeed = 1f;
        public float disappearDepth = -10f;
        public float cooldownTime = 1f;
        [HideInInspector] public Vector3 initialPosition = Vector3.zero;
        private float _cooldownClock;
        
        private void Update()
        {
            _cooldownClock += Time.deltaTime;
            if (_cooldownClock < cooldownTime) return;
            foreach (var meteor in meteors)
            {
                if (meteor.activeInHierarchy) continue;
                meteor.SetActive(true);
                var xPos = Random.Range(startPositionX, endPositionX);
                initialPosition = new Vector3(xPos, initialHeight, initialPosition.z);
                _cooldownClock = 0f;
                break;
            }
        }
    }
}