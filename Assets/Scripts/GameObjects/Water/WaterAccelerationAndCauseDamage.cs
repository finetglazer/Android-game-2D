using MainCharacter;
using UnityEngine;

namespace GameObjects.Water
{
    public class WaterAccelerationAndCauseDamage : MonoBehaviour
    {
        private static readonly int Die = Animator.StringToHash("die");
        public float speedIncrement = 1f;
        private Movement _waterMovement;
        private const string SpeedAcceleratorName = "Accelerator";
        private void Start()
        {
            _waterMovement = GetComponent<Movement>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.name == SpeedAcceleratorName)
            {
                _waterMovement.currentSpeed += speedIncrement;
            }
            
            var characterAnimator = other.GetComponent<Animator>();
            if (characterAnimator == null) return;  // is not player or enemies
            characterAnimator.SetTrigger(Die);
            ClearDeathEnemies.Clear();
        }
    }
}