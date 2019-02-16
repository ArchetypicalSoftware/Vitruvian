using Archetypical.Software.Vitruvian.Common;
using Archetypical.Software.Vitruvian.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Archetypical.Software.Vitruvian.Microsite
{
    public class MicrositeConfiguration
    {
        public Uri VitruvianGatewayUri { get; set; }

        /// <summary>
        /// This is the route or "SLUG" that will redirect to your microsite
        /// from the Vitruvian Web Gateway.
        /// <see cref="https://en.wikipedia.org/wiki/Clean_URL#Slug"/>
        /// </summary>
        /// <example>my.vitruviangateway.com/Microsite1 .. Microsite1 is your slug</example>
        public string Slug { get; set; }

        /// <summary>
        /// Some administrative name to identify your microsite
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A bucket of tags that can be used for metadata and can be utilized in any overrides of
        /// <see cref="IMicrositeResolver"/> for routing
        /// </summary>
        internal List<string> Tags { get; set; } = new List<string>();

        public void AddTags(params string[] tag)
        {
            Tags.AddRange(tag);
        }

        public Version Version { get; set; }

        internal Endpoint Endpoint { get; set; }

        internal Common.Microsite MicroSite => new Common.Microsite(Slug, Name, Version, Endpoint, Tags.ToArray());
    }
}