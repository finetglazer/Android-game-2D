using System.Collections.Generic;
using System.Linq;
using GameObjects.Texture;
using UnityEngine;

namespace Recorder
{
    public class DeathNote : MonoBehaviour
    {
        private const string MerchantEnemyTag = "MerchantEnemy";
        private const string PeasantEnemyTag = "PeasantEnemy";
        private const string PriestEnemyTag = "PriestEnemy";
        private const string SoldierEnemyTag = "SoldierEnemy";
        private const string ThiefEnemyTag = "ThiefEnemy";
        private static readonly List<KeyValuePair<GameObject, Vector3>> List = new(); 
            
        public static void AddObject(GameObject obj, Vector3 position)
        {
            List.Add(new KeyValuePair<GameObject, Vector3>(obj, position));
        }

        public static void ClearList()
        {
            List.Clear();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void ReRender()
        {
            foreach (var obj in List)
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
                        targetedGameObject.GetComponent<Others.Merchant.Movement>().enabled = true;
                        targetedGameObject.GetComponent<Others.Merchant.AttackHandler>().enabled = true;
                        break;
                    case PriestEnemyTag:
                        targetedGameObject.GetComponent<Others.Priest.Movement>().enabled = true;
                        targetedGameObject.GetComponent<Others.Priest.AttackHandler>().enabled = true;
                        break;
                    case SoldierEnemyTag:
                        targetedGameObject.GetComponent<Others.Soldier.Movement>().enabled = true;
                        targetedGameObject.GetComponent<Others.Soldier.AttackHandler>().enabled = true;
                        break;
                    case ThiefEnemyTag:
                        targetedGameObject.GetComponent<Others.Thief.Movement>().enabled = true;
                        targetedGameObject.GetComponent<Others.Thief.AttackHandler>().enabled = true;
                        break;
                    case PeasantEnemyTag:
                        targetedGameObject.GetComponent<Others.Peasant.Movement>().enabled = true;
                        targetedGameObject.GetComponent<Others.Peasant.AttackHandler>().enabled = true;
                        break;
                    default:
                        targetedGameObject.GetComponent<Others.Peasant.Movement>().enabled = true;
                        targetedGameObject.GetComponent<Others.Peasant.AttackHandler>().enabled = true;
                        break;
                }
            }
            else if (targetedGameObject.name.ToLower().Contains("tmp"))
            {
                var temporaryTextureScript = targetedGameObject.GetComponent<TemporaryTexture>();
                temporaryTextureScript.timeOnTexture = 0;
                temporaryTextureScript.playerIsOnTexture = false;
                temporaryTextureScript.textureActive = true;
            }
        }
    }
}