
using Photon.Solo.Commons.PlayerCore;
using UnityEngine;

namespace Photon.FireMultiPlayer
{
    public class FireAccelerationAndCauseDamageMultiplayer : MonoBehaviour
    {
        public static bool FireIsOngoing = true;
        private static readonly int Die = Animator.StringToHash("die");
        public float speedIncrement = 1f;
        private FireMovementMultiplayer _fireFireMovementMultiplayer;
        private const string SpeedAcceleratorName = "FireAccelerator";
        private void Start()
        {
            _fireFireMovementMultiplayer = GetComponent<FireMovementMultiplayer>();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.name.Contains(SpeedAcceleratorName))
            {
                _fireFireMovementMultiplayer.currentSpeed += speedIncrement;
            }

            if (other.name.Contains("FireEndingPoint"))
            {
                // gameObject.SetActive(false);
                FireIsOngoing = false;
                // speed = 0;
                _fireFireMovementMultiplayer.currentSpeed = 0;
                // return;
            }
            
            if (other.GetComponent<Animator>() == null) return;  // neither player nor enemies
            var characterAnimator = other.GetComponent<Animator>();
            characterAnimator.SetTrigger(Die);
            other.GetComponent<PlayerDieNetworked>().Die();
        }
    }
}