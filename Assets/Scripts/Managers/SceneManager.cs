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
        private const string TAG_FOR_NOT_ACTIVATED_OBJECTS = "ShouldBeActive";
        private Dictionary<SceneName, SceneComponent> scenes { get; set; }
        
        private SceneService sceneService;
        private LogService logService;

        public SceneManager(SceneService sceneService, LogService logService)
        {
            this.sceneService = sceneService;
            this.logService = logService;
            scenes = new Dictionary<SceneName, SceneComponent>();
        }

        public async UniTask LoadSceneAsync(SceneName name, bool isActive = true)
        {
            var scene = scenes.First(pair => pair.Key == name).Value;

            if (scene.LoadingScreenNeeded)
            {
                await LoadLoadingScreenAsync();
                await UnloadScenesWithExceptAsync(SceneName.LoadingScreen);

                await UniTask.WhenAll(LoadSceneRecursivelyAsync(scene, false), UniTask.Delay(3000));
                await UnloadScenesWithExceptAsync(scene.Name);
                ActivateSceneObjectsRecursively(scene);
            }
            else
            {
                await LoadSceneRecursivelyAsync(scene);
                await UnloadScenesWithExceptAsync(scene.Name);
            }

            sceneService.SetActiveScene(scene.Name);
            logService.Loggger.ZLogTrace($"Scene was set to active state. ({scene.Name.GetString()})");
        }

        private async UniTask LoadSceneRecursivelyAsync(SceneComponent scene, bool isSceneObjectsActive = true)
        {
            await sceneService.LoadSceneAsync(scene.Name);
            scene.Loaded = true;
            logService.Loggger.ZLogTrace($"Scene loaded. ({scene.Name.GetString()})");

            if (!isSceneObjectsActive)
            {
                sceneService.ChangeStateForObjectsInScene(scene.Name, false, TAG_FOR_NOT_ACTIVATED_OBJECTS);
                logService.Loggger.ZLogTrace($"Scene objects deactivated. ({scene.Name.GetString()})");
            }

            foreach (var dependentScene in scene.DependentScenes)
            {
                await LoadSceneRecursivelyAsync(dependentScene, isSceneObjectsActive);
            }
        }

        private async UniTask UnloadSceneRecursivelyAsync(SceneComponent scene)
        {
            foreach (var dependentScene in scene.DependentScenes)
            {
                await UnloadSceneRecursivelyAsync(dependentScene);
            }

            await sceneService.UnloadSceneAsync(scene.Name);
            scene.Loaded = false;
            logService.Loggger.ZLogTrace($"Scene unloaded. ({scene.Name.GetString()})");
        }

        private async UniTask LoadLoadingScreenAsync()
        {
            var loadingScene = scenes.FirstOrDefault(pair => pair.Key == SceneName.LoadingScreen).Value;

            await LoadSceneRecursivelyAsync(loadingScene);
        }

        private async UniTask UnloadScenesWithExceptAsync(SceneName exceptSceneName)
        {
            foreach (var scene in scenes)
            {
                if (scene.Key != exceptSceneName && scene.Value.Loaded && scene.Key != SceneName.Main)
                {
                    await UnloadSceneRecursivelyAsync(scene.Value);
                }
            }
        }

        private void ActivateSceneObjectsRecursively(SceneComponent scene)
        {
            sceneService.ChangeStateForObjectsInScene(scene.Name, true);
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
            var levelMenuScene = new SceneComponent(SceneName.LevelMenu);
            var gameOver = new SceneComponent(SceneName.GameOver);

            scenes.Add(SceneName.GameMenu, menuScene);
            scenes.Add(SceneName.LoadingScreen, loadingScreenScene);
            scenes.Add(SceneName.Level, levelScene);
            levelScene.Add(guiScene);
            scenes.Add(SceneName.Workshop, workshopScene);
            scenes.Add(SceneName.LevelMenu, levelMenuScene);
            scenes.Add(SceneName.GameOver, gameOver);
        }

        public async UniTask LoadSceneAdditiveAsync(SceneName name)
        {
            var scene = scenes.First(pair => pair.Key == name).Value;
            
            await LoadSceneRecursivelyAsync(scene);
        }

        public async UniTask UnloadSceneAdditiveAsync(SceneName name)
        {
            var scene = scenes.First(pair => pair.Key == name).Value;

            await UnloadSceneRecursivelyAsync(scene);
        }
    }
}
