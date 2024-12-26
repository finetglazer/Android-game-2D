using Photon.Enemy;
using Photon.Pun;
using Photon.Solo.Characters.Knight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Solo.Commons.HealthBar
{
    public class HealthBarUI : MonoBehaviour
    {
        public RectTransform healthBarFill; // Assign in Inspector (the fill part)
        public TMP_Text playerNameText; // Assign in Inspector

        private Transform _characterTransform;
        private Transform _enemyTransform;
        private Camera _mainCamera;
        private float _initialHealth;

        private Characters.Knight.MovementSoloPlayer _knightMovementSoloPlayer;
        private Characters.Merchant.MovementSoloPlayer _merchantMovementSoloPlayer;
        private Characters.Peasant.MovementSoloPlayer _peasantMovementSoloPlayer;
        private Characters.Soldier.MovementSoloPlayer _soldierMovementSoloPlayer;
        private Characters.Thief.MovementSoloPlayer _thiefMovementSoloPlayer;

        private Enemy.MultiEnemyMovement _enemyMovement;

        public Vector3 offset = new (0, 1.5f, 0); // Offset above the character

        private bool _isEnemy;
        public void SetCharacter(Transform character, Camera mainCamera)
        {
            _characterTransform = character;
            _mainCamera = mainCamera;

            _knightMovementSoloPlayer = character.GetComponent<Characters.Knight.MovementSoloPlayer>();
            _merchantMovementSoloPlayer = character.GetComponent<Characters.Merchant.MovementSoloPlayer>();
            _peasantMovementSoloPlayer = character.GetComponent<Characters.Peasant.MovementSoloPlayer>();
            _soldierMovementSoloPlayer = character.GetComponent<Characters.Soldier.MovementSoloPlayer>();
            _thiefMovementSoloPlayer = character.GetComponent<Characters.Thief.MovementSoloPlayer>();

            if (_knightMovementSoloPlayer) _initialHealth = _knightMovementSoloPlayer.currentHealth;
            else if (_merchantMovementSoloPlayer) _initialHealth = _merchantMovementSoloPlayer.currentHealth;
            else if (_peasantMovementSoloPlayer) _initialHealth = _peasantMovementSoloPlayer.currentHealth;
            else if (_soldierMovementSoloPlayer) _initialHealth = _soldierMovementSoloPlayer.currentHealth;
            else if (_thiefMovementSoloPlayer) _initialHealth = _thiefMovementSoloPlayer.currentHealth;

            // Set player name
            var pv = character.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != null)
            {
                Debug.Log("lalalalala");
                playerNameText.text = pv.Owner.NickName;
                // set up our name color is green and other is blue
                playerNameText.color = pv.Owner.IsLocal ? Color.green : Color.red;
            }
        }
        
        public void SetCharacterEnemy(Transform character, Camera mainCamera, bool isEnemy = false)
        {
            _enemyTransform = character;
            _mainCamera = mainCamera;
            _isEnemy = isEnemy;

            _enemyMovement = character.GetComponent<MultiEnemyMovement>();
            if (_enemyMovement) _initialHealth = _enemyMovement.currentHealth;
            
            var pv = character.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != null)
            {
                Debug.Log("setting up for enemy");
                playerNameText.text = pv.Owner.NickName;
                playerNameText.color = Color.red;
            }
        }

        private void Update()
        {
            if (!_isEnemy &&
                (_characterTransform == null 
                    || healthBarFill == null 
                    || _mainCamera == null 
                    || (!_knightMovementSoloPlayer 
                        && !_merchantMovementSoloPlayer 
                        && !_peasantMovementSoloPlayer 
                        && !_soldierMovementSoloPlayer 
                        && !_thiefMovementSoloPlayer)
                    )
                || (_isEnemy
                    && (_enemyTransform == null
                        || healthBarFill == null
                        || _mainCamera == null
                        || !_enemyMovement))
                ) return;

            float currentHealth = -10000;
            
            // If not Enemy
            
            if (!_isEnemy)
            {
                // Update player health bar position
                var worldPosition = _characterTransform.position + offset;
                // Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
                var screenPosition = worldPosition;
                transform.position = screenPosition;
                
                if (_knightMovementSoloPlayer) currentHealth = _knightMovementSoloPlayer.currentHealth;
                else if (_merchantMovementSoloPlayer) currentHealth = _merchantMovementSoloPlayer.currentHealth;
                else if (_peasantMovementSoloPlayer) currentHealth = _peasantMovementSoloPlayer.currentHealth;
                else if (_soldierMovementSoloPlayer) currentHealth = _soldierMovementSoloPlayer.currentHealth;
                else if (_thiefMovementSoloPlayer) currentHealth = _thiefMovementSoloPlayer.currentHealth;
            }

            // If is Enemy
            else
            {
                // Update enemy health bar position
                var worldPosition = _enemyTransform.position + offset;
                // Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
                var screenPosition = worldPosition;
                transform.position = screenPosition;
                
                currentHealth = _enemyMovement.currentHealth;
            }
                
            var proportionRemainedHp = currentHealth / _initialHealth;
            proportionRemainedHp = Mathf.Clamp01(proportionRemainedHp);

            
            // Update the fill amount
            healthBarFill.GetComponent<Image>().fillAmount = proportionRemainedHp;
            
            // Optional: Change color based on health
            if (proportionRemainedHp > 0.5f)
            {
                healthBarFill.GetComponent<Image>().color = Color.green;
            }
            else if (proportionRemainedHp > 0.2f)
            {
                healthBarFill.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                healthBarFill.GetComponent<Image>().color = Color.red;
            }
        }
    }
}
