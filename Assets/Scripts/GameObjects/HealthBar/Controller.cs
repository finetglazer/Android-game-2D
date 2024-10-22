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
        private Movement _characterMovement;
        private GameObject _healthBarSystem;
        private Image[] _healthBarImages;
        private const string PriestEnemyTag = "PriestEnemy";
        private float _proportionRemainedHp;
        private float _initialHealth;
        private float _currentHealth;

        private void Start()
        {
            _healthBarSystem = GameObject.Find("HealthBarSystem");
            _characterMovement = GameObject.FindGameObjectWithTag(PriestEnemyTag).GetComponent<Movement>();
            _healthBarImages = GetComponentsInChildren<Image>();
            _initialHealth = character.GetComponent<Movement>().currentHealth;
        }

        private void Update()
        {
            _currentHealth = _characterMovement.currentHealth;
            if (_currentHealth <= 0)
            {
                _healthBarSystem.SetActive(false);
                return;
            }
            _proportionRemainedHp = _currentHealth / _initialHealth;
            var fillAmountNeeded = _proportionRemainedHp * _healthBarImages.Length;
            for (var i = _healthBarImages.Length - 1; i >= 0; i--)
            {
                var maximumFillAmount = Mathf.Min(1f, fillAmountNeeded);
                _healthBarImages[i].fillAmount = maximumFillAmount;
                fillAmountNeeded -= maximumFillAmount;
            }
        }
    }
}