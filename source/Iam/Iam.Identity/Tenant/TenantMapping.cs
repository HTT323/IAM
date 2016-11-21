#region

using System;
using Iam.Common.Contracts;
using JetBrains.Annotations;

#endregion

namespace Iam.Identity.Tenant
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class TenantMapping : IModel
    {
        public Guid TenantMappingId { get; set; }

        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string ClientId { get; set; }
        public string Logo { get; set; }

        public Guid ObjectId => TenantMappingId;
    }
}