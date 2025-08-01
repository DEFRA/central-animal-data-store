using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CAS.Test.Infrastructure;

public sealed class CasApiFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder b) =>
        b.UseEnvironment("Testing"); 
}