using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Movement = OtherCharacters.Priest.Movement;

namespace GameObjects.HealthBar
{
    public class HealthBarController : MonoBehaviour
    {
        public GameObject character;
        private Movement _characterMovement;
        private Image[] _healthBarImages;
        private float _proportionRemainedHp;
        private float _initialHealth;
        private float _currentHealth;

        private void OnEnable()
        {
            _characterMovement = character.GetComponent<Movement>();
            _healthBarImages = GetComponentsInChildren<Image>().Where(image => image.name.Contains("FilledHealthBar")).ToArray();
            _initialHealth = _characterMovement.currentHealth;
        }

        private void Update()
        {
            _currentHealth = _characterMovement.currentHealth;
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