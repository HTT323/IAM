#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;
using IdentityServer3.Core.ViewModels;
using JetBrains.Annotations;
using Microsoft.Security.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#endregion

namespace Iam.Web.Services
{
    /// <summary>
    ///     IdentityServer3.Samples /source/CustomViewService/CustomViewService/CustomViewService.cs
    /// </summary>
    [UsedImplicitly]
    public class CustomViewService : IViewService
    {
        private readonly IClientStore _clientStore;

        public CustomViewService(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        public async Task<Stream> Login(LoginViewModel model, SignInMessage message)
        {
            var client = await _clientStore.FindClientByIdAsync(message.ClientId);
            var name = client?.ClientName;

            return await Render(model, "login", name);
        }

        public Task<Stream> Logout(LogoutViewModel model, SignOutMessage message)
        {
            return Render(model, "logout");
        }

        public Task<Stream> LoggedOut(LoggedOutViewModel model, SignOutMessage message)
        {
            return Render(model, "loggedOut");
        }

        public Task<Stream> Consent(ConsentViewModel model, ValidatedAuthorizeRequest authorizeRequest)
        {
            return Render(model, "consent");
        }

        public Task<Stream> ClientPermissions(ClientPermissionsViewModel model)
        {
            return Render(model, "permissions");
        }

        public Task<Stream> Error(ErrorViewModel model)
        {
            return Render(model, "error");
        }

        private static Task<Stream> Render(CommonViewModel model, string page, string clientName = null)
        {
            var json =
                JsonConvert.SerializeObject(
                    model,
                    Formatting.None,
                    new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            var html = LoadHtml(page);

            html = Replace(html, new
            {
                siteName = Encoder.HtmlEncode(model.SiteName),
                model = Encoder.HtmlEncode(json),
                clientName
            });

            return Task.FromResult(StringToStream(html));
        }

        private static string LoadHtml(string name)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"content\app");

            file = Path.Combine(file, name + ".html");

            return File.ReadAllText(file);
        }

        private static string Replace(string value, IDictionary<string, object> values)
        {
            foreach (var key in values.Keys)
            {
                var val = values[key];

                val = val ?? string.Empty;
                value = value.Replace("{" + key + "}", val.ToString());
            }

            return value;
        }

        private static string Replace(string value, object values)
        {
            return Replace(value, Map(values));
        }

        private static IDictionary<string, object> Map(object values)
        {
            var dictionary = values as IDictionary<string, object>;

            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                dictionary.Add(descriptor.Name, descriptor.GetValue(values));
            }

            return dictionary;
        }

        private static Stream StringToStream(string s)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            sw.Write(s);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}