using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Services
{
    public class SceneService
    {
        public UniTask LoadSceneAsync(SceneName sceneName)
        {
            return SceneManager.LoadSceneAsync(sceneName.GetString(), LoadSceneMode.Additive).ToUniTask();
        }

        public UniTask UnloadScene(SceneName sceneName)
        {
            return SceneManager.UnloadSceneAsync(sceneName.GetString()).ToUniTask();
        }

        public void ActivateSceneObjects(SceneName sceneName, bool isActive)
        {
            var scene = SceneManager.GetSceneByName(sceneName.GetString());
            var objects = scene.GetRootGameObjects();

            foreach (var obj in objects)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
