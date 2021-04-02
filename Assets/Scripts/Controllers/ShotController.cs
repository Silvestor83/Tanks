using Assets.Scripts.GameEntities;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Assets.Scripts.Controllers
{
    public class ShotController : MonoBehaviour
    {
        [FormerlySerializedAs("bullet")] public Rigidbody2D bulletRB;

        private Projectile projectile;
        private Vector2 direction;
        private ExplosionCreator explosionCreator;
        private DestructionService destructionService;

        [Inject]
        public void Init(ExplosionCreator explosionCreator, DestructionService destructionService, Projectile projectile, Vector2 direction)
        {
            this.explosionCreator = explosionCreator;
            this.destructionService = destructionService;
            this.projectile = projectile;
            this.direction = direction;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            bulletRB.AddForce(direction * projectile.Speed, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            explosionCreator.CreateExplosion(projectile.explosionType, collision.contacts[0].point);
            CheckDestruction(collision.gameObject);
            Destroy(gameObject);
        }

        private void CheckDestruction(GameObject collisionGameObject)
        {
            destructionService.CheckDestruction(collisionGameObject, projectile);
        }
    }
}
