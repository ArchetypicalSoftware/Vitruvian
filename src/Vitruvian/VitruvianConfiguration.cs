namespace Archetypical.Software.Vitruvian
{
    public class VitruvianConfiguration
    {
        public void RegisterResolver(IMicroSiteResolverProvider resolver)
        {
            Resolver = resolver;
        }

        internal IMicroSiteResolverProvider Resolver { get; set; }
    }
}