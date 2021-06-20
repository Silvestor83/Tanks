using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class ExplosionController : MonoBehaviour
    {
        private ParticleSystem particle;

        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            particle.Emit(50);
        }
    }
}
