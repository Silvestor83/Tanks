using System;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.GameEntities.Units;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Tank
{
    public class FiringController : MonoBehaviour
    {
        private Gun gun;
        private ProjectileCreator projectileCreator;
        private AudioSource shotSound;
        private float projectileOffset;

        [Inject]
        public void Init(ProjectileCreator creator, Gun gun)
        {
            this.projectileCreator = creator;
            this.gun = gun;
        }

        private void Start()
        {
            var bindings = GetComponent<GunBindings>();
            projectileOffset = bindings.ProjectileOffset;

            shotSound = GetComponent<AudioSource>();
        }

        private void OnClick(InputValue value)
        {
            var root = transform.root;
            projectileCreator.CreateProjectile(gun.ProjectileType, projectileOffset * transform.up + transform.position, transform.rotation, transform.up, root);
            shotSound.Play();
        }
    }
}
