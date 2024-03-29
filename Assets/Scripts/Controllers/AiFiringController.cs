﻿using Assets.Scripts.Core.GameData;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Controllers
{
    public class AiFiringController : MonoBehaviour
    {
        private Gun gun;
        private ProjectileCreator projectileCreator;
        private LevelData levelData;
        private AiRotationControllerBase rotationController;
        private AudioSource shotSound;
        private float projectileOffset;
        private float lastShotWasAt;

        [Inject]
        public void Init(ProjectileCreator creator, LevelData levelData, Gun gun)
        {
            this.projectileCreator = creator;
            this.levelData = levelData;
            this.gun = gun;
        }

        private void Start()
        {
            var bindings = GetComponent<GunBindings>();
            projectileOffset = bindings.ProjectileOffset;

            shotSound = GetComponent<AudioSource>();
            rotationController = GetComponentInParent<AiRotationControllerBase>();
        }

        private void Update()
        {
            if (rotationController.InLineOfSight && (Time.time - lastShotWasAt > gun.FiringRate * levelData.EnemyFirerateFactor))
            {
                var shotDirection = Quaternion.AngleAxis(Random.Range(-levelData.EnemyBulletsSpreadAngle, levelData.EnemyBulletsSpreadAngle), transform.forward) * transform.up;

                // ToDo rewrite to suitable way to get parent for owner of AiFiringController (Get enemy, cannon or boss or player)
                var root = transform.parent.parent.parent.parent;

                projectileCreator.CreateProjectile(gun.ProjectileType, projectileOffset * transform.up + transform.position, transform.rotation, shotDirection, root);
                shotSound.Play();

                lastShotWasAt = Time.time;
            }
        }
    }
}
