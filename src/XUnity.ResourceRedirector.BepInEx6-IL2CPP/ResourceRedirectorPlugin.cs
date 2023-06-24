using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using System;

namespace XUnity.ResourceRedirector.BepInEx6_IL2CPP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ResourceRedirectorPlugin : BasePlugin
{
   public static ConfigEntry<bool> LogAllLoadedResources { get; set; }
   public static ConfigEntry<bool> LogCallbackOrder { get; set; }

   public override void Load()
   {
      
      LogAllLoadedResources = Config.Bind( new ConfigDefinition( "Diagnostics", "Log all loaded resources" ), false );
      ResourceRedirection.LogAllLoadedResources = LogAllLoadedResources.Value;
      LogAllLoadedResources.SettingChanged += ( s, e ) => ResourceRedirection.LogAllLoadedResources = LogAllLoadedResources.Value;

      LogCallbackOrder = Config.Bind( new ConfigDefinition( "Diagnostics", "Log callback order" ), false );
      ResourceRedirection.LogCallbackOrder = LogCallbackOrder.Value;
      LogCallbackOrder.SettingChanged += ( s, e ) => ResourceRedirection.LogCallbackOrder = LogCallbackOrder.Value;

      Config.ConfigReloaded += Config_ConfigReloaded;
   }

   private static void Config_ConfigReloaded( object sender, EventArgs e )
   {
      ResourceRedirection.LogAllLoadedResources = LogAllLoadedResources.Value;
      ResourceRedirection.LogCallbackOrder = LogCallbackOrder.Value;
   }
}
