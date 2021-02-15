using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Assets.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        private VisualElement root;
        private LocalizationService locService;
        private SceneManager sceneManager;
        private MainSettings mainSettings;

        private void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            var startButton = root.Q<Button>("start");
            var optionsButton = root.Q<Button>("options");
            var exitButton = root.Q<Button>("exit");

            //startButton.text = locService.GetString(LocalizationTables.UI, "");

            //root.Q<Button>("exit").RegisterCallback<MouseDownEvent>();



            startButton.clicked += StartButtonPressed;
            optionsButton.clicked += OptionsButtonPressed;
            exitButton.clicked += ExitButtonPressed;
        }

        [Inject]
        public void Init(SceneManager sceneManager, MainSettings mainSettings)
        {
            this.sceneManager = sceneManager;
            this.mainSettings = mainSettings;
        }

        private void StartButtonPressed()
        {
            _ = sceneManager.LoadScene(SceneName.Level);
        }

        private void OptionsButtonPressed()
        {
            _ = sceneManager.LoadScene(SceneName.Workshop);
        }

        private void ExitButtonPressed()
        {
            Application.Quit();
        }
    }
}
