using UnityEngine;

namespace OtherCharacters.Priest
{
    public class LightningCollision : MonoBehaviour
    {
        private AttackHandler _attackHandler;
        private Animator _playerAnimator;
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        private const string PlayerTag = "Player";
        private const string PriestEnemyTag = "PriestEnemy";
        private float _damageDealt;
        private void Start()
        {
            _playerAnimator = GameObject.FindGameObjectWithTag(PlayerTag).GetComponent<Animator>();
            _attackHandler = GameObject.FindGameObjectWithTag(PriestEnemyTag).GetComponent<AttackHandler>();
            _damageDealt = _attackHandler.damageDealt;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other is null) return;
            
            if (!other.CompareTag(PlayerTag)) return;
            _playerAnimator.SetTrigger(Hurt);

            var currentPlayerHealth = other.GetComponent<MainCharacter.Movement>().currentHealth;
            currentPlayerHealth -= _damageDealt;
            other.GetComponent<MainCharacter.Movement>().currentHealth = currentPlayerHealth;
            
            if (!(currentPlayerHealth <= 0)) return;
            
            _playerAnimator.SetTrigger(Die);
            Destroy(other, 3);
        }
    }
}