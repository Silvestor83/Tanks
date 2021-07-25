using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.Services;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Core
{
    public class Initialization : MonoBehaviour
    {
        [SerializeField]
        private MainSettings mainSettings;
        [SerializeField]
        private SettingsSO settingsSo;
        private SceneManager sceneManager;
        private AudioService audioService;

        [Inject]
        public void Init(MainSettings mainSettings, SceneManager sceneManager, AudioService audioService)
        {
            this.mainSettings = mainSettings;
            this.sceneManager = sceneManager;
            this.audioService = audioService;
        }

        private async void Start()
        {
            SetAudioSettings();

            sceneManager.InitScenes();
            await sceneManager.LoadSceneAsync(SceneName.GameMenu);
        }

        private void SetAudioSettings()
        {
            audioService.ChangeMasterVolume(mainSettings.MasterVolume);
            audioService.ChangeMusicVolume(mainSettings.MusicVolume);
            audioService.ChangeEffectsVolume(mainSettings.EffectsVolume);
        }

        private void OnApplicationQuit()
        {
            settingsSo.UpdateSettingFiles();
        }
    }
}
