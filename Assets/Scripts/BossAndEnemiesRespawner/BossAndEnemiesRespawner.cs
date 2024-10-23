using System.Collections.Generic;
using System.Linq;
using OtherCharacters.Priest;
using UnityEngine;
using Random = System.Random;

namespace BossAndEnemiesRespawner
{
    public class BossAndEnemiesRespawner : MonoBehaviour
    {
        [Header("Render enemies when Boss's HP percentage lost (< 1)")]
        public float renderEnemiesWhenHpPercentageLost = 0.33f;
        public float delayRenderEnemyTimeAfterCautionZoneEmerges = 0.5f;
        public static bool CanReproducible = true;
        public GameObject referencingRenderCautionZone;
        public GameObject[] priestBosses;
        public GameObject[] enemies;
        private float _xLeft;
        private float _xRight;
        private float _yUp;
        private float _yDown;
        private float _initialHealth;
        private float _clock;
        private bool _canRenderEnemies = true;
        private GameObject[] _renderCautionZones;
        private readonly Dictionary<GameObject, bool > _used = new();

        private void Start()
        {
            _initialHealth = priestBosses[0].GetComponent<Movement>().currentHealth;
            _xLeft = GameObject.Find("X-Left").transform.position.x;
            _xRight = GameObject.Find("X-Right").transform.position.x;
            _yUp = GameObject.Find("Y-Up").transform.position.y;
            _yDown = GameObject.Find("Y-Down").transform.position.y;
            _renderCautionZones = new GameObject[priestBosses.Length + enemies.Length];
            for (var i = 0; i < priestBosses.Length + enemies.Length; ++i)
            {
                _renderCautionZones[i] = Instantiate(referencingRenderCautionZone);
                _renderCautionZones[i].name = "RenderCautionZone" + i;
                _renderCautionZones[i].SetActive(false);
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
                foreach (var obj in _renderCautionZones)
                {
                    obj.SetActive(true);
                    obj.transform.position = new Vector3(GetRandomPosition().x, obj.transform.position.y, 0);
                    _used.Add(obj, false);
                }
            }

            if (_clock < delayRenderEnemyTimeAfterCautionZoneEmerges) return;
            foreach (var enemy in enemies)
            {
                foreach (var cautionZone in _used.Where(cautionZone => !cautionZone.Value))
                {
                    enemy.SetActive(true);
                    _used[cautionZone.Key] = true;
                    enemy.transform.position = new Vector3(cautionZone.Key.transform.position.x, GetRandomPosition().y, 0);
                    break;
                }
            }
            
            foreach (var obj in _renderCautionZones)
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