using UnityEngine;

namespace OtherCharacters.Priest
{
    public class AttackHandler : MonoBehaviour
    {
        public float damageDealt = 1;
        public float distanceDetectingPlayer = 1;
        public float attackCoolDownTime = 1;                // Must > 1 second
        public float increaseWalkSpeedWhenChasingBy = 0.2f;
        public float distanceTriggeringAttack = 1f;
        public GameObject cautionZone;
        public GameObject lightning;
        public GameObject lightningExplosion;
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int Walk = Animator.StringToHash("walk");
        private const string PlayerMask = "Player";
        private const string PlayerTag = "Player";
        private Vector3 _lightningOccurPoint;
        private GameObject _player;
        private BoxCollider2D _characterBoxCollider;
        private Animator _characterAnimator;
        private Movement _characterMovement;
        private float _attackCoolDownTime;
        private float _firstWalkSpeed;
        
        private void Start()
        {
            _attackCoolDownTime = attackCoolDownTime;
            _player = GameObject.FindGameObjectWithTag(PlayerTag);
            _characterAnimator = GetComponent<Animator>();
            _characterBoxCollider = GetComponent<BoxCollider2D>();
            _characterMovement = GetComponent<Movement>();
            _firstWalkSpeed = GetComponent<Movement>().walkSpeed;
        }

        private void Update()
        {
            if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("die"))
            {
                cautionZone.SetActive(false);
                lightning.SetActive(false);
                lightningExplosion.SetActive(false);
                enabled = false;
                return;
            }

            if (_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("hurt"))
            {
                cautionZone.SetActive(false);
                lightning.SetActive(false);
                lightningExplosion.SetActive(false);
            }
            
            _attackCoolDownTime += Time.deltaTime;
                
            if (!PlayerDetectedOnLeft() && !PlayerDetectedOnRight()) return;
            
            ChasePlayer();
            
            if (!PlayerIsInDamageDealtDistance()) return;
            
            if (_attackCoolDownTime < attackCoolDownTime)
            {
                _characterAnimator.SetTrigger(Idle);
                return;
            }
            
            _attackCoolDownTime = 0;
            
            _lightningOccurPoint = _player.transform.position;
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
        
        private bool PlayerIsInDamageDealtDistance()
        {
            var raycastHitLeft = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.left, distanceTriggeringAttack, LayerMask.GetMask(PlayerMask));
            var raycastHitRight = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0f, Vector2.right, distanceTriggeringAttack, LayerMask.GetMask(PlayerMask));
            return raycastHitLeft.collider is not null || raycastHitRight.collider is not null;            
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void ChasePlayer()
        {
            if (_player is null || PlayerIsInDamageDealtDistance())
            {
                return;
            }
            
            if (PlayerDetectedOnLeft()) _characterMovement.TurnLeft();
            else _characterMovement.TurnRight();

            if (_characterMovement.IsWallOnLeft() && PlayerDetectedOnLeft()) return;
            if (_characterMovement.IsWallOnRight() && PlayerDetectedOnRight()) return;
            
            _characterAnimator.SetTrigger(Walk);

            var newWalkSpeed = _firstWalkSpeed * (1 + increaseWalkSpeedWhenChasingBy);
            GetComponent<Movement>().walkSpeed = newWalkSpeed;
            transform.position += Mathf.Approximately(Mathf.Sign(transform.localScale.x), 1) switch
            {
                true => Vector3.left * (newWalkSpeed * Time.deltaTime),
                _ => Vector3.right * (newWalkSpeed * Time.deltaTime)
            };
        }

        private void ActivateCautionZone()
        {
            cautionZone.SetActive(true);
            cautionZone.transform.position = new Vector3(_lightningOccurPoint.x, cautionZone.transform.position.y, cautionZone.transform.position.z);
        }

        private void DeActivateCautionZone()
        {
            cautionZone.SetActive(false);
            lightning.SetActive(true);
            lightning.transform.position = new Vector3(cautionZone.transform.position.x, lightning.transform.position.y, lightning.transform.position.z);
        }

        private void ActivateLightningExplosion()
        {
            lightningExplosion.SetActive(true);
            lightningExplosion.transform.position = new Vector3(cautionZone.transform.position.x, lightningExplosion.transform.position.y, lightningExplosion.transform.position.z);
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
