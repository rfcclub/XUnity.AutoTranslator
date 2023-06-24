using MelonLoader;
using System.IO;
using XUnity.AutoTranslator.Plugin.Core;
using XUnity.AutoTranslator.Plugin.Core.Constants;
using XUnity.AutoTranslator.Plugin.MelonMod6;

[assembly: MelonInfo( typeof( AutoTranslatorPlugin ), PluginData.Name, PluginData.Version, PluginData.Author )]
[assembly: MelonGame( null, null )]
namespace XUnity.AutoTranslator.Plugin.MelonMod6
{
    public class AutoTranslatorPlugin : MelonLoader.MelonMod
    {
      public override void OnApplicationLateStart()
      {
#if IL2CPP
         var modFi = new FileInfo( Location );
         var gameDir = modFi.Directory.Parent;
         var unhollowedPath = Path.Combine( gameDir.FullName, @"MelonLoader\Managed" );
         Il2CppProxyAssemblies.Location = unhollowedPath;
#endif

         PluginLoader.Load( false );
      }
   }
}
