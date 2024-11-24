using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjects.HealthBar
{
    public class HealthBarController : MonoBehaviour
    {
        public GameObject character;
        private Image[] _healthBarImages;
        private float _proportionRemainedHp;
        private float _initialHealth;

        private void OnEnable()
        {
            _healthBarImages = GetComponentsInChildren<Image>().Where(image => image.name.Contains("FilledHealthBar")).ToArray();
            _initialHealth = GetCharacterCurrentHealth();
        }

        private void Update()
        {
            var currentHealth = GetCharacterCurrentHealth();
            _proportionRemainedHp = currentHealth / _initialHealth;
            var fillAmountNeeded = _proportionRemainedHp * _healthBarImages.Length;
            for (var i = _healthBarImages.Length - 1; i >= 0; i--)
            {
                var maximumFillAmount = Mathf.Min(1f, fillAmountNeeded);
                _healthBarImages[i].fillAmount = maximumFillAmount;
                fillAmountNeeded -= maximumFillAmount;
            }
        }

        private float GetCharacterCurrentHealth()
        {
            var mainCharacterMovement = character.GetComponent<MainCharacter.Movement>();
            var priestMovement = character.GetComponent<OtherCharacters.Priest.Movement>();
            var merchantMovement = character.GetComponent<OtherCharacters.Merchant.Movement>();
            var peasantMovement = character.GetComponent<OtherCharacters.Peasant.Movement>();
            var soldierMovement = character.GetComponent<OtherCharacters.Soldier.Movement>();
            var thiefMovement = character.GetComponent<OtherCharacters.Thief.Movement>(); 
            if (mainCharacterMovement is not null)
            {
                return mainCharacterMovement.currentHealth;
            }
            if (priestMovement is not null)
            {
                return priestMovement.currentHealth;
            }
            if (merchantMovement is not null)
            {
                return merchantMovement.currentHealth;
            }
            if (peasantMovement is not null)
            {
                return peasantMovement.currentHealth;
            }
            if (soldierMovement is not null)
            {
                return soldierMovement.currentHealth;
            }
            return thiefMovement.currentHealth;
        }
    }
}