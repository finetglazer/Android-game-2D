using UnityEngine;

namespace OtherCharacters.Peasant
{ 
    public class ArrowMovement : MonoBehaviour
    {
        [HideInInspector] public AttackHandler characterController;
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        private GameObject _arrow;
        private float _damageDealt;
        private float _speed ;
        private float _arrowDirection;
        private void Start()
        {
            _damageDealt = characterController.damageDealt;
            _speed = characterController.arrowSpeed;
            _arrowDirection = characterController.ArrowDirection;
        }

        private void Update()
        {
            transform.Translate(new Vector2(0, _speed * Time.deltaTime * _arrowDirection)); 
            // Since arrow has been rotated 90 degrees to be horizontal, the translation should be handled carefully! 
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var playerAnimator = other.GetComponent<Animator>();
                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die")) return;
                playerAnimator.SetTrigger(Hurt);

                var playerCurrentHealth = other.GetComponent<MainCharacter.Movement>().currentHealth;
                playerCurrentHealth -= _damageDealt;
                other.GetComponent<MainCharacter.Movement>().currentHealth = playerCurrentHealth;
                if (playerCurrentHealth <= 0)
                {
                    playerAnimator.SetTrigger(Die);
                }
            }
            
            DeActivateArrow();
        }

        private void DeActivateArrow()
        {
            gameObject.SetActive(false);
        }
    }
}