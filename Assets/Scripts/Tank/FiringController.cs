using System;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Tank
{
    public class FiringController : MonoBehaviour
    {
        private ProjectileType projectileType;
        private ProjectileCreator projectileCreator;
        private float projectileOffset;

        [Inject]
        public void Init(ProjectileCreator creator, ProjectileType projectileType)
        {
            this.projectileCreator = creator;
            this.projectileType = projectileType;
        }

        private void Start()
        {
            var bindings = GetComponent<GunBindings>();
            projectileOffset = bindings.ProjectileOffset;
        }

        private void OnClick(InputValue value)
        {
            projectileCreator.CreateProjectile(projectileType, projectileOffset * transform.up + transform.position, transform.rotation, transform.up);
        }
    }
}
