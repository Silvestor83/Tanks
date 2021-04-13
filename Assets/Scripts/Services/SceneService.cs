using System;
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

        public UniTask UnloadSceneAsync(SceneName sceneName)
        {
            return SceneManager.UnloadSceneAsync(sceneName.GetString()).ToUniTask();
        }

        /// <summary>
        /// Change state from active to not active or vise versa, for scene
        /// </summary>
        public void ChangeStateForObjectsInScene(SceneName sceneName, bool isActive, string exceptTagged = null)
        {
            var scene = SceneManager.GetSceneByName(sceneName.GetString());
            var objects = scene.GetRootGameObjects();

            foreach (var obj in objects)
            {
                if (exceptTagged == null || !obj.CompareTag(exceptTagged))
                {
                    obj.SetActive(isActive);
                }
            }
        }

        public void SetActiveScene(SceneName sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName.GetString());
            SceneManager.SetActiveScene(scene);
        }
    }
}
