using System;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        private bool onPause = false;
        private PlayerInput[] playerInputs;
        private SceneManager sceneManager;
        private DestructionService destructionService;

        [Inject]
        public void Init(SceneManager sceneManager, DestructionService destructionService)
        {
            this.sceneManager = sceneManager;
            this.destructionService = destructionService;
        }

        private void Awake()
        {
            playerInputs = GetComponentsInChildren<PlayerInput>();

            destructionService.DamageDone += PlayerTookDamage;
        }
    
        private async void FixedUpdate()
        {
            await OtherActions();
        }

        private async UniTask OtherActions()
        {
            if (onPause && Time.timeScale != 0)
            {
                onPause = false;

                foreach (var playerInput in playerInputs)
                {
                    playerInput.ActivateInput();
                }
            }

            if (Keyboard.current.escapeKey.isPressed && !onPause)
            {
                onPause = true;
                Time.timeScale = 0;

                foreach (var playerInput in playerInputs)
                {
                    playerInput.DeactivateInput();
                }

                await sceneManager.LoadSceneAdditiveAsync(SceneName.LevelMenu);
            }
        }

        private void PlayerTookDamage(object o, DamageEventArgs e)
        {
            if (e.CurrentHealth <= 0)
            {
                _ = sceneManager.LoadSceneAsync(SceneName.GameOver);
            }
        }

        private void OnDestroy()
        {
            destructionService.DamageDone -= PlayerTookDamage;
        }
    }
}
