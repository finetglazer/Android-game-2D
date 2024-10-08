using UnityEngine;

namespace OtherCharacters.Peasant
{ 
    public class ArrowMovement : MonoBehaviour
    {
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        private GameObject _arrow;
        private const string EnemyPeasantTag = "PeasantEnemy";
        private const string EnemyPeasantArrowTag = "EnemyPeasantArrow";
        private GameObject _character;
        private AttackHandler _attackHandler;
        private float _damageDealt;
        private float _speed ;
        private float _arrowDirection;
        private void Start()
        {
            _character = GameObject.FindGameObjectWithTag(EnemyPeasantTag);
            _arrow = GameObject.FindGameObjectWithTag(EnemyPeasantArrowTag);
            _attackHandler = _character.GetComponent<AttackHandler>();
            _damageDealt = _attackHandler.damageDealt;
            _speed = _attackHandler.arrowSpeed;
            _arrowDirection = _attackHandler.ArrowDirection;
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
                playerAnimator.SetTrigger(Hurt);

                var playerCurrentHealth = other.GetComponent<MainCharacter.Movement>().currentHealth;
                playerCurrentHealth -= _damageDealt;
                other.GetComponent<MainCharacter.Movement>().currentHealth = playerCurrentHealth;
                if (playerCurrentHealth <= 0)
                {
                    playerAnimator.SetTrigger(Die);
                    Destroy(other, 2);
                }
            }
            
            DeActivateArrow();
        }

        private void DeActivateArrow()
        {
            _arrow.SetActive(false);
        }
    }
}