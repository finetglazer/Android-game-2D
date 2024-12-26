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

        private Transform characterTransform;
        private UnityEngine.Camera mainCamera;
        private float initialHealth;

        private Characters.Knight.MovementSoloPlayer _knightMovementSoloPlayer;
        private Characters.Merchant.MovementSoloPlayer _merchantMovementSoloPlayer;
        private Characters.Peasant.MovementSoloPlayer _peasantMovementSoloPlayer;
        private Characters.Soldier.MovementSoloPlayer _soldierMovementSoloPlayer;
        private Characters.Thief.MovementSoloPlayer _thiefMovementSoloPlayer;

        public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset above the character

        public void SetCharacter(Transform character, UnityEngine.Camera camera)
        {
            characterTransform = character;
            mainCamera = camera;

            _knightMovementSoloPlayer = character.GetComponent<Characters.Knight.MovementSoloPlayer>();
            _merchantMovementSoloPlayer = character.GetComponent<Characters.Merchant.MovementSoloPlayer>();
            _peasantMovementSoloPlayer = character.GetComponent<Characters.Peasant.MovementSoloPlayer>();
            _soldierMovementSoloPlayer = character.GetComponent<Characters.Soldier.MovementSoloPlayer>();
            _thiefMovementSoloPlayer = character.GetComponent<Characters.Thief.MovementSoloPlayer>();

            if (_knightMovementSoloPlayer) initialHealth = _knightMovementSoloPlayer.currentHealth;
            else if (_merchantMovementSoloPlayer) initialHealth = _merchantMovementSoloPlayer.currentHealth;
            else if (_peasantMovementSoloPlayer) initialHealth = _peasantMovementSoloPlayer.currentHealth;
            else if (_soldierMovementSoloPlayer) initialHealth = _soldierMovementSoloPlayer.currentHealth;
            else if (_thiefMovementSoloPlayer) initialHealth = _thiefMovementSoloPlayer.currentHealth;

            // Set player name
            PhotonView pv = character.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != null)
            {
                playerNameText.text = pv.Owner.NickName;
                // set up our name color is green and other is blue
                playerNameText.color = pv.Owner.IsLocal ? Color.green : Color.red;
            }
        }

        private void Update()
        {
            if (characterTransform == null || healthBarFill == null || mainCamera == null || (
                    !_knightMovementSoloPlayer && !_merchantMovementSoloPlayer && !_peasantMovementSoloPlayer && !_soldierMovementSoloPlayer && !_thiefMovementSoloPlayer)) return;

            // Update position
            Vector3 worldPosition = characterTransform.position + offset;
            // Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            Vector3 screenPosition = worldPosition;
            transform.position = screenPosition;

            // Update health bar
            float currentHealth = -1;
            if (_knightMovementSoloPlayer) currentHealth = _knightMovementSoloPlayer.currentHealth;
            else if (_merchantMovementSoloPlayer) currentHealth = _merchantMovementSoloPlayer.currentHealth;
            else if (_peasantMovementSoloPlayer) currentHealth = _peasantMovementSoloPlayer.currentHealth;
            else if (_soldierMovementSoloPlayer) currentHealth = _soldierMovementSoloPlayer.currentHealth;
            else if (_thiefMovementSoloPlayer) currentHealth = _thiefMovementSoloPlayer.currentHealth;
            
            float proportionRemainedHp = currentHealth / initialHealth;
            proportionRemainedHp = Mathf.Clamp01(proportionRemainedHp);

            // Update the fill amount
            // healthBarFill.localScale = new Vector3(proportionRemainedHp, 1, 1);
            
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
