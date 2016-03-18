using System;
using System.Collections.Generic;
using UnityEngine;
using ImportantUtilities;
using CareerManager;

namespace CareerManagerUI
{
    public enum CareerOptions
    {
        LOCKFUNDS,
        LOCKSCIENCE,
        UNLOCKTECH,
        UNLOCKBUILDINGS
    }

    public class CareerManagerUI : MonoBehaviour
    {
        private bool guiActive = false;
        public bool useAppLauncher = true;
        private IButton toolbarButton = null;
        private ApplicationLauncherButton appLauncherButton = null;

        private int windowID;
        private Rect optionsWindowRect;

        private Dictionary<CareerOptions, MenuToggle> options;

        public bool GuiActive
        {
            get
            {
                return guiActive;
            }
            set
            {
                guiActive = value;
            }
        }

        public Dictionary<CareerOptions, MenuToggle> Options
        {
            get
            {
                return options;
            }
        }

        public void ToggleGui(bool show)
        {
            GuiActive = show;
        }

        public CareerManagerUI()
        {
            options = new Dictionary<CareerOptions, MenuToggle>();

            // Check if the toolbar is available
            if (ToolbarManager.ToolbarAvailable)
            {
                useAppLauncher = false; // force using the toolbar if it exists!

                toolbarButton = ToolbarManager.Instance.add("CareerManager", "careermanagerbutton");
                toolbarButton.TexturePath = "CareerManager/icons/careermanager_stock";
                toolbarButton.ToolTip = "CareerManager options";
                toolbarButton.OnClick += (e) =>
                {
                    ToggleGui(!guiActive);
                };
            }
            else
            {
                if (ApplicationLauncher.Ready && appLauncherButton == null)
                {
                    appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                        () => { ToggleGui(true); },
                        () => { ToggleGui(false); },
                        () => { }, // DoNothing! :)
                        () => { },
                        () => { },
                        () => { },
                        ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.TRACKSTATION,
                        (Texture)GameDatabase.Instance.GetTexture("CareerManager/icons/careermanager_stock", false));
                }
            }



            windowID = Guid.NewGuid().GetHashCode();

            optionsWindowRect = new Rect(200, 175, 200, 25);
        }


        /* Creates a toggle button. By default only visible in the space center. */
        public void CreateToggle(CareerOptions opt, Rect rect, bool defaultstate, string description, Action<bool> cback)
        {
            options.Add(
                opt,
                new MenuToggle(rect, defaultstate, description, cback)
                );
        }

        /* Override: Creates a toggle button that works in specific scenes */
        public void CreateToggle(CareerOptions opt, Rect rect, bool defaultstate, string description, Action<bool> cback, GameScenes[] scenes)
        {
            options.Add(
                opt,
                new MenuToggle(rect, defaultstate, description, cback, scenes)
                );
        }

        public bool GetOption(CareerOptions opt)
        {
            if (options.ContainsKey(opt))
            {
                return options[opt].GetState();
            }

            return false;
        }

        public void SetOption(CareerOptions opt, bool value)
        {
            if (options.ContainsKey(opt))
            {
                options[opt].state = value;
            }
        }

        public void LoadSettings(ConfigNode node)
        {

            node.GetConfigValue(out options[CareerOptions.LOCKFUNDS]._state, "LockFunds");
            node.GetConfigValue(out options[CareerOptions.LOCKSCIENCE]._state, "LockScience");
            node.GetConfigValue(out options[CareerOptions.UNLOCKBUILDINGS]._state, "UnlockBuildings");
            node.GetConfigValue(out options[CareerOptions.UNLOCKTECH]._state, "UnlockTech");
        }

        public void SaveSettings(ConfigNode node)
        {
            node.AddValue("LockFunds", GetOption(CareerOptions.LOCKFUNDS).ToString());
            node.AddValue("LockScience", GetOption(CareerOptions.LOCKSCIENCE).ToString());
            node.AddValue("UnlockBuildings", GetOption(CareerOptions.UNLOCKBUILDINGS).ToString());
            node.AddValue("UnlockTech", GetOption(CareerOptions.UNLOCKTECH).ToString());
        }

        public void DrawGUI()
        {
            if (GuiActive)
            {
                GUI.skin = HighLogic.Skin;
                optionsWindowRect = GUILayout.Window(windowID, optionsWindowRect, Draw, "CareerManager Options", GUI.skin.window);
            }
        }

        public void Draw(int windowID)
        {

            GUILayout.BeginVertical();
            foreach (KeyValuePair<CareerOptions, MenuToggle> toggle in options)
            {
                toggle.Value.draw(HighLogic.LoadedScene);
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public void OnDisable()
        {
            if (ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving scene
            {
                toolbarButton.Destroy();
            }
            else
            {
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
            }
        }

    }

    /* The MenuToggle class was built on the MIT Licensed code of Technicalfool's mod HeatWarning
	 * https://github.com/Technicalfool/HeatWarning/
	 */
    public class MenuToggle
    {
        private Rect size;
        private string description;
        public bool _state;
        private Action<bool> callback;
        private GameScenes[] _scenes;

        public MenuToggle(Rect sizeRect, bool defaultState, string desc, Action<bool> cback, GameScenes[] scenes)
        {
            initialize(sizeRect, defaultState, desc, cback, scenes);
        }

        public MenuToggle(Rect sizeRect, bool defaultState, string desc, Action<bool> cback)
        {
            initialize(sizeRect, defaultState, desc, cback, new[] { GameScenes.SPACECENTER });
        }

        private void initialize(Rect sizeRect, bool defaultState, string desc, Action<bool> cback, GameScenes[] scenes)
        {
            size = new Rect(sizeRect);
            _state = defaultState;
            description = desc;
            callback = cback;
            _scenes = scenes;
        }

        public bool state
        {
            get { return _state; }
            set { _state = value; }
        }

        public string getDescription
        {
            get { return description; }
        }

        public Rect getSize
        {
            get { return size; }
        }

        public bool GetState()
        {
            return _state;
        }


        // this must be called in OnGUI
        public void draw(GameScenes scene)
        {

            if (Array.FindIndex(_scenes, sc => sc == scene) > -1)
            {
                bool oldState = _state;
                _state = GUILayout.Toggle(_state, description, GUILayout.ExpandWidth(true));
                if (_state != oldState)
                {
                    callback(_state);
                }
            }
        }
    }

}

