using Archetypical.Software.Vitruvian.Common;
using System.Threading.Tasks;
using Xunit;

namespace Archetypical.Software.Vitruvian.Resolvers.Consul.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var resolver = new Consul.ConsulMicrositeResolver();
            await resolver.GetAllAsync();
        }

        [Fact]
        public async Task Microsite_Regsitering_Test()
        {
            var resolver = new Consul.ConsulMicrositeResolver();
            var site = new Microsite("Test", "Unit Test Microsite", new System.Version(1, 0), new Endpoint(new System.Uri("http://localhost:5004")), "Unit Test");

            var result = await resolver.RegisterAsync(site);
        }
    }
}