using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public class HealthService
    {
        public int MaxHealth;
        public int CurrentHealth;

        public event EventHandler<HealthEventArgs> HealthChanged;


        public void Init(int health)
        {
            MaxHealth = CurrentHealth = health;
        }

        public void PlayerHealthChanged(int currentHealth, int maxHealth)
        {
            OnHealthChanged(new HealthEventArgs(maxHealth, currentHealth));
        }

        private void OnHealthChanged(HealthEventArgs e)
        {
            if (HealthChanged != null)
            {
                HealthChanged(this, e);
            }
        }
    }


    public class HealthEventArgs : EventArgs
    {
        public readonly float CurrentHealth;
        public readonly float MaxHealth;

        public HealthEventArgs(float maxHealth, float currentHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }
    }
}

