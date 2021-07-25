using Assets.Scripts.GameEntities;
using Assets.Scripts.GameEntities.Creators;
using Assets.Scripts.Infrastructure.Enums;
using Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
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
        private bool inCollision = false;

        [Inject]
        public void Init(ExplosionCreator explosionCreator, DestructionService destructionService, Projectile projectile, Vector2 direction)
        {
            this.explosionCreator = explosionCreator;
            this.destructionService = destructionService;
            this.projectile = projectile;
            this.direction = direction;
        }

        private void Start()
        {
            bulletRB.AddForce(direction * projectile.Speed, ForceMode2D.Impulse);
        }

        private async void OnCollisionEnter2D(Collision2D collision)
        {
            if (!inCollision)
            {
                inCollision = true;
                explosionCreator.CreateExplosion(projectile.explosionType, collision.contacts[0].point);
                Destroy(gameObject);

                if (collision.gameObject.layer == (int) GameObjectLayer.DestructibleObstacles)
                {
                    Vector3 hitPosition = Vector3.zero;
                    var tilemap = collision.gameObject.GetComponent<Tilemap>();
                    foreach (ContactPoint2D hit in collision.contacts)
                    {
                        hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                        hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
                        tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
                    }
                }
                else
                {
                    await CheckDestruction(collision.gameObject);
                }
            }
        }

        private async UniTask CheckDestruction(GameObject collisionGameObject)
        {
            await destructionService.CheckDestruction(collisionGameObject, projectile);
        }
    }
}
