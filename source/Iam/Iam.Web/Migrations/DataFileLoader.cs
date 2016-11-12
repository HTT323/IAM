#region

using System;
using System.IO;
using IdentityServer3.EntityFramework.Serialization;
using Newtonsoft.Json;

#endregion

namespace Iam.Web.Migrations
{
    public class DataFileLoader
    {
        public static T[] Load<T>(string file)
        {
            var empty = new T[0];

            if (!Path.IsPathRooted(file))
            {
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Seed", file);
            }

            file = Path.GetFullPath(file);

            if (!File.Exists(file)) return empty;

            var json = File.ReadAllText(file);

            return string.IsNullOrWhiteSpace(json)
                ? empty
                : JsonConvert.DeserializeObject<T[]>(json, new ClaimConverter());
        }
    }
}