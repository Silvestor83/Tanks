using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Assets.Scripts.UI.Templates;
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
        private AudioService audioService;
        private OptionsMenuTempalte optionsMenuTemplate;
        private VisualElement mainMenu;
        private VisualElement optionsMenu;

        private void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            mainMenu = root.Q("mainMenu");
            optionsMenu = root.Q("optionsMenu");

            root.Q<Button>("start").clicked += StartButtonPressed;
            root.Q<Button>("options").clicked += OptionsButtonPressed; 
            root.Q<Button>("exit").clicked += ExitButtonPressed;
            root.Q<Button>("back").clicked += BackButtonPressed;

            root.Q<TextElement>("version").text = "ver.: " + Application.version;

            optionsMenuTemplate.Init(optionsMenu);

            //ToDo startButton.text = locService.GetString(LocalizationTables.UI, "");
        }

        [Inject]
        public void Init(MainSettings mainSettings, SceneManager sceneManager, AudioService audioService)
        {
            this.mainSettings = mainSettings;
            this.sceneManager = sceneManager;
            this.audioService = audioService;
            optionsMenuTemplate = new OptionsMenuTempalte(mainSettings, audioService);
        }

        private void StartButtonPressed()
        {
            audioService.PlaySound(AudioSoundName.ButtonClick3);
            _ = sceneManager.LoadSceneAsync(SceneName.Level);
        }

        private void OptionsButtonPressed()
        {
            audioService.PlaySound(AudioSoundName.ButtonClick3);

            mainMenu.style.display = DisplayStyle.None;
            optionsMenu.style.display = DisplayStyle.Flex;
        }

        private void ExitButtonPressed()
        {
            audioService.PlaySound(AudioSoundName.ButtonClick3);
            Application.Quit();
        }

        private void BackButtonPressed()
        {
            audioService.PlaySound(AudioSoundName.ButtonClick3);

            mainMenu.style.display = DisplayStyle.Flex;
            optionsMenu.style.display = DisplayStyle.None;
        }
    }
}
