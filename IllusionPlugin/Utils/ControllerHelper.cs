using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllusionPlugin.Utils
{
    public static class ControllerHelper
    {
        public static bool LeftTrigger()
        {
            return VRControllersInputManager.TriggerValue(UnityEngine.XR.XRNode.LeftHand) > 0.75f;
        }

        public static bool RightTrigger()
        {
            return VRControllersInputManager.TriggerValue(UnityEngine.XR.XRNode.RightHand) > 0.75f;
        }

        public static float LeftVerticalAxis()
        {
            return VRControllersInputManager.VerticalAxisValue(UnityEngine.XR.XRNode.LeftHand);
        }

        public static float RightVerticalAxis()
        {
            return VRControllersInputManager.VerticalAxisValue(UnityEngine.XR.XRNode.RightHand);
        }

        public static float LeftHorizontalAxis()
        {
            return VRControllersInputManager.HorizontalAxisValue(UnityEngine.XR.XRNode.LeftHand);
        }

        public static float RightHorizontalAxis()
        {
            return VRControllersInputManager.HorizontalAxisValue(UnityEngine.XR.XRNode.RightHand);
        }

        public static bool MenuButtonDown()
        {
            return VRControllersInputManager.MenuButtonDown();
        }

        public static bool MenuButton()
        {
            return VRControllersInputManager.MenuButton();
        }
    }
}
