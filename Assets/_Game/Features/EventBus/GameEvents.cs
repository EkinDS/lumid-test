using _Game.Infrastructure;
using UnityEngine;

namespace _Game
{
    public class GameEvents : MonoBehaviour
    {
        public EventBus Bus { get; private set; }

        void Awake()
        {
            Bus = new EventBus();
        }
    }
}