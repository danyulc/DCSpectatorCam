using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Kinemotik;
using System;
using System.IO;
using UnityEngine;

namespace DCSpectatorCam
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            CameraLogger.Instance.Logger = Logger;
            Harmony.CreateAndPatchAll(typeof(ToggleFirstPersonPatch));
            Harmony.CreateAndPatchAll(typeof(UpdatePatch));
            CameraLogger.Instance.Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }

    public sealed class CameraLogger
    {
        private static readonly Lazy<CameraLogger> _instance = new Lazy<CameraLogger>(() => new CameraLogger());
        public static CameraLogger Instance { get { return _instance.Value; } }
        public BepInEx.Logging.ManualLogSource Logger { get; set; }
    }

    public sealed class CameraConfig
    {
        private static readonly Lazy<CameraConfig> _instance = new Lazy<CameraConfig>(() => new CameraConfig());
        private ConfigEntry<float> _cameraFOV;
        private ConfigEntry<Vector3> _cameraPosition;
        private ConfigEntry<Vector3> _focalPointPosition;
        private ConfigFile config;

        public static CameraConfig Instance { get { return _instance.Value; } }

        public float CameraFOV
        {
            get { return _cameraFOV.Value; }
            set { _cameraFOV.Value = value; }
        }

        public Vector3 CameraPosition
        {
            get { return _cameraPosition.Value; }
            set { _cameraPosition.Value = value; }
        }

        public Vector3 FocalPointPosition
        {
            get { return _focalPointPosition.Value; }
            set { _focalPointPosition.Value = value; }
        }

        public static bool IsEditingActive { get; set; } = false;

        private CameraConfig()
        {
            this.config = new ConfigFile(Path.Combine(Paths.ConfigPath, "DCSpectatorCam.cfg"), true);
            this._cameraFOV = this.config.Bind("Camera",
                "FOV", 82.1f,
                "The vertical field of view of the spectator camera.");
            this._cameraPosition = this.config.Bind("Camera",
                "Position", new Vector3(1.0f, 2.25f, -2.0f),
                "The position of the spectator camera.");
            this._focalPointPosition = this.config.Bind("FocalPoint",
                "Position", new Vector3(0.0f, 1.0f, 1.5f),
                "The position of the spectator cameras focal point.");
        }
    }

    [HarmonyPatch(typeof(SpectatorCameraManager), "ToggleFirstPerson")]
    public class ToggleFirstPersonPatch
    {
        static void Prefix(ref Camera ___specCam, ref Transform ___thirdPersonTrans)
        {
            // Disable the gamepad controls and set values from config.
            ___thirdPersonTrans.GetChild(0).GetComponent<OrbitByJoystick>().enabled = false;
            ___thirdPersonTrans.GetChild(1).GetComponent<OffsetByJoystick>().enabled = false;
            ___thirdPersonTrans.GetChild(0).position = CameraConfig.Instance.CameraPosition;
            ___thirdPersonTrans.GetChild(1).position = CameraConfig.Instance.FocalPointPosition;
            ___specCam.fieldOfView = CameraConfig.Instance.CameraFOV;
        }
    }

    [HarmonyPatch(typeof(SpectatorCameraManager), "Update")]
    public class UpdatePatch
    {
        static void Postfix(ref Transform ___thirdPersonTrans)
        {
            if (KeyboardManager.GetKeyDown(KeyCode.E))
            {
                CameraConfig.IsEditingActive = !CameraConfig.IsEditingActive;
                if (CameraConfig.IsEditingActive)
                {
                    CameraLogger.Instance.Logger.LogInfo("Camera editing is active");
                }
                else
                {
                    CameraConfig.Instance.CameraPosition = ___thirdPersonTrans.GetChild(0).position;
                    CameraConfig.Instance.FocalPointPosition = ___thirdPersonTrans.GetChild(1).position;
                    CameraLogger.Instance.Logger.LogInfo("Camera editing is deactive");
                }
            }
            if (CameraConfig.IsEditingActive)
            {
                var rightHand = GameObject.Find("RightHand");
                TrackedXRController trackedRightHand = rightHand.GetComponent<TrackedXRController>();
                if (trackedRightHand.GetTriggerClicked())
                {
                    ___thirdPersonTrans.GetChild(0).position = rightHand.transform.position;
                }

                var leftHand = GameObject.Find("LeftHand");
                TrackedXRController trackedLeftHand = leftHand.GetComponent<TrackedXRController>();
                if (trackedLeftHand.GetTriggerClicked())
                {
                    ___thirdPersonTrans.GetChild(1).position = leftHand.transform.position;
                }
            }
        }
    }
}
