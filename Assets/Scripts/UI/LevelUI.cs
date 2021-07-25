using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Scripts.UI
{
    public class LevelUI : MonoBehaviour
    {
        private VisualElement healthbar;
        private Label health;
        private VisualElement akvilaHealthbar;
        private Label akvilaHealth;
        private LocalizationService locService;
        private HealthService healthService;

        private void Start()
        {
            healthService.HealthChangedPlayer += PlayerHealthChanged;
            healthService.HealthChangedAkvila += AkvilaHealthChanged;

            healthbar = GetComponent<UIDocument>().rootVisualElement.Q("healthbar");
            health = GetComponent<UIDocument>().rootVisualElement.Q<Label>("health");
            akvilaHealthbar = GetComponent<UIDocument>().rootVisualElement.Q("akvilaHealthbar");
            akvilaHealth = GetComponent<UIDocument>().rootVisualElement.Q<Label>("akvilaHealth");
        }

        [Inject]
        public void Init(HealthService healthService)
        {
            this.healthService = healthService;
        }

        private void PlayerHealthChanged(object o, HealthEventArgs e)
        {
            var currentHealth = e.CurrentHealth < 0 ? 0 : e.CurrentHealth;
            healthbar.style.width = 200 * (currentHealth / e.MaxHealth);
            health.text = currentHealth + " / " + e.MaxHealth;
        }

        private void AkvilaHealthChanged(object o, HealthEventArgs e)
        {
            var currentHealth = e.CurrentHealth < 0 ? 0 : e.CurrentHealth;
            akvilaHealthbar.style.width = 200 * (currentHealth / e.MaxHealth);
            akvilaHealth.text = currentHealth + " / " + e.MaxHealth;
        }

        private void OnDestroy()
        {
            healthService.HealthChangedPlayer -= PlayerHealthChanged;
            healthService.HealthChangedAkvila -= AkvilaHealthChanged;
        }
    }
}
