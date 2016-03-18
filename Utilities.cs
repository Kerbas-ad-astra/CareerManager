using UnityEngine;

namespace ImportantUtilities
{
    public static class Utilities
    {
        public static void GetConfigValue(this ConfigNode node, out bool outval, string key)
        {
            bool result = bool.TryParse(node.GetValue(key), out outval);
            if (!result)
            {
                outval = false;
                return;
            }
        }

        public static void GetConfigValue(this ConfigNode node, out int outval, string key)
        {
            bool result = int.TryParse(node.GetValue(key), out outval);
            if (false == result)
            {
                outval = 0; // default to 0..
                return;
            }
        }

        public static void GetConfigValue(this ConfigNode node, out float outval, string key)
        {
            bool result = float.TryParse(node.GetValue(key), out outval);
            if (false == result)
            {
                outval = 0; // default to 0..
                return;
            }
        }

        public static void GetConfigValue(this ConfigNode node, out double outval, string key)
        {
            bool result = double.TryParse(node.GetValue(key), out outval);
            if (false == result)
            {
                outval = 0; // default to 0..
                return;
            }
        }

        public static void GetConfigValue(this ConfigNode node, out string outval, string key)
        {
            outval = node.GetValue(key);
        }

        public static void Log(string pluginName, int instanceID, string message)
        {
            Debug.Log("[" + pluginName + "[" + instanceID.ToString("X") + "][" + Time.time.ToString("0.0000") + "]: " + message);
        }
    }
}

