using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IllusionInjector
{
    public class PluginComponent : MonoBehaviour
    {
        private CompositeBSPlugin bsPlugins;
        private CompositeIPAPlugin ipaPlugins;
        private bool quitting = false;

        public static PluginComponent Create()
        {
            return new GameObject("IPA_PluginManager").AddComponent<PluginComponent>();
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            bsPlugins = new CompositeBSPlugin(PluginManager.BSPlugins);
            ipaPlugins = new CompositeIPAPlugin(PluginManager.Plugins);

            // this has no relevance since there is a new mod updater system
            //gameObject.AddComponent<ModUpdater>(); // AFTER plugins are loaded, but before most things
            gameObject.AddComponent<Updating.ModsaberML.Updater>();
            gameObject.AddComponent<IllusionPlugin.Utils.BSSceneManager>();
            gameObject.AddComponent<IllusionPlugin.BSUI.BeatSaberUI>();
            gameObject.AddComponent<IllusionPlugin.BSUI.SettingsUI.SettingsUI>();

            bsPlugins.OnApplicationStart();
            ipaPlugins.OnApplicationStart();
            
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            var Harmony = HarmonyInstance.Create("com.cirr.beatsaber.modmanager");

            var originalOptionsAreTurnOnText = typeof(GameOptionsTextsHelper).GetMethod("OptionsAreTurnOnText", BindingFlags.Public | BindingFlags.Static);
            var modifiedOptionsAreTurnOnText = typeof(PluginComponent).GetMethod(nameof(ModifiedOptionsAreTurnOnText), BindingFlags.Public | BindingFlags.Static);
            Harmony.Patch(originalOptionsAreTurnOnText, null, null, new HarmonyMethod(modifiedOptionsAreTurnOnText));

            var validForScoreUse = typeof(GameplayOptions).GetMethod("validForScoreUse", BindingFlags.Public | BindingFlags.Instance);
            var validForScoreUsePost = typeof(PluginComponent).GetMethod(nameof(ValidForScoreUsePost), BindingFlags.Public | BindingFlags.Static);
            Harmony.Patch(validForScoreUse, null, new HarmonyMethod(validForScoreUsePost));
        }

        public static bool ValidForScoreUsePost(bool __result)
        {
            var allowScoreUse = __result;

            foreach (var plugin in PluginManager.BSPlugins)
            {
                allowScoreUse &= plugin.ValidForScoreUse;
            }

            return allowScoreUse;
        }

        public static string ModifiedOptionsAreTurnOnText(bool noEnergy, bool noObstacles)
        {
            string text = "";

            if (noEnergy)
            {
                if (!string.IsNullOrEmpty(text)) { text += " ,"; };
                text += "<color=#00AAFF>NO FAIL</color>";
            }
            if (noObstacles)
            {
                if (!string.IsNullOrEmpty(text)) { text += " ,"; };
                text += "<color=#00AAFF>NO OBSTACLES</color>";
            }

            foreach (var plugin in PluginManager.BSPlugins)
            {
                if (!plugin.ValidForScoreUse)
                {
                    if (!string.IsNullOrEmpty(text)) { text += " ,"; };
                    text += "<color=#00AAFF>" + plugin.NoScoreReason + "</color>";
                }
            }

            text += " options are turned ON.";
            return text;
        }

        void Update()
        {
            bsPlugins.OnUpdate();
            ipaPlugins.OnUpdate();
        }

        void LateUpdate()
        {
            bsPlugins.OnLateUpdate();
            ipaPlugins.OnLateUpdate();
        }

        void FixedUpdate()
        {
            bsPlugins.OnFixedUpdate();
            ipaPlugins.OnFixedUpdate();
        }

        void OnDestroy()
        {
            if (!quitting)
            {
                Create();
            }
        }
        
        void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            
            bsPlugins.OnApplicationQuit();
            ipaPlugins.OnApplicationQuit();

            quitting = true;
        }

        void OnLevelWasLoaded(int level)
        {
            ipaPlugins.OnLevelWasLoaded(level);
        }

        public void OnLevelWasInitialized(int level)
        {
            ipaPlugins.OnLevelWasInitialized(level);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            bsPlugins.OnSceneLoaded(scene, sceneMode);
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            bsPlugins.OnSceneUnloaded(scene);
        }

        private void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            bsPlugins.OnActiveSceneChanged(prevScene, nextScene);
        }
    }
}
