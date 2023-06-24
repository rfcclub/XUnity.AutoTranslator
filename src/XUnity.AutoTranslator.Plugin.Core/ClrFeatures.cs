﻿using System;
using System.Reflection.Emit;
using XUnity.Common.Constants;

namespace XUnity.AutoTranslator.Plugin.Core
{
   internal static class ClrFeatures
   {
      internal static bool SupportsNet4x { get; } = false;

      internal static bool SupportsReflectionEmit { get; } = false;

      static ClrFeatures()
      {
         try
         {
            SupportsNet4x = ClrTypes.Task != null;
         }
         catch( Exception )
         {

         }

         try
         {
            TestReflectionEmit();

            SupportsReflectionEmit = true;
         }
         catch( Exception )
         {
            SupportsReflectionEmit = false;
         }
      }

      private static void TestReflectionEmit()
      {
#if IL2CPP || MANAGED
         MethodToken t1 = default( MethodToken );
         MethodToken t2 = default( MethodToken );
         
         var ok = t1 == t2;
#endif
#if IL2CPPINTEROP
         // do nothing .NET 6 support System.Reflection.Emit
#endif

      }
   }
}
