using JetBrains.Annotations;
using UnityEngine;

namespace MainCharacter
{
    public class AttackHandler : MonoBehaviour
    {
        public float damageDealt = 1;
        public float distanceDealDamage = 1;
        public float delayBeforeCausingDamage = 0.2f;
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
        private void Start()
        {
            _playerBoxCollider = GetComponent<BoxCollider2D>();
            _playerAnimator = GetComponent<Animator>();
        }
        private void Update()
        {
            if (!EnemyIsInDamageDealtDistance()) return;
            if (Input.GetMouseButtonDown(0)) CauseDamage();
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
            var enemyAnimator = _enemy!.GetComponent<Animator>();
            float? currentEnemyHealth = _enemy!.tag switch
            {
                MerchantEnemyTag => _enemy.GetComponent<Others.Merchant.Movement>().currentHealth,
                PeasantEnemyTag => _enemy.GetComponent<Others.Peasant.Movement>().currentHealth,
                PriestEnemyTag => _enemy.GetComponent<Others.Priest.Movement>().currentHealth,
                // SoliderEnemyTag => _enemy.GetComponent<Others.Solider.Movement>().currentHealth,
                // ThiefEnemyTag => _enemy.GetComponent<Others.Thief.Movement>().currentHealth,
                _ => null
            };
            enemyAnimator.SetTrigger(Hurt);
            var currentPlayerAnimation = _playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentPlayerAnimation.IsName("attack") && currentPlayerAnimation.length >= delayBeforeCausingDamage) currentEnemyHealth -= damageDealt;
            if (currentEnemyHealth! <= 0)
            {
                enemyAnimator.SetTrigger(Die);
            }
        }
    }   
}
