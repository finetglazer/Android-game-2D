using MainCharacter;
using UnityEngine;

namespace GameObjects.Fire
{
    public class FireAccelerationAndCauseDamage : MonoBehaviour
    {
        private static readonly int Die = Animator.StringToHash("die");
        public float speedIncrement = 1f;
        private Movement _fireMovement;
        private const string SpeedAcceleratorName = "Accelerator";
        private void Start()
        {
            _fireMovement = GetComponent<Movement>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.name == SpeedAcceleratorName)
            {
                _fireMovement.currentSpeed += speedIncrement;
            }
            
            var characterAnimator = other.GetComponent<Animator>();
            if (characterAnimator == null) return;  // is not player or enemies
            characterAnimator.SetTrigger(Die);
            ClearDeathEnemies.Clear();
        }
    }
}