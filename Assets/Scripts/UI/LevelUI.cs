using Assets.Scripts.Core.Settings;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.Scripts.UI
{
    public class LevelUI : MonoBehaviour
    {
        private VisualElement healthbar;
        private Label health;
        private LocalizationService locService;
        private SceneManager sceneManager;
        private MainSettings mainSettings;
        private DestructionService destructionService;

        private void Awake()
        {
            destructionService.DamageDone += PlayerTookDamage;
        }

        private void Start()
        {
            healthbar = GetComponent<UIDocument>().rootVisualElement.Q("healthbar");
            health = GetComponent<UIDocument>().rootVisualElement.Q<Label>("health");
        }

        [Inject]
        public void Init(SceneManager sceneManager, MainSettings mainSettings, DestructionService destructionService)
        {
            this.sceneManager = sceneManager;
            this.mainSettings = mainSettings;
            this.destructionService = destructionService;
        }

        private void PlayerTookDamage(object o, DamageEventArgs e)
        {
            healthbar.style.width = 200 * (e.CurrentHealth / e.MaxHealth);
            health.text = e.CurrentHealth + " / " + e.MaxHealth;
        }
    }
}
