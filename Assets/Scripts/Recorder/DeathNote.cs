using System.Collections.Generic;
using System.Linq;
using GameObjects.Texture.TemporaryTexture;
using OtherCharacters.Merchant;
using UnityEngine;
using Movement = OtherCharacters.Merchant.Movement;

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
        private static readonly List<KeyValuePair<GameObject, float>> MobileTextureList = new();

        public static void AddObject(GameObject obj, Vector3 position)
        {
            List.Add(new KeyValuePair<GameObject, Vector3>(obj, position));
            var movementScript = obj.GetComponent<GameObjects.Texture.MobileTexture.Movement>();
            if (movementScript is not null)
            {
                MobileTextureList.Add(new KeyValuePair<GameObject, float>(obj, movementScript.clock));
            }
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
                        targetedGameObject.GetComponent<Movement>().enabled = true;
                        targetedGameObject.GetComponent<AttackHandler>().enabled = true;
                        break;
                    case PriestEnemyTag:
                        targetedGameObject.GetComponent<OtherCharacters.Priest.Movement>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Priest.AttackHandler>().enabled = true;
                        break;
                    case SoldierEnemyTag:
                        targetedGameObject.GetComponent<OtherCharacters.Soldier.Movement>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Soldier.AttackHandler>().enabled = true;
                        break;
                    case ThiefEnemyTag:
                        targetedGameObject.GetComponent<OtherCharacters.Thief.Movement>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Thief.AttackHandler>().enabled = true;
                        break;
                    case PeasantEnemyTag:
                        targetedGameObject.GetComponent<OtherCharacters.Peasant.Movement>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Peasant.AttackHandler>().enabled = true;
                        break;
                    default:
                        targetedGameObject.GetComponent<OtherCharacters.Peasant.Movement>().enabled = true;
                        targetedGameObject.GetComponent<OtherCharacters.Peasant.AttackHandler>().enabled = true;
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
    }
}

