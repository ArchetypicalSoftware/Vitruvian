using System;

namespace Archetypical.Software.Vitruvian.Common
{
    public class Endpoint
    {
        public Endpoint(Uri uri)
        {
            Uri = uri;
        }

        public Uri Uri { get; }
    }
}