using Assets.Scripts.Core.Settings;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjects;
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

        [Inject]
        public void Init(MainSettings mainSettings, SceneManager sceneManager)
        {
            this.mainSettings = mainSettings;
            this.sceneManager = sceneManager;
        }

        // Start is called before the first frame update
        private async void Start()
        {
            sceneManager.InitScenes();
            await sceneManager.LoadScene(SceneName.GameMenu);
        }

        private void OnApplicationQuit()
        {
            settingsSo.UpdateSettingFiles();
        }
    }
}
