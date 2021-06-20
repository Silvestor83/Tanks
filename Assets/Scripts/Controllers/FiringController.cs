using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.GameEntities.Units;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class FiringController : MonoBehaviour
    {
        private Gun gun;
        private ProjectileCreator projectileCreator;
        private AudioSource shotSound;
        private float projectileOffset;
        private float lastShootTime = 0;
            
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
            if (Time.time - lastShootTime >= gun.FiringRate)
            {
                lastShootTime = Time.time;

                projectileCreator.CreateProjectile(gun.ProjectileType, projectileOffset * transform.up + transform.position, transform.rotation, transform.up, transform.root);
                shotSound.Play();
            }
        }
    }
}
