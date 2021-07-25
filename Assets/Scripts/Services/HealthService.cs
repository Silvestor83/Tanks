using System;

namespace Assets.Scripts.Services
{
    public class HealthService
    {
        public event EventHandler<HealthEventArgs> HealthChangedPlayer;
        public event EventHandler<HealthEventArgs> HealthChangedAkvila;

        public void PlayerHealthChanged(int currentHealth, int maxHealth)
        {
            OnPlayerHealthChanged(new HealthEventArgs(maxHealth, currentHealth));
        }

        private void OnPlayerHealthChanged(HealthEventArgs e)
        {
            HealthChangedPlayer?.Invoke(this, e);
        }

        public void AkvilaHealthChanged(int currentHealth, int maxHealth)
        {
            OnAkvilaHealthChanged(new HealthEventArgs(maxHealth, currentHealth));
        }

        private void OnAkvilaHealthChanged(HealthEventArgs e)
        {
            HealthChangedAkvila?.Invoke(this, e);
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

