using System;
using System.Collections.Generic;

namespace Archetypical.Software.Vitruvian
{
    public class MicroSite
    {
        public MicroSite(string slug, string name, Version version, Endpoint endpoint, params string[] tags)
        {
            Slug = slug;
            Name = name;
            Version = version;
            Endpoints = new List<Endpoint> { endpoint };
            Tags = tags;
        }

        public string Slug { get; }

        public string Name { get; }

        public Version Version { get; }

        public IEnumerable<string> Tags { get; }

        public IEnumerable<Endpoint> Endpoints { get; }
    }
}