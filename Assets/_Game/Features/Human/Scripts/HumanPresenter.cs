using System.Collections;
using _Game.Features.Bosses;
using _Game.Features.PlayerWallet;
using UnityEngine;

namespace _Game.Features.Humans
{
    [RequireComponent(typeof(HumanView))]
    public class HumanPresenter : MonoBehaviour
    {
        [Header("Base Stats")] 
        [SerializeField] private int _baseHealth;
        [SerializeField] private int _baseDamage;

        private HumanModel _model;
        private HumanView _view;
        private BossView _boss;
        private Coroutine _attackRoutine;

        public bool IsDead => _model.IsDead;

        private void Awake()
        {
            _view = GetComponent<HumanView>();
        }

        public void Initialize()
        {
            _model = new HumanModel(_baseHealth, _baseDamage);

            _model.OnHealthChanged += HandleHealthChanged;
            _model.OnDied += HandleDied;

            _view.ArrangeHealthBar(_model.Health, _model.MaximumHealth);
        }
        
        public void MoveTo(Vector3 targetPosition, System.Action<HumanPresenter> onReached)
        {
            _view.MoveTo(targetPosition, 1F, () => onReached?.Invoke(this));
        }

        public void Train()
        {
            _model.Train(10, 10);
        }

        public void StartAttacking(BossView bossView)
        {
            _boss = bossView;

            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);

            _view.SetFightingPosition();
            _attackRoutine = StartCoroutine(AttackLoop());
        }

        public void TakeDamage(int damage)
        {
            _model.TakeDamage(damage);
            _view.PlayHitAnimationDown();
        }
        
        private void HandleHealthChanged(int current, int max)
        {
            _view.ArrangeHealthBar(current, max);
        }

        private void HandleDied()
        {
            Debug.Log("Human Dead!");
            if (_attackRoutine != null)
                StopCoroutine(_attackRoutine);

            Destroy(gameObject);
        }
        
        private IEnumerator AttackLoop()
        {
            var wait = new WaitForSeconds(1f);

            while (_boss != null && _boss.IsAlive() && !_model.IsDead)
            {
                Wallet.AddCoins(_model.Damage);
                _boss.TakeDamage(_model.Damage);
                _view.PlayAttackAnimationUp();

                yield return wait;
            }
        }
    }
}
