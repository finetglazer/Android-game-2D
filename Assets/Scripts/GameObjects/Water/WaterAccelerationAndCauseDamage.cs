using MainCharacter;
using UnityEngine;

namespace GameObjects.Water
{
    public class WaterAccelerationAndCauseDamage : MonoBehaviour
    {
        public static bool WaterIsOngoing = true;
        private static readonly int Die = Animator.StringToHash("die");
        public float speedIncrement = 1f;
        private Movement _waterMovement;
        private const string SpeedAcceleratorName = "WaterAccelerator";
        private void Start()
        {
            _waterMovement = GetComponent<Movement>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.name.Contains(SpeedAcceleratorName))
            {
                _waterMovement.currentSpeed += speedIncrement;
            }

            if (other.name.Contains("WaterEndingPoint"))
            {
                WaterIsOngoing = false;
                gameObject.SetActive(false);
                return;
            }
            
            var characterAnimator = other.GetComponent<Animator>();
            if (characterAnimator == null) return;  // is not player or enemies
            characterAnimator.SetTrigger(Die);
            ClearDeathEnemies.Clear();
        }
    }
}