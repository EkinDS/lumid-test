using System.Collections.Generic;
using _Game.Features.Bosses;
using _Game.Features.Humans;
using _Game.Features.HumansState.Scripts.Combat;
using _Game.Features.HumansState.Scripts.Portal;
using _Game.Features.HumansState.Scripts.Spawn;
using _Game.Features.HumansState.Scripts.Training;
using _Game.Features.HumansState.Scripts.Waiting;
using _Game.Infrastructure;
using UnityEngine;

namespace _Game.Features.HumansState.Scripts.Core
{
    public class HumanStateController : MonoBehaviour
    {
        [SerializeField] private HumanPresenter _humanPrefab;
        [SerializeField] private BossPresenter _bossPrefab;
        [SerializeField] private HumanData _data;
        [SerializeField] private GameEvents _eventBus;

        public List<HumanPresenter> Humans { get; } = new();

        private List<HumanState> _states;

        public TrainingData trainingData = new TrainingData();

        
        public EventBus EventBus => _eventBus.Bus;
        
        private void Start()
        {
            trainingData.trainingAttackingInterval = _data.humanAttackIntervalLevelData[0].attackInterval;
            trainingData.trainingMaximumHealth = _data.humanMaximumHealthLevelData[0].maximumHealth;
            trainingData.trainingDamage = _data.humanDamageLevelData[0].damage;
            trainingData.trainingMovementSpeed = _data.humanMovementSpeedLevelData[0].movementSpeed;
            
            _states = new List<HumanState>()
            {
                new SpawnState(this, _humanPrefab),
                new PortalState(this),
                new WaitingState(this),
                new TrainingState(this),
                new CombatState(this, _bossPrefab),
            };

            TransitionTo<SpawnState>(null);
        }


        public HumanData GetHumanData()
        {
            return _data;
        }

        public void TransitionTo<T>(HumanPresenter human = null) where T : HumanState
        {
            foreach (var state in _states)
            {
                if (state.GetType() == typeof(T))
                {
                    state.OnEnter(human);
                }
            }
        }

        public bool FreeSlotIn<T>() where T : HumanState
        {
            foreach (var state in _states)
            {
                if (state.GetType() == typeof(T))
                {
                    return state.HasFreeSlot();
                }
            }

            return false;
        }

        public void RegisterHuman(HumanPresenter human)
        {
            if (!Humans.Contains(human))
                Humans.Add(human);
        }

        public void UnregisterHuman(HumanPresenter human)
        {
            Humans.Remove(human);
        }

        public class TrainingData
        {
            public int trainingMaximumHealth;
            public float trainingMovementSpeed;
            public float trainingAttackingInterval;
            public int trainingDamage;
        }
    }
}