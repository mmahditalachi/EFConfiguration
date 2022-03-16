using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace EFConfiguration
{
    public class EFConfigurationSource : IConfigurationSource
    {
        public Action<DbContextOptionsBuilder> OptionsAction { get; set; }

        public bool ReloadOnChange { get; set; }

        // Number of milliseconds that reload will wait before calling Load. This helps avoid triggering a reload before a change is completely saved. Default is 500.
        public int ReloadDelay { get; set; } = 500;

        public string API_URL { get; set; }
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EFConfigurationProvider(this);
        }
    }
}
