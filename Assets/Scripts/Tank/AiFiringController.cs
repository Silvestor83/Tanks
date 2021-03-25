using System;
using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Tank
{
    public class AiFiringController : MonoBehaviour
    {
        private Gun gun;
        private ProjectileCreator projectileCreator;
        private LevelData levelData;
        private AiRotationControllerBase rotationController;
        private float projectileOffset;
        private float lastShotWasAt;

        [Inject]
        public void Init(ProjectileCreator creator, LevelData levelData, Gun gun)
        {
            this.projectileCreator = creator;
            this.levelData = levelData;
            this.gun = gun;
        }

        private void Awake()
        {
            var bindings = GetComponent<GunBindings>();
            projectileOffset = bindings.ProjectileOffset;

            rotationController = GetComponentInParent<AiRotationControllerBase>();
        }

        private void Update()
        {
            if (rotationController.InLineOfSight)
            {
                if (Time.time - lastShotWasAt > gun.FiringRate * levelData.EnemyFirerateFactor)
                {
                    var shotDirection = Quaternion.AngleAxis(Random.Range(-levelData.EnemyBulletsSpreadAngle, levelData.EnemyBulletsSpreadAngle), transform.forward) * transform.up;

                    // ToDo rewrite to suitable way to get parent for owner of AiFiringController (Get enemy, cannon or boss or player)
                    var parent = transform.parent.parent.parent.parent;

                    projectileCreator.CreateProjectile(gun.ProjectileType, projectileOffset * transform.up + transform.position, transform.rotation, shotDirection, parent);

                    lastShotWasAt = Time.time;
                }
            }
        }
    }
}
