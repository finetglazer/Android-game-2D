using UnityEngine;

namespace GameObjects.Water
{
    public class WaveAnimation : MonoBehaviour
    {
        private static readonly int Wave = Animator.StringToHash("wave");
        public float delay = 1f;
        private Animator _animator;
        private float _clock;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _clock += Time.deltaTime;
            if (_clock <= delay) return;
            _animator.SetTrigger(Wave);
        }
    }
}