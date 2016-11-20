#region

using System;
using Iam.Common.Contracts;
using JetBrains.Annotations;

#endregion

namespace Iam.Identity.Tenant
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WsFedMapping : IModel
    {
        public Guid WsFedMappingId { get; set; }

        public int WsFedId { get; set; }
        public string TenantId { get; set; }
        public string Caption { get; set; }
        public string MetadataAddress { get; set; }
        public string Realm { get; set; }
        public string Audience { get; set; }

        public Guid ObjectId => WsFedMappingId;
    }
}