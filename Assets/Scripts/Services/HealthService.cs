using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Services
{
    public class HealthService
    {
        public event EventHandler<HealthEventArgs> HealthChanged;
        public event EventHandler<HealthEventArgs> HealthChangedAkvila;

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

        public void AkvilaHealthChanged(int currentHealth, int maxHealth)
        {
            OnAkvilaHealthChanged(new HealthEventArgs(maxHealth, currentHealth));
        }

        private void OnAkvilaHealthChanged(HealthEventArgs e)
        {
            if (HealthChangedAkvila != null)
            {
                HealthChangedAkvila(this, e);
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

