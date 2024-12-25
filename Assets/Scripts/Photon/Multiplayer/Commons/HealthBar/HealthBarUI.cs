﻿using Photon.Pun;
using Photon.Solo.Characters.Knight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Multiplayer.Commons.HealthBar
{
    public class HealthBarUI : MonoBehaviour
    {
        public RectTransform healthBarFill; // Assign in Inspector (the fill part)
        public TMP_Text playerNameText; // Assign in Inspector

        private Transform characterTransform;
        private UnityEngine.Camera mainCamera;
        private float initialHealth;

        private MovementSoloPlayer _movementSoloPlayer;

        public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset above the character

        public void SetCharacter(Transform character, UnityEngine.Camera camera)
        {
            characterTransform = character;
            mainCamera = camera;

            _movementSoloPlayer = character.GetComponent<MovementSoloPlayer>();
            if (_movementSoloPlayer != null)
            {
                initialHealth = _movementSoloPlayer.currentHealth;
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
            if (characterTransform == null || healthBarFill == null || mainCamera == null || !_movementSoloPlayer) return;

            // Update position
            Vector3 worldPosition = characterTransform.position + offset;
            // Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            Vector3 screenPosition = worldPosition;
            transform.position = screenPosition;

            // Update health bar
            float currentHealth = _movementSoloPlayer.currentHealth;
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
