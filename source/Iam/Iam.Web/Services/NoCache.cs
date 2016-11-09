#region

using System;
using Iam.Common.Contracts;
using JetBrains.Annotations;

#endregion

namespace Iam.Web.Services
{
    [UsedImplicitly]
    public class NoCache : ICache
    {
        public object Get(string key)
        {
            return null;
        }

        public void Put(string key, object value)
        {
        }

        public void Put(string key, object value, TimeSpan timeout)
        {
        }

        public void Remove(string key)
        {
        }

        public void Clear()
        {
        }
    }
}