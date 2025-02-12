﻿using System.Collections.Generic;
using System.Linq;
using OtherCharacters.Priest;
using UnityEngine;
using Random = System.Random;

namespace Respawner
{
    public class BossAndEnemiesRespawner : MonoBehaviour
    {
        [Header("Render enemies when Boss's HP percentage lost (< 1)")]
        public float renderEnemiesWhenHpPercentageLost = 0.33f;
        public float delayRenderEnemyTimeAfterCautionZoneEmerges = 0.5f;
        public static bool CanReproducible = true;
        public GameObject referencingRenderCautionZone;
        public GameObject[] priestBosses;
        public GameObject[] baerEnemies;
        private float _xLeft;
        private float _xRight;
        private float _yUp;
        private float _yDown;
        private float _initialHealth;
        private float _clock;
        private bool _canRenderEnemies = true;
        private GameObject[] _renderEnemiesCautionZones;
        private readonly Dictionary<GameObject, bool > _used = new();
        private float _secondPriestBossHealth;
        public static int cnt = 0;

        private void Start()
        {
            _initialHealth = priestBosses[0].GetComponent<Movement>().currentHealth;
            _xLeft = GameObject.Find("X-Left").transform.position.x;
            _xRight = GameObject.Find("X-Right").transform.position.x;
            _yUp = GameObject.Find("Y-Up").transform.position.y;
            _yDown = GameObject.Find("Y-Down").transform.position.y;
            _renderEnemiesCautionZones = new GameObject[baerEnemies.Length];
            for (var i = 0; i < baerEnemies.Length; ++i)
            {
                _renderEnemiesCautionZones[i] = Instantiate(referencingRenderCautionZone);
                _renderEnemiesCautionZones[i].name = "RenderCautionZone" + i;
                _renderEnemiesCautionZones[i].SetActive(false);
            }
        }

        private void Update()
        {
            var currentFirstBossHealth = priestBosses[0].GetComponent<Movement>().currentHealth;
            if (currentFirstBossHealth <= 0 && CanReproducible)
            {
                foreach (var priestBoss in priestBosses)
                {
                    if (priestBoss.activeInHierarchy)
                    {
                        priestBoss.SetActive(false);
                        priestBoss.GetComponent<Movement>().healthBar.SetActive(false);    // Trigger health bar resets
                    }
                    priestBoss.SetActive(true);
                    priestBoss.transform.position = GetRandomPosition();
                    priestBoss.GetComponent<Movement>().enabled = true;
                    priestBoss.GetComponent<Movement>().currentHealth = _initialHealth / 2;
                    priestBoss.GetComponent<Movement>().healthBar.SetActive(true);

                }
                CanReproducible = false;
                return;
            }
            
            
            

            if (currentFirstBossHealth > (1 - renderEnemiesWhenHpPercentageLost) * _initialHealth || !_canRenderEnemies) return;

            _clock += Time.deltaTime;
            if (_used.Count == 0)
            {
                foreach (var obj in _renderEnemiesCautionZones)
                {
                    obj.SetActive(true);
                    obj.transform.position = new Vector3(GetRandomPosition().x, obj.transform.position.y, 0);
                    _used.Add(obj, false);
                }
            }

            if (_clock < delayRenderEnemyTimeAfterCautionZoneEmerges) return;
            foreach (var enemy in baerEnemies)
            {
                foreach (var cautionZone in _used.Where(cautionZone => !cautionZone.Value))
                {
                    enemy.SetActive(true);
                    var merchantMovement = enemy.GetComponent<OtherCharacters.Merchant.Movement>();
                    var peasantMovement = enemy.GetComponent<OtherCharacters.Peasant.Movement>();
                    var soldierMovement = enemy.GetComponent<OtherCharacters.Soldier.Movement>();
                    var thiefMovement = enemy.GetComponent<OtherCharacters.Thief.Movement>();
                    merchantMovement?.healthBar.SetActive(true);
                    peasantMovement?.healthBar.SetActive(true);
                    soldierMovement?.healthBar.SetActive(true);
                    thiefMovement?.healthBar.SetActive(true);
                    _used[cautionZone.Key] = true;
                    enemy.transform.position = new Vector3(cautionZone.Key.transform.position.x, GetRandomPosition().y, 0);
                    break;
                }
            }
            
            foreach (var obj in _renderEnemiesCautionZones)
            {
                obj.SetActive(false);
            }

            _canRenderEnemies = false;
        }

        private Vector3 GetRandomPosition()
        {
            var random = new Random();
            return new Vector3((float)random.NextDouble() * (_xRight - _xLeft) + _xLeft, (float)random.NextDouble() * (_yUp - _yDown) + _yDown, 0);
        }
    }
}