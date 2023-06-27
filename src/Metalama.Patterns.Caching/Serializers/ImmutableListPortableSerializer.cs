// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Serializers
{
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
}
