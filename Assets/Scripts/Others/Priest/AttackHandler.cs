using UnityEngine;

namespace Others.Priest
{
    public class AttackHandler : MonoBehaviour
    {
        public float damageDealt = 1;
        public float distanceDetectingPlayer = 1;
        public float attackCoolDownTime = 1;                // Must > 1 second
        public Transform lightningOccurPoint;
        public GameObject cautionZone;
        public GameObject lightning;
        public GameObject lightningExplosion;
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Idle = Animator.StringToHash("idle");
        private const string PlayerMask = "Player";
        private const string PlayerTag = "Player";
        private GameObject _player;
        private Animator _playerAnimator;
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        private Movement _characterMovement;
        private float _attackCoolDownTime;
        private bool _isPlayerDead;
        
        private void Start()
        {
            _attackCoolDownTime = attackCoolDownTime;
            _player = GameObject.FindGameObjectWithTag(PlayerTag);
            _playerAnimator = _player.GetComponent<Animator>();
            _characterAnimator = GetComponent<Animator>();
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterMovement = GetComponent<Movement>();
            _isPlayerDead = false;
        }

        private void Update()
        {
            if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                cautionZone.SetActive(false);
                lightning.SetActive(false);
                lightningExplosion.SetActive(false);
                Destroy(this);
                return;
            }
            
            _attackCoolDownTime += Time.deltaTime;
                
            if (!PlayerDetectedOnLeft() && !PlayerDetectedOnRight()) return;

            if (_playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                _isPlayerDead = true;
            }
            if (_attackCoolDownTime < attackCoolDownTime)
            {
                _characterAnimator.SetTrigger(Idle);
                return;
            }
            
            _attackCoolDownTime = 0;
            if (_isPlayerDead) return;
            
            if (PlayerDetectedOnLeft())
            {
                _characterMovement.TurnLeft();
            }
            else if (PlayerDetectedOnRight())
            {
                _characterMovement.TurnRight();
            }
            else return;
            
            lightningOccurPoint.position = _player.transform.position;
            _characterAnimator.SetTrigger(Attack);      // All events triggered in Attack animation
        }

        internal bool PlayerDetectedOnLeft()
        {
            var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.left, distanceDetectingPlayer, LayerMask.GetMask(PlayerMask));
            return raycastHit.collider is not null;
        }

        internal bool PlayerDetectedOnRight()
        {
            var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.right, distanceDetectingPlayer, LayerMask.GetMask(PlayerMask));
            return raycastHit.collider is not null;
        }

        private void ActivateCautionZone()
        {
            cautionZone.SetActive(true);
            cautionZone.transform.position = new Vector3(lightningOccurPoint.position.x, cautionZone.transform.position.y, cautionZone.transform.position.z);
        }

        private void DeActivateCautionZone()
        {
            cautionZone.SetActive(false);
            lightning.SetActive(true);
            lightning.transform.position = new Vector3(lightningOccurPoint.position.x, lightning.transform.position.y, lightning.transform.position.z);
        }

        private void ActivateLightningExplosion()
        {
            lightningExplosion.SetActive(true);
            lightningExplosion.transform.position = new Vector3(lightningOccurPoint.position.x, lightningExplosion.transform.position.y, lightningExplosion.transform.position.z);
        }

        private void DeActivateLightningExplosion()
        {
            lightningExplosion.SetActive(false);
        }

        private void DeActivateLightning()
        {
            lightning.SetActive(false);
        }
    }
}
