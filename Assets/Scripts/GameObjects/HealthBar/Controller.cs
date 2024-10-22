using System;
using MainCharacter;
using UI.Buttons;
using UnityEngine;
using UnityEngine.UI;
using Movement = OtherCharacters.Priest.Movement;

namespace GameObjects.HealthBar
{
    public class Controller : MonoBehaviour
    {
        public GameObject character;
        [Header("Render enemies when HP percentage lost (< 1)")]
        public float renderEnemiesWhenHpPercentageLost = 1f;
        private Animator _characterAnimator;
        private AttackHandler _playerAttackHandler;
        private AttackButton _attackButton;
        private Image[] _healthBarImages;
        private const string PlayerTag = "Player";
        private float _damageReceived;
        private float _proportionHpLost;
        private float _initialHealth;
        private bool _isFighting;

        private void Start()
        {
            _characterAnimator = character.GetComponent<Animator>();
            _playerAttackHandler = GameObject.FindGameObjectWithTag(PlayerTag).GetComponent<AttackHandler>();
            _healthBarImages = GetComponentsInChildren<Image>();
            while (!_attackButton)
            {
                try
                {
                    _attackButton = FindObjectOfType<Button>().GetComponent<AttackButton>();
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
            _damageReceived = GameObject.FindGameObjectWithTag(PlayerTag).GetComponent<AttackHandler>().damageDealt;
            _initialHealth = character.GetComponent<Movement>().currentHealth;
            _proportionHpLost = _damageReceived / _initialHealth;
        }

        private void Update()
        {
            _isFighting = _playerAttackHandler.isFighting;
            if (!_characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("hurt") || !_isFighting) return;
            print("aew");
            var fillAmountLost = _proportionHpLost * _healthBarImages.Length;
            foreach (var healthBarImage in _healthBarImages)
            {
                if (fillAmountLost <= 0) break;
                if (healthBarImage.fillAmount == 0) continue;
                var t = 1 - healthBarImage.fillAmount;
                healthBarImage.fillAmount = Mathf.Max(healthBarImage.fillAmount - fillAmountLost, 0f);
                fillAmountLost -= Mathf.Min(fillAmountLost, t);
            }
        }
    }
}