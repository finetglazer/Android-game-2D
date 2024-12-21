using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Photon.Character
{
    public class HealthBarUI : MonoBehaviour
    {
        public RectTransform healthBarFill; // Assign in Inspector (the fill part)
        public TMP_Text playerNameText; // Assign in Inspector

        private Transform characterTransform;
        private UnityEngine.Camera mainCamera; // Reference to the main camera
        private float initialHealth;

        private MovementMultiplayer baseMovement;

        public Vector3 offset = new Vector3(4.8f, 0, 0); // Offset above the character

        public void SetCharacter(Transform character, Camera camera)
        {
            
            characterTransform = character;
            mainCamera = camera;
        
            baseMovement = character.GetComponent<MovementMultiplayer>();
            if (baseMovement != null)
            {
                
                initialHealth = baseMovement.currentHealth;
            } 
            // Set player name
            PhotonView pv = character.GetComponent<PhotonView>();
            if (pv != null && pv.Owner != null)
            {
                playerNameText.text = pv.Owner.NickName;
            }
          
        }

        private void Update()
        {
            if (characterTransform == null || healthBarFill == null || mainCamera == null) return;
        
            // Update position
            Vector3 worldPosition = characterTransform.position + offset;
            // Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            Vector3 screenPosition = worldPosition;
            transform.position = screenPosition;
        
            // Update health bar
            if (baseMovement != null)
            {
                float currentHealth = baseMovement.currentHealth;
                float proportionRemainedHp = currentHealth / initialHealth;
                healthBarFill.localScale = new Vector3(proportionRemainedHp, 1, 1);
            }
        }
    }
}