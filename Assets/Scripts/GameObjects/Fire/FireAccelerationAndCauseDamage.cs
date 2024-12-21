using MainCharacter;
using UnityEngine;

namespace GameObjects.Fire
{
    public class FireAccelerationAndCauseDamage : MonoBehaviour
    {
        public static bool FireIsOngoing = true;
        private static readonly int Die = Animator.StringToHash("die");
        public float speedIncrement = 1f;
        private Movement _fireMovement; 
        private const string SpeedAcceleratorName = "FireAccelerator";
        private void Start()
        {
            _fireMovement = GetComponent<Movement>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.name.Contains(SpeedAcceleratorName))
            {
                _fireMovement.currentSpeed += speedIncrement;
            }

            if (other.name.Contains("FireEndingPoint"))
            {
                // gameObject.SetActive(false);
                FireIsOngoing = false;
                // speed = 0;
                _fireMovement.currentSpeed = 0;
                // return;
            }
            
            if (other.GetComponent<Animator>() == null) return;  // neither player nor enemies
            var characterAnimator = other.GetComponent<Animator>();
            characterAnimator.SetTrigger(Die);
            ClearDeathEnemies.Clear();
        }
    }
}