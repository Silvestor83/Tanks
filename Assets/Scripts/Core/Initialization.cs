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

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.cyan;
            //Gizmos.DrawCube(Vector3.zero, Vector3.one * 10f);

            //Gizmos.color = Color.magenta;
            //Gizmos.DrawCube(new Vector3(20 , 0) , Vector3.one * 10f);

            //Gizmos.color = Color.red;
            //Gizmos.DrawCube(new Vector3(0, mainSettings.DistanceForRed), Vector3.one * 10f);
        }

        private void OnApplicationQuit()
        {
            settingsSo.UpdateSettingFiles();
        }
    }
}
