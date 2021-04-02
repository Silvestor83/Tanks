using System;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using Random = UnityEngine.Random;

namespace Assets.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        private VisualElement root;
        private LocalizationService locService;
        private SceneManager sceneManager;
        private AudioService audioService;

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
        public void Init(SceneManager sceneManager, AudioService audioService)
        {
            this.sceneManager = sceneManager;
            this.audioService = audioService;
        }

        private void StartButtonPressed()
        {
            audioService.PlaySound(AudioSoundName.ButtonClick3);
            _ = sceneManager.LoadScene(SceneName.Level);
        }

        private void OptionsButtonPressed()
        {
            audioService.PlaySound(AudioSoundName.ButtonClick3);
        }

        private void ExitButtonPressed()
        {
            audioService.PlaySound(AudioSoundName.ButtonClick3);
            Application.Quit();
        }
    }
}
