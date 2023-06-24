﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BepInEx.IL2CPP;
using ExIni;
using UnhollowerRuntimeLib;
using UnityEngine;
using XUnity.AutoTranslator.Plugin.Core;
using XUnity.AutoTranslator.Plugin.Core.Configuration;
using XUnity.AutoTranslator.Plugin.Core.Constants;
using XUnity.Common.Constants;
using XUnity.Common.Logging;

namespace XUnity.AutoTranslator.Plugin.BepInEx
{
   [BepInPlugin( GUID: PluginData.Identifier, Name: PluginData.Name, Version: PluginData.Version )]
   public class AutoTranslatorPlugin : BasePlugin, IPluginEnvironment
   {
      private IniFile _file;
      private string _configPath;

      public AutoTranslatorPlugin()
      {
         ConfigPath = Paths.ConfigPath;
         TranslationPath = Paths.BepInExRootPath;

         _configPath = Path.Combine( ConfigPath, "AutoTranslatorConfig.ini" );

         Il2CppProxyAssemblies.Location = Preloader.IL2CPPUnhollowedPath;
      }

      public override void Load()
      {
         PluginLoader.LoadWithConfig( this );
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
}
