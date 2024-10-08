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
                if ((x.CompareTag(MerchantEnemyTag) && !x.GetComponent<OtherCharacters.Merchant.AttackHandler>().enabled)
                    || (x.CompareTag(PeasantEnemyTag) && !x.GetComponent<OtherCharacters.Peasant.AttackHandler>().enabled)
                    || (x.CompareTag(PriestEnemyTag) && !x.GetComponent<OtherCharacters.Priest.AttackHandler>().enabled)
                    || (x.CompareTag(SoldierEnemyTag) && !x.GetComponent<OtherCharacters.Soldier.AttackHandler>().enabled)
                    || (x.CompareTag(ThiefEnemyTag) && !x.GetComponent<OtherCharacters.Thief.AttackHandler>().enabled))
                {
                    x.SetActive(false);
                }
            }
        }
    }
}