using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace IllusionPlugin.Utils
{
    public class BSSceneManager : MonoBehaviour
    {
        public static BSSceneManager Instance;

        public static Action menuLoaded;
        public static Action gameLoaded;

        public static bool isMenuScene(Scene scene)
        {
            return (scene.name == "Menu");
        }

        public static bool isLoadingScene(Scene scene)
        {
            return (scene.name == "StandardLevelLoader");
        }

        public static bool isGameScene(Scene scene)
        {
            return scene.name.Contains("Environment");
        }

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            DontDestroyOnLoad(gameObject);
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (isMenuScene(scene))
            {
                menuLoaded.Invoke();
                return;
            }

            if (isLoadingScene(scene))
            {
                var asyncLoader = Resources.FindObjectsOfTypeAll<AsyncScenesLoader>().FirstOrDefault();
                if (asyncLoader == null)
                {
                    // The scene was loaded normally
                    GameSceneLoaded();
                }
                else
                {
                    // AsyncScenesLoader is loading the scene, subscribe to the load complete event
                    asyncLoader.loadingDidFinishEvent -= GameSceneLoaded; // make sure we don't subscribe twice
                    asyncLoader.loadingDidFinishEvent += GameSceneLoaded;
                }
                return;
            }
        }

        private void GameSceneLoaded()
        {
            gameLoaded.Invoke();
        }
    }
}
