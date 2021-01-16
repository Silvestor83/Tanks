using Assets.Scripts.Core.Settings;
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

        private int count;

        [Inject]
        public void Init(MainSettings mainSettings)
        {
            this.mainSettings = mainSettings;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnDrawGizmos()
        {
            count++;

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * 10f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(new Vector3(20 , 0) , Vector3.one * 10f);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(0, mainSettings.DistanceForRed), Vector3.one * 10f);

            Debug.Log(count);
            Debug.Log(count);
        }

        private void OnApplicationQuit()
        {
            settingsSo.UpdateSettingFiles();
        }
    }
}
