#region

using System;
using Iam.Common.Contracts;
using JetBrains.Annotations;

#endregion

namespace Iam.Identity.Tenant
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class WsFedProtocolMapping : IModel
    {
        public Guid WsFedMappingProtocolId { get; set; }

        public string Realm { get; set; }
        public string TenantId { get; set; }
        public string Caption { get; set; }

        public Guid ObjectId => WsFedMappingProtocolId;
    }
}