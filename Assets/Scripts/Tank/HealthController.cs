using UnityEngine;

namespace Assets.Scripts.Tank
{
    public class HealthController : MonoBehaviour
    {
        public int MaxHealth;
        public int CurrentHealth;

        public void Init(int health)
        {
            MaxHealth = CurrentHealth = health;
        }
    }
}
