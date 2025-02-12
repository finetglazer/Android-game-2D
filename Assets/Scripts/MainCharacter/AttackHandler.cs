using JetBrains.Annotations;
using Respawner;
using UnityEngine;
using Photon.Pun;  // Import Photon.Pun for networking

namespace MainCharacter
{
    public class AttackHandler : MonoBehaviourPunCallbacks
    {
        public float damageDealt = 1;
        public float distanceDealDamage = 1;
        private static readonly int Hurt = Animator.StringToHash("hurt");
        private static readonly int Die = Animator.StringToHash("die");
        private const string EnemyTag = "Enemy";
        private const string MerchantEnemyTag = "MerchantEnemy";
        private const string PeasantEnemyTag = "PeasantEnemy";
        private const string PriestEnemyTag = "PriestEnemy";
        private const string SoldierEnemyTag = "SoldierEnemy";
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
            // Only process input for the local player's character
            // if (!photonView.IsMine) return;

            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                gameObject.GetComponent<PlayerDie>().Die();
                return;
            }

            if (EnemyIsInDamageDealtDistance())  // Just a reason to update _enemy
            {
                // Optional: Could call attack or damage-related actions here
            }
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
            float currentEnemyHealth = 0, immortalRerenderTime = 0;
            if (_enemy.CompareTag(MerchantEnemyTag))
            {
                var t = _enemy.GetComponent<OtherCharacters.Merchant.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
                immortalRerenderTime = t.immortalRerenderTime;
            }
            else if (_enemy.CompareTag(PeasantEnemyTag))
            {
                var t = _enemy.GetComponent<OtherCharacters.Peasant.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
                immortalRerenderTime = t.immortalRerenderTime;
            }
            else if (_enemy.CompareTag(PriestEnemyTag))
            {
                var t = _enemy.GetComponent<OtherCharacters.Priest.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
                immortalRerenderTime = t.immortalRerenderTime;
            }
            else if (_enemy.CompareTag(SoldierEnemyTag))
            {
                var t = _enemy.GetComponent<OtherCharacters.Soldier.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
                immortalRerenderTime = t.immortalRerenderTime;
            }
            else if (_enemy.CompareTag(ThiefEnemyTag))
            {
                var t = _enemy.GetComponent<OtherCharacters.Thief.Movement>();
                if (t is null) return;  // Enemy is dead
                currentEnemyHealth = t.currentHealth;
                immortalRerenderTime = t.immortalRerenderTime;
            }

            if (EnemyIsInDamageDealtDistance())
            {
                currentEnemyHealth -= damageDealt;
                enemyAnimator.SetTrigger(Hurt);
                if (_enemy.CompareTag(PriestEnemyTag))
                {
                    _enemy.GetComponent<OtherCharacters.Priest.Movement>().currentHealth = currentEnemyHealth;
                }
                if (_enemy.CompareTag(MerchantEnemyTag))
                {
                    _enemy.GetComponent<OtherCharacters.Merchant.Movement>().currentHealth = currentEnemyHealth;
                }
                else if (_enemy.CompareTag(PeasantEnemyTag))
                {
                    _enemy.GetComponent<OtherCharacters.Peasant.Movement>().currentHealth = currentEnemyHealth;
                }
                else if (_enemy.CompareTag(PriestEnemyTag))
                {
                    _enemy.GetComponent<OtherCharacters.Priest.Movement>().currentHealth = currentEnemyHealth;
                }
                else if (_enemy.CompareTag(SoldierEnemyTag))
                {
                    _enemy.GetComponent<OtherCharacters.Soldier.Movement>().currentHealth = currentEnemyHealth;
                }
                else if (_enemy.CompareTag(ThiefEnemyTag))
                {
                    _enemy.GetComponent<OtherCharacters.Thief.Movement>().currentHealth = currentEnemyHealth;
                }
            }

            if (currentEnemyHealth > 0) return;
            if (_enemy.name.Contains("Immortal"))
            {
                DeathNote.AddImmortalEnemy(_enemy, immortalRerenderTime, _enemy.transform.position);
                _enemy.SetActive(false);
                return;
            }

            _isEnemyDead = !_enemy.name.ToLower().Contains("priest") || !BossAndEnemiesRespawner.CanReproducible;
            if (_isEnemyDead)
            {
                enemyAnimator.SetTrigger(Die);
            }
        }

        private void EnemyDisappears()
        {
            if (!_isEnemyDead) return;
            ClearDeathEnemies.Clear();
        }
    }
}
