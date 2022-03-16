using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace EFConfiguration
{
    public static class EntityFrameworkExtensions
    {
        public static IConfigurationBuilder AddEFConfiguration(this IConfigurationBuilder builder, bool reloadOnChange, [NotNull] string API_URL, string inMemoryDatabaseName)
        {
            return builder.Add(new EFConfigurationSource
            {
                OptionsAction = o => o.UseInMemoryDatabase(inMemoryDatabaseName),
                ReloadOnChange = reloadOnChange,
                API_URL = API_URL
            });
        }
    }
}
