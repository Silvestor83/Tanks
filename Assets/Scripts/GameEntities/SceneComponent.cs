using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Infrastructure.Enums;

namespace Assets.Scripts.GameEntities
{
    public class SceneComponent
    {
        public List<SceneComponent> DependentScenes { get; }
        public SceneName Name { get; }
        public bool LoadingScreenNeeded { get; }
        public bool Loaded { get; set; }
        
        public SceneComponent(SceneName sceneName, bool loadingScreen = false)
        {
            Name = sceneName;
            LoadingScreenNeeded = loadingScreen;
            DependentScenes = new List<SceneComponent>();
        }

        public void Add(SceneComponent scene)
        {
            DependentScenes.Add(scene);
        }

        public void Remove(SceneComponent scene)
        {
            DependentScenes.Remove(scene);
        }
    }
}
