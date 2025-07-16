using Microsoft.AspNetCore.Builder;

namespace CentralAnimalDataStore.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   { 
       var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
       var isDev = CentralAnimalDataStore.Config.Environment.IsDevMode(builder);
       Assert.False(isDev);
   }
}
