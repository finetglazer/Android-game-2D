using JetBrains.Annotations;
using UnityEngine;

namespace MainCharacter
{
    public class AttackHandler : MonoBehaviour
    {
        public float damageDealt = 1;
        public float distanceDealDamage = 1;
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        private const string EnemyTag = "Enemy";
        private const string MerchantEnemyTag = "MerchantEnemy";
        private const string PeasantEnemyTag = "PeasantEnemy";
        private const string PriestEnemyTag = "PriestEnemy";
        private const string SoliderEnemyTag = "SoliderEnemy";
        private const string ThiefEnemyTag = "ThiefEnemy";
        [CanBeNull] private GameObject _enemy; 
        private BoxCollider2D _playerBoxCollider;
        private Animator _playerAnimator;
        private bool _isEnemyDead;
        
        private void Start()
        {
            _playerBoxCollider = GetComponent<BoxCollider2D>();
            _playerAnimator = GetComponent<Animator>();
        }
        private void Update()
        {
            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                Destroy(this);
                return;
            }

            if (EnemyIsInDamageDealtDistance()){}   // Just a reason to update _enemy
        }
        
        private bool EnemyIsInDamageDealtDistance()
        {
            var raycastHit = Physics2D.BoxCast(_playerBoxCollider.bounds.center, _playerBoxCollider.bounds.size, 0, new Vector2(-Mathf.Sign(transform.localScale.x), 0), distanceDealDamage, LayerMask.GetMask(EnemyTag));
            if (raycastHit.collider is not null) _enemy = raycastHit.collider.gameObject;
            return raycastHit.collider is not null;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void CauseDamage()
        {
            if (_enemy == null) return;
            if (_enemy.GetComponent<Animator>() is null) return;   // Enemy is dead
            if (!EnemyIsInDamageDealtDistance()) return;
            
            var enemyAnimator = _enemy.GetComponent<Animator>();
            float currentEnemyHealth = 0;
            if (_enemy.CompareTag(MerchantEnemyTag))
            {
                var t = _enemy.GetComponent<Others.Merchant.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
            }
            else if (_enemy.CompareTag(PeasantEnemyTag))
            {
                var t = _enemy.GetComponent<Others.Peasant.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
            }
            else if (_enemy.CompareTag(PriestEnemyTag))
            {
                var t = _enemy.GetComponent<Others.Priest.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
            }
            else if (_enemy.CompareTag(SoliderEnemyTag))
            {
                var t = _enemy.GetComponent<Others.Soldier.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
            }
            else if (_enemy.CompareTag(ThiefEnemyTag))
            {
                var t = _enemy.GetComponent<Others.Thief.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
            }
            
            if (EnemyIsInDamageDealtDistance())
            {
                currentEnemyHealth -= damageDealt;
                enemyAnimator.SetTrigger(Hurt);
            }

            if (currentEnemyHealth > 0) return;
            
            enemyAnimator.SetTrigger(Die);
            _isEnemyDead = true;
        }
        
        private void EnemyDisappears()
        {
            if (!_isEnemyDead) return;
            Destroy(_enemy);
        }
    }   
}
