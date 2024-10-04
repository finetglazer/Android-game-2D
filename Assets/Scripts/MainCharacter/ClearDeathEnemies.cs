using UnityEngine;

namespace MainCharacter
{
    public class ClearDeathEnemies : MonoBehaviour
    {
        private const string MerchantEnemyTag = "MerchantEnemy";
        private const string PeasantEnemyTag = "PeasantEnemy";
        private const string PriestEnemyTag = "PriestEnemy";
        private const string SoldierEnemyTag = "SoldierEnemy";
        private const string ThiefEnemyTag = "ThiefEnemy";
        
        public static void Clear()
        {
            var gameObjects = FindObjectsOfType<GameObject>();
            foreach (var x in gameObjects)
            {
                if ((x.CompareTag(MerchantEnemyTag) && x.GetComponent<Others.Merchant.AttackHandler>() is null)
                    || (x.CompareTag(PeasantEnemyTag) && x.GetComponent<Others.Peasant.AttackHandler>() is null)
                    || (x.CompareTag(PriestEnemyTag) && x.GetComponent<Others.Priest.AttackHandler>() is null)
                    || (x.CompareTag(SoldierEnemyTag) && x.GetComponent<Others.Soldier.AttackHandler>() is null)
                    || (x.CompareTag(ThiefEnemyTag) && x.GetComponent<Others.Thief.AttackHandler>() is null))
                {
                    x.SetActive(false);
                }
            }
        }
    }
}