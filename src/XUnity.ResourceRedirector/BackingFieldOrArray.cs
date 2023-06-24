using System.Collections.Generic;
using UnityEngine;
#if IL2CPP
using UnhollowerBaseLib;
#elif IL2CPPINTEROP
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#endif

namespace XUnity.ResourceRedirector
{
   internal struct BackingFieldOrArray
   {
      private UnityEngine.Object _field;
#if IL2CPP
      private UnhollowerBaseLib.Il2CppReferenceArray<UnityEngine.Object> _array;
#elif IL2CPPINTEROP
      private Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object> _array;
#else
      private UnityEngine.Object[] _array;
#endif
      private BackingSource _source;

      public BackingFieldOrArray( UnityEngine.Object field )
      {
         _field = field;
         _array = null;
         _source = BackingSource.SingleField;
      }

#if IL2CPP
      public BackingFieldOrArray( UnhollowerBaseLib.Il2CppReferenceArray<UnityEngine.Object> array )
#elif IL2CPPINTEROP
      public BackingFieldOrArray( Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object> array )
#else
      public BackingFieldOrArray( UnityEngine.Object[] array )
#endif
      {
         _field = null;
         _array = array;
         _source = BackingSource.Array;
      }

      public UnityEngine.Object Field
      {
         get
         {
            if( _source == BackingSource.None )
            {
               return null;
            }
            else if( _source == BackingSource.SingleField )
            {
               return _field;
            }
            else
            {
               return _array != null && _array.Length > 0 ? _array[ 0 ] : null;
            }
         }
         set
         {
            _field = value;
            _array = null;
            _source = BackingSource.SingleField;
         }
      }

#if IL2CPP
      public UnhollowerBaseLib.Il2CppReferenceArray<UnityEngine.Object> Array
#elif IL2CPPINTEROP
      public Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object> Array
#else
      public UnityEngine.Object[] Array
#endif
      {
         get
         {
            if( _source == BackingSource.Array )
            {
               return _array;
            }
            else
            {
               // create an empty array if None is correct
#if IL2CPP
               _array = new Il2CppReferenceArray<UnityEngine.Object>(0);
               if (_field != null) {
                  _array = new Il2CppReferenceArray<UnityEngine.Object>(1);
                  _array[0] = _field;
               }
#elif IL2CPPINTEROP
               _array = new Il2CppReferenceArray<UnityEngine.Object>(0);
               if (_field != null) {
                  _array = new Il2CppReferenceArray<UnityEngine.Object>(1);
                  _array[0] = _field;
               }
#else
               if( _field == null )
               {
                  _array = new UnityEngine.Object[ 0 ];
               }
               else
               {
                  _array = new UnityEngine.Object[ 1 ] { _field };
               }
#endif
               return _array;
            }
         }
         set
         {
            _field = null;
            _array = value;
            _source = BackingSource.Array;
         }
      }

      public IEnumerable<UnityEngine.Object> IterateObjects()
      {
         if( _array != null )
         {
            foreach( var obj in _array )
            {
               yield return obj;
            }
         }
         else if( _field != null )
         {
            yield return _field;
         }
      }
   }
}
