#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Iam.Common.Contracts;
using JetBrains.Annotations;

#endregion

// ReSharper disable RedundantAssignment

namespace Iam.Web.Services
{
    /// <summary>
    ///     CSS and JS files bundler for Identity Server HTML templates.
    ///     Bundles for development will be rendered on debug mode <see cref="ConditionalAttribute" />.
    /// </summary>
    [UsedImplicitly]
    public class Bundle : IBundle
    {
        private bool _debug;

        public Bundle()
        {
            SetDebugFlag();
        }

        public string RenderCss(string html)
        {
            return html.Replace("[css-bundle]", BundleCss());
        }

        public string RenderJs(string html)
        {
            return html.Replace("[js-bundle]", BundleJs());
        }

        [Conditional("DEBUG")]
        private void SetDebugFlag()
        {
            _debug = true;
        }

        private string BundleCss()
        {
            var bundles = new List<string>();

            if (_debug)
            {
                bundles.Add("<link href='/content/styles.min.css' rel='stylesheet' />");
                bundles.Add("<link href='/content/site.css' rel='stylesheet'>");
            }
            else
            {
                bundles.Add("<link href='/content/all.min.css' rel='stylesheet'>");
            }

            return string.Join(Environment.NewLine, bundles);
        }

        private string BundleJs()
        {
            var bundles = new List<string>();

            if (_debug)
            {
                bundles.Add("<script src='/scripts/scripts.min.js'></script>");
                bundles.Add("<script src='/scripts/idsapp.min.js'></script>");
            }
            else
            {
                bundles.Add("<script src='/scripts/scripts.min.js'></script>");
                bundles.Add("<script src='/scripts/idsapp.min.js'></script>");
            }

            return string.Join(Environment.NewLine, bundles);
        }
    }
}