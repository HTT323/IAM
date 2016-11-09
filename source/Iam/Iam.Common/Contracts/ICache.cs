#region

using System;

#endregion

namespace Iam.Common.Contracts
{
    public interface ICache
    {
        object Get(string key);
        void Put(string key, object value);
        void Put(string key, object value, TimeSpan timeout);
        void Remove(string key);
        void Clear();
    }
}