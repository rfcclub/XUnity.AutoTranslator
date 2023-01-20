using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XUnity.AutoTranslator.Plugin.Core.Endpoints;
using XUnity.Common.Logging;

namespace UVPTranslator
{
   public class UVPTranslatorEndpoint : ITranslateEndpoint
   {
      public string Id => "UVPTranslator";

      public string FriendlyName => "UVPTranslator by 3D2T";

      public int MaxConcurrency => 5;

      public int MaxTranslationsPerRequest => 20;

      private bool initialized = false;

      public void Initialize( IInitializationContext context )
      {
         TranslateCenter.AssemblyDirectory = context.TranslatorDirectory;
         if( !initialized )
         {
            TranslateCenter.Init();
            initialized = true;
         }
         //TransDict.Initialize(context.TranslatorDirectory);
      }

      public IEnumerator Translate( ITranslationContext context )
      {
         TranslateCall translateCall = new TranslateCall( context.UntranslatedTexts );
         translateCall.Run();
         XuaLogger.Common.Info( $"UVP input: {context.UntranslatedTexts.Length} lines" );
         var iterator = translateCall.GetSupportedEnumerator();
         while( iterator.MoveNext() ) yield return iterator.Current;
         XuaLogger.Common.Info( $"UVP translated: {translateCall.GetResult().Length} lines" );
         context.Complete( translateCall.GetResult() );
      }
   }
}
