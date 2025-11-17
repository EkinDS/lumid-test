using _Game.Infrastructure;
using UnityEngine;

namespace _Game
{
    public class GameEvents : MonoBehaviour
    {
        public EventBus Bus { get; private set; }

        public void Initialize()
        {            print("init bus");

            Bus = new EventBus();
        }
    }
}