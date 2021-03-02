using System;
using System.Collections.Generic;
using System.Resources;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Tank
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
