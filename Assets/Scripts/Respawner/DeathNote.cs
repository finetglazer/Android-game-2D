﻿using System.Collections.Generic;
using System.Linq;
using GameObjects.Texture.TemporaryTexture;
using UnityEngine;

namespace Respawner
{
    public class DeathNote : MonoBehaviour
    {
        private const string MerchantEnemyTag = "MerchantEnemy";
        private const string SoldierEnemyTag = "SoldierEnemy";
        private const string ThiefEnemyTag = "ThiefEnemy";
        private static readonly List<KeyValuePair<GameObject, Vector3>> List = new(); 
        private static readonly List<KeyValuePair<GameObject, float>> MobileTextureList = new();
        private static readonly List<(GameObject, float, float, Vector3)> ImmortalEnemiesList = new();

        private void Update()
        {
            if (ImmortalEnemiesList.Count == 0) return;

            for (var i = 0; i < ImmortalEnemiesList.Count; i++)
            {
                var cooldownTime = ImmortalEnemiesList[i].Item2;
                var currentClock = ImmortalEnemiesList[i].Item3;
                currentClock += Time.deltaTime;
                if (currentClock < cooldownTime)
                {
                    ImmortalEnemiesList[i] = (ImmortalEnemiesList[i].Item1, ImmortalEnemiesList[i].Item2,
                        ImmortalEnemiesList[i].Item3 + Time.deltaTime, ImmortalEnemiesList[i].Item4);
                    continue;
                }
                ReRenderImmortalEnemy(ImmortalEnemiesList[i].Item1.name);
                ImmortalEnemiesList.RemoveAt(i);
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static void AddObject(GameObject obj, Vector3 position)
        {
            List.Add(new KeyValuePair<GameObject, Vector3>(obj, position));
            var movementScript = obj.GetComponent<GameObjects.Texture.MobileTexture.Movement>();
            if (movementScript is not null)
            {
                MobileTextureList.Add(new KeyValuePair<GameObject, float>(obj, movementScript.clock));
            }
        }

        public static void ClearLists()
        {
            List.Clear();
            MobileTextureList.Clear();
            ImmortalEnemiesList.Clear();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void ReRender()
        {
            foreach (var obj in List.Where(obj => !obj.Key.name.ToLower().Contains("priest")))
            {
                ReRenderObject(obj.Key.name);
            }
        }

        public static void PrintAll()
        {
            foreach (var (obj, position) in List)
            {
                print("Name: " + obj.name + " Position: " + position);
            }
        }

        private static void ReRenderObject(string objName)
        {
            print("Re-rendering object: " + objName);
            KeyValuePair<GameObject, Vector3> temp = new(null, Vector3.zero);
            foreach (var i in List.Where(i => i.Key.name == objName))
            {
                temp = i;
                break;
            }

            if (temp.Key is null)
            {
                print("Game object: " + objName + " not found");
                return;
            }
            
            var targetedGameObject = temp.Key;
            targetedGameObject.SetActive(true);
            targetedGameObject.transform.position = temp.Value;
            
            // In case enemies are re-rendered
            if (targetedGameObject.tag.Contains("Enemy"))
            {
                switch (targetedGameObject.tag)
                {
                    case MerchantEnemyTag:
                        targetedGameObject.GetComponent<OtherCharacters.Merchant.AttackHandler>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Merchant.Movement>().enabled = true;
                        var merchantMovement = targetedGameObject.GetComponent<OtherCharacters.Merchant.Movement>();
                        merchantMovement.currentHealth = 3;
                        merchantMovement.healthBar.SetActive(true);
                        break;
                    case SoldierEnemyTag:
                        targetedGameObject.GetComponent<OtherCharacters.Soldier.AttackHandler>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Soldier.Movement>().enabled = true;
                        var soldierMovement = targetedGameObject.GetComponent<OtherCharacters.Soldier.Movement>();
                        soldierMovement.currentHealth = 3;
                        soldierMovement.healthBar.SetActive(true);
                        break;
                    case ThiefEnemyTag:
                        targetedGameObject.GetComponent<OtherCharacters.Thief.AttackHandler>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Thief.Movement>().enabled = true;
                        var thiefMovement = targetedGameObject.GetComponent<OtherCharacters.Thief.Movement>();
                        thiefMovement.currentHealth = 3;
                        thiefMovement.healthBar.SetActive(true);
                        break;
                    default:
                        targetedGameObject.GetComponent<OtherCharacters.Peasant.AttackHandler>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Peasant.Movement>().enabled = true; 
                        var peasantMovement = targetedGameObject.GetComponent<OtherCharacters.Peasant.Movement>();
                        peasantMovement.currentHealth = 3;
                        peasantMovement.healthBar.SetActive(true);
                        break;
                }
            }
            if (targetedGameObject.GetComponent<TemporaryTexture>() is not null)
            {
                var temporaryTextureScript = targetedGameObject.GetComponent<TemporaryTexture>();
                temporaryTextureScript.timeOnTexture = 0;
                temporaryTextureScript.playerIsOnTexture = false;
                temporaryTextureScript.textureActive = true;
            }
            if (targetedGameObject.GetComponent<GameObjects.Texture.MobileTexture.Movement>() is not null)
            {
                var clock = MobileTextureList.Find(
                    obj => obj.Key.name == objName).Value;
                var mobileTextureScript = targetedGameObject.GetComponent<GameObjects.Texture.MobileTexture.Movement>();
                mobileTextureScript.clock = clock;
            }
        }

        public static void AddImmortalEnemy(GameObject obj, float cooldownTime, Vector3 position)
        {
            ImmortalEnemiesList.Add((obj, cooldownTime, 0, position));
        }

        private static void ReRenderImmortalEnemy(string objName)
        {
            print("Re-rendering Immortal Enemy:  " + objName);
            var targetedEnemy = ImmortalEnemiesList.Find(enemy => enemy.Item1.name == objName);
            targetedEnemy.Item1.SetActive(true);
            targetedEnemy.Item1.transform.position = targetedEnemy.Item4;
        }
    }
}

