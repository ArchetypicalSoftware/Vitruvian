using System;
using System.Collections.Generic;
using System.Linq;

namespace Archetypical.Software.Vitruvian.Common
{
    public class Microsite
    {
        internal Microsite()
        {
        }

        public Microsite(string slug, string name, Version version, Endpoint endpoint, params string[] tags)
        {
            Slug = slug;
            Name = name;
            Version = version;
            Endpoints = new List<Endpoint> { endpoint };
            Tags = tags.ToList();
        }

        public string Slug { get; }

        public string Name { get; }

        public Version Version { get; }

        public List<string> Tags { get; }

        public List<Endpoint> Endpoints { get; }
    }
}