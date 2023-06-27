// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Serializers;

internal sealed class ImmutableListPortableSerializer<T>
#if !TODO
{ }
#else
        : ReferenceTypeSerializer
    {
        private const string keyName = "_";

        public ImmutableListPortableSerializer()
        {

        }

        /// <exclude/>
        public override object CreateInstance(Type type, IArgumentsReader constructorArguments)
        {
            T[] values = constructorArguments.GetValue<T[]>(keyName);
            return ImmutableList.Create(values);
        }

        /// <exclude/>
        public override void SerializeObject(object obj, IArgumentsWriter constructorArguments, IArgumentsWriter initializationArguments)
        {
            ImmutableList<T> list = (ImmutableList<T>)obj;

            // we need to save arrays in constructorArguments because objects from initializationArguments can be not fully deserialized when DeserializeFields is called
            T[] array = new T[list.Count];
            list.CopyTo(array);
            constructorArguments.SetValue(keyName, array);
        }

        /// <exclude/>
        public override void DeserializeFields(object obj, IArgumentsReader initializationArguments)
        {
        }
    }
#endif