using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using ExIni;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Reflection;
using Il2CppSystem.Runtime.Remoting.Activation;
using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using XUnity.AutoTranslator.Plugin.Core;
using XUnity.AutoTranslator.Plugin.Core.Configuration;
using XUnity.AutoTranslator.Plugin.Core.Constants;
using XUnity.AutoTranslator.Plugin.Core.Web;
using XUnity.Common.Constants;
using XUnity.Common.Logging;

namespace XUnity.AutoTranslator.Plugin.BepInEx6_IL2CPP;

[BepInPlugin( MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION )]
public class AutoTranslatorPlugin : BasePlugin, IPluginEnvironment
{
      private IniFile _file;
      private string _configPath;
      private string[] essentialFiles = new string[] {
         "UnityEngine.dll",
         "UnityEngine.CoreModule.dll",
         "UnityEngine.TextRenderingModule.dll",
         "UnityEngine.TextCoreTextEngineModule.dll",
         "UnityEngine.TextCoreFontEngineModule.dll",
         "Unity.TextMeshPro.dll",
      };
      public AutoTranslatorPlugin()
      {
         ConfigPath = Paths.ConfigPath;
         TranslationPath = Paths.BepInExRootPath;
         _configPath = Path.Combine( ConfigPath, "AutoTranslatorConfig.ini" );

         Il2CppProxyAssemblies.Location = Path.Combine( Paths.BepInExRootPath, "interop" );
         XuaLogger.AutoTranslator.Info( $"{Il2CppProxyAssemblies.Location}" );
      }

      public override void Load()
      {
         XuaLogger.AutoTranslator.Info( "Load all assemblies" );
         LoadUnityEssential();
         XuaLogger.AutoTranslator.Info( "Load all assemblies done !!!" );
         PluginLoader.LoadWithConfig( this );
      }

      private void LoadUnityEssential()
      {
         foreach ( var file in essentialFiles )
         {
            string filePath = Path.Combine( Il2CppProxyAssemblies.Location, file );
            try
            {
               XuaLogger.AutoTranslator.Info( $"Loading {filePath}" );
               if (File.Exists(filePath))
               {
                  System.Reflection.Assembly.Load( filePath );
               }
            }
            catch(Exception ex )
            {
               XuaLogger.AutoTranslator.Warn(ex.Message);
            }
         }
      }

      public IniFile Preferences
      {
         get
         {
            return ( _file ?? ( _file = ReloadConfig() ) );
         }
      }

      public string ConfigPath { get; }

      public string TranslationPath { get; }

      public bool AllowDefaultInitializeHarmonyDetourBridge => false;

      public IniFile ReloadConfig()
      {
         if( !File.Exists( _configPath ) )
         {
            return ( _file ?? new IniFile() );
         }
         IniFile ini = IniFile.FromFile( _configPath );
         if( _file == null )
         {
            return ( _file = ini );
         }
         _file.Merge( ini );
         return _file;
      }

      public void SaveConfig()
      {
         _file.Save( _configPath );
      }
}
