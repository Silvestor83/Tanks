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
        private LocalizationService locService;
        private HealthService healthService;

        private void Start()
        {
            healthService.HealthChanged += PlayerHealthChanged;

            healthbar = GetComponent<UIDocument>().rootVisualElement.Q("healthbar");
            health = GetComponent<UIDocument>().rootVisualElement.Q<Label>("health");
        }

        [Inject]
        public void Init(HealthService healthService)
        {
            this.healthService = healthService;
        }

        private void PlayerHealthChanged(object o, HealthEventArgs e)
        {
            healthbar.style.width = 200 * (e.CurrentHealth / e.MaxHealth);
            health.text = e.CurrentHealth + " / " + e.MaxHealth;
        }

        private void OnDestroy()
        {
            healthService.HealthChanged += PlayerHealthChanged;
        }
    }
}
