using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using VRUI;
using VRUIControls;
using TMPro;
using IllusionPlugin;
using HMUI;
using IllusionPlugin.Utils;

namespace IllusionPlugin.BSUI.SettingsUI
{
    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance = null;
        static bool ready = false;
        public static bool Ready
        {
            get => ready;
        }

        static SettingsNavigationController settingsMenu = null;
        static MainSettingsMenuViewController mainSettingsMenu = null;
        static MainSettingsTableView _mainSettingsTableView = null;
        static TableView subMenuTableView = null;
        static TableViewHelper subMenuTableViewHelper;

        static Transform othersSubmenu = null;

        static Button _pageUpButton = null;
        static Button _pageDownButton = null;
        static Vector2 buttonOffset = new Vector2(24, 0);

        public void Awake()
        {
            if (Instance == null)
            {
                Create();
            }
            else
            {
                Destroy(this);
            }
        }

        private void Create()
        {
            Instance = this;
            BSSceneManager.menuLoaded += MenuLoaded;
            DontDestroyOnLoad(gameObject);
        }

        public void MenuLoaded()
        {
            try
            {
                SetupUI();

                //var testSub = CreateSubMenu("Test 1");
                //var testSub2 = CreateSubMenu("Test 2");
                //var testSub3 = CreateSubMenu("Test 3");
                //var testSub4 = CreateSubMenu("Test 4");
                //var testSub5 = CreateSubMenu("Test 5");
                //var testSub6 = CreateSubMenu("Test 6");
            }
            catch (Exception e)
            {
                Console.WriteLine("SettingsUI done fucked up: " + e);
            }
        }

        private static void SetupUI()
        {
            if (mainSettingsMenu == null)
            {
                ready = false;
            }

            if (!Ready)
            {
                try
                {
                    mainSettingsMenu = Resources.FindObjectsOfTypeAll<MainSettingsMenuViewController>().FirstOrDefault();
                    _mainSettingsTableView = mainSettingsMenu.GetPrivateField<MainSettingsTableView>("_mainSettingsTableView");
                    subMenuTableView = _mainSettingsTableView.GetComponentInChildren<TableView>();
                    subMenuTableViewHelper = subMenuTableView.gameObject.AddComponent<TableViewHelper>();

                    settingsMenu = Resources.FindObjectsOfTypeAll<SettingsNavigationController>().FirstOrDefault();
                    othersSubmenu = settingsMenu.transform.Find("Others");

                    //var buttons = settingsMenu.transform.Find("Buttons");
                    //RectTransform okButton = (RectTransform)buttons.Find("OkButton"); //{x: -17, y: 6}
                    //RectTransform CancelButton = (RectTransform)buttons.Find("CancelButton"); // {x: 0, y: 6}
                    //RectTransform ApplyButton = (RectTransform)buttons.Find("ApplyButton"); // {x: 17, y: 6}

                    //okButton.anchoredPosition += buttonOffset;
                    //CancelButton.anchoredPosition += buttonOffset;
                    //ApplyButton.anchoredPosition += buttonOffset;

                    if (_mainSettingsTableView != null)
                    {
                        AddPageButtons();
                    }

                    ready = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Beat Saver UI: Oops - " + e.Message);
                }
            }
        }

        static void AddPageButtons()
        {
            RectTransform viewport = _mainSettingsTableView.GetComponentsInChildren<RectTransform>().First(x => x.name == "Viewport");
            viewport.anchorMin = new Vector2(0f, 0.5f);
            viewport.anchorMax = new Vector2(1f, 0.5f);
            viewport.sizeDelta = new Vector2(0f, 48f);
            viewport.anchoredPosition = new Vector2(0f, 0f);

            if (_pageUpButton == null)
            {
                _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), _mainSettingsTableView.transform, false);
                (_pageUpButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                (_pageUpButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 24f);
                _pageUpButton.interactable = true;
                _pageUpButton.onClick.AddListener(delegate ()
                {
                    subMenuTableViewHelper.PageScrollUp();
                });
            }

            if (_pageDownButton == null)
            {
                _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), _mainSettingsTableView.transform, false);
                (_pageDownButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                (_pageDownButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -24f);
                _pageDownButton.interactable = true;
                _pageDownButton.onClick.AddListener(delegate ()
                {
                    subMenuTableViewHelper.PageScrollDown();
                });
            }

            subMenuTableViewHelper._pageUpButton = _pageUpButton;
            subMenuTableViewHelper._pageDownButton = _pageDownButton;

            //subMenuTableView.SetPrivateField("_pageUpButton", _pageUpButton);
            //subMenuTableView.SetPrivateField("_pageDownButton", _pageDownButton);
        }

        public static void LogComponents(Transform t, string prefix = "=", bool includeScipts = false)
        {
            Console.WriteLine(prefix + ">" + t.name);

            if (includeScipts)
            {
                foreach (var comp in t.GetComponents<MonoBehaviour>())
                {
                    Console.WriteLine(prefix + "-->" + comp.GetType());
                }
            }

            foreach (Transform child in t)
            {
                LogComponents(child, prefix + "=", includeScipts);
            }
        }

        public static SubMenu CreateSubMenu(string name)
        {
            if (!BSSceneManager.isMenuScene(SceneManager.GetActiveScene()))
            {
                Console.WriteLine("Cannot create settings menu when not in the main scene.");
                return null;
            }

            SetupUI();

            var subMenuGameObject = Instantiate(othersSubmenu.gameObject, othersSubmenu.transform.parent);
            Transform mainContainer = CleanScreen(subMenuGameObject.transform);

            var newSubMenuInfo = new SettingsSubMenuInfo();
            newSubMenuInfo.SetPrivateField("_menuName", name);
            newSubMenuInfo.SetPrivateField("_controller", subMenuGameObject.GetComponent<VRUIViewController>());

            var subMenuInfos = mainSettingsMenu.GetPrivateField<SettingsSubMenuInfo[]>("_settingsSubMenuInfos").ToList();
            subMenuInfos.Add(newSubMenuInfo);
            mainSettingsMenu.SetPrivateField("_settingsSubMenuInfos", subMenuInfos.ToArray());

            SubMenu menu = new SubMenu(mainContainer);
            return menu;
        }

        static Transform CleanScreen(Transform screen)
        {
            var container = screen.Find("Content").Find("SettingsContainer");
            var tempList = container.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }
            return container;
        }
    }
}