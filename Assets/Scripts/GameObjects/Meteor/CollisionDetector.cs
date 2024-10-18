using MainCharacter;
using UnityEngine;

namespace GameObjects.Meteor
{
    public class CollisionDetector : MonoBehaviour
    {
        private const string PlayerTag = "Player";
        private const string EnemyTag = "Enemy";
        private static readonly int Die = Animator.StringToHash("die");

        private static void CausesDamage(GameObject obj)
        {
            var objAnimator = obj.GetComponent<Animator>();
            objAnimator.SetTrigger(Die);
            if (obj.tag.Contains(EnemyTag))
            {
                ClearDeathEnemies.Clear();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(PlayerTag) && !other.tag.Contains(EnemyTag)) return;
            CausesDamage(other.gameObject);
        }
        
    }
}