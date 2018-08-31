using IllusionPlugin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace IllusionPlugin.BSUI.SettingsUI
{
    public class SubMenu
    {
        public Transform transform;

        public SubMenu(Transform transform)
        {
            this.transform = transform;
        }

        public BoolViewController AddBool(string name)
        {
            return AddToggleSetting<BoolViewController>(name);
        }

        public ListViewController AddList(string name, float[] values)
        {
            var view = AddListSetting<ListViewController>(name);
            view.SetValues(values);
            return view;
        }

        public T AddListSetting<T>(string name) where T : ListSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            VolumeSettingsController volume = newSettingsObject.GetComponent<VolumeSettingsController>();
            T newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), newSettingsObject, typeof(T));
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            return newListSettingsController;
        }

        public T AddToggleSetting<T>(string name) where T : SwitchSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SwitchSettingsController), newSettingsObject, typeof(T));
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            return newToggleSettingsController;
        }
    }
}
