using Photon.Pun;
using UnityEngine;

namespace Photon.Solo.Characters.Peasant
{
    public class ArrowSoloMovement : MonoBehaviourPun
    {
        private float _speed;
        private float _arrowDirection;
        private Rigidbody2D _rb;
        private float _damageDealt;

        public void SetDirection(float direction, float speed)
        {
            _arrowDirection = direction;
            _speed = speed;

            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody2D>();
                if (_rb == null)
                {
                    _rb = gameObject.AddComponent<Rigidbody2D>();
                }
            }

            _rb.gravityScale = 0;
            _rb.velocity = new Vector2(-_speed * _arrowDirection, 0); // Moves right if direction is 1, left if -1

            // Debug log for velocity
            Debug.Log($"Arrow velocity set to: {_rb.velocity}");
        }

        private void Start()
        {
            // Initialize _damageDealt if needed
            _damageDealt = 1f; // Or assign from another source
        }

        private void Update()
        {
            // Movement is handled by Rigidbody2D
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var playerAnimator = other.GetComponent<Animator>();
                if (playerAnimator != null && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("die")) return;

                var enemyPhotonView = other.GetComponent<PhotonView>();
                if (enemyPhotonView != null && !enemyPhotonView.IsMine)
                {
                    Debug.Log("Player " + other.name + " hit by arrow");
                    enemyPhotonView.RPC("ApplyDamage", RpcTarget.All, _damageDealt);
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
