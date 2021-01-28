using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.GameEntities;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLogger;

namespace Assets.Scripts.Managers
{
    public class SceneManager
    {
        private Dictionary<SceneName, SceneComponent> scenes { get; set; }
        
        private SceneService sceneService;
        private LogService logService;

        public SceneManager(SceneService sceneService, LogService logService)
        {
            this.sceneService = sceneService;
            this.logService = logService;
            scenes = new Dictionary<SceneName, SceneComponent>();
        }

        public async UniTask LoadScene(SceneName name, bool isActive = true)
        {
            var scene = scenes.First(pair => pair.Key == name).Value;

            if (scene.LoadingScreenNeeded)
            {
                await LoadLoadingScreen();
                await UnloadScenes(SceneName.LoadingScreen);

                await UniTask.WhenAll(LoadSceneRecursively(scene, false), UniTask.Delay(10000));
                await UnloadScenes(scene.Name);
                ActivateSceneObjectsRecursively(scene);
                return;
            }

            await LoadSceneRecursively(scene);
            await UnloadScenes(scene.Name);
        }

        private async UniTask LoadSceneRecursively(SceneComponent scene, bool isSceneObjectsActive = true)
        {
            await sceneService.LoadSceneAsync(scene.Name);
            scene.Loaded = true;
            logService.Loggger.ZLogTrace($"Scene loaded. ({scene.Name.GetString()})");

            if (!isSceneObjectsActive)
            {
                sceneService.ActivateSceneObjects(scene.Name, false);
                logService.Loggger.ZLogTrace($"Scene objects deactivated. ({scene.Name.GetString()})");
            }

            foreach (var dependentScene in scene.DependentScenes)
            {
                await LoadSceneRecursively(dependentScene, isSceneObjectsActive);
            }
        }

        private async UniTask LoadLoadingScreen()
        {
            var loadingScene = scenes.FirstOrDefault(pair => pair.Key == SceneName.LoadingScreen).Value;

            await LoadSceneRecursively(loadingScene);
        }

        private async UniTask UnloadScenes(SceneName exceptSceneName)
        {
            foreach (var scene in scenes)
            {
                if (scene.Key != exceptSceneName && scene.Value.Loaded)
                {
                    await sceneService.UnloadScene(scene.Key);
                    scene.Value.Loaded = false;
                    logService.Loggger.ZLogTrace($"Scene unloaded. ({scene.Key.GetString()})");
                }
            }
        }

        private void ActivateSceneObjectsRecursively(SceneComponent scene)
        {
            sceneService.ActivateSceneObjects(scene.Name, true);
            logService.Loggger.ZLogTrace($"Scene objects activated. ({scene.Name.GetString()})");

            foreach (var dependentScene in scene.DependentScenes)
            {
                ActivateSceneObjectsRecursively(dependentScene);
            }
        }

        public void InitScenes()
        {
            var menuScene = new SceneComponent(SceneName.GameMenu);
            var loadingScreenScene = new SceneComponent(SceneName.LoadingScreen);
            var guiScene = new SceneComponent(SceneName.GUI);
            var levelScene = new SceneComponent(SceneName.Level, true);
            var workshopScene = new SceneComponent(SceneName.Workshop, true);

            scenes.Add(SceneName.GameMenu, menuScene);
            scenes.Add(SceneName.LoadingScreen, loadingScreenScene);
            scenes.Add(SceneName.Level, levelScene);
            levelScene.Add(guiScene);
            scenes.Add(SceneName.Workshop, workshopScene);
        }
    }
}
