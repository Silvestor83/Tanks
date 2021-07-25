using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Controllers;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.Core.Settings;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.GameEntities.Units;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class EnhancementService
    {
        private TankCreator tankCreator;
        private readonly PlayerData playerData;
        private readonly PlayerSettings playerSettings;

        public EnhancementService(TankCreator tankCreator, PlayerData playerData, PlayerSettings playerSettings)
        {
            this.tankCreator = tankCreator;
            this.playerData = playerData;
            this.playerSettings = playerSettings;
        }

        public async UniTask UpgradeMechanicalPart(MechanicalPart part, GameObject rootGameObject)
        {
            MechanicalPart oldPart;

            switch (part)
            {
                case Hull _:
                    oldPart = playerSettings.Hulls.First(t => t.Name == playerData.hullName);
                    break;
                case Tower _:
                    oldPart = playerSettings.Towers.First(t => t.Name == playerData.towerName);
                    break;
                case Track _:
                    oldPart = playerSettings.Tracks.First(t => t.Name == playerData.trackName);
                    break;
                case Gun _:
                    oldPart = playerSettings.Guns.First(t => t.Name == playerData.gunName);
                    break;
                default:
                    throw new Exception("Wrong type of the MechanicalPart to upgrade MechanicalPart.");
            }

            var healthDifference = part.Durability - oldPart.Durability;

            if (healthDifference != 0)
            {
                UpdateHealth(healthDifference, rootGameObject);
            }

            await tankCreator.UpgradePlayerTankAsync(part, rootGameObject);
        }

        private void UpdateHealth(int healthDifference, GameObject rootGameObject)
        {
            var healthController = rootGameObject.GetComponent<HealthController>();
            var currentHealth = healthDifference > 0 ? healthController.CurrentHealth + healthDifference : healthController.CurrentHealth;

            healthController.ChangeHealth(currentHealth, healthController.MaxHealth + healthDifference);
        }

        public void UpdateHealth(GameObject rootGameObject)
        {
            var healthController = rootGameObject.GetComponent<HealthController>();
            int currentHealth;

            if (healthController.MaxHealth * 0.2f + healthController.CurrentHealth > healthController.MaxHealth)
            {
                currentHealth = healthController.MaxHealth;
            }
            else
            {
                currentHealth = healthController.CurrentHealth + (int)(healthController.MaxHealth * 0.2f);
            }

            healthController.ChangeHealth(currentHealth);
        }
    }
}
