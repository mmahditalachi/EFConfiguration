using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EFConfiguration
{
    public class EFConfigurationProvider : ConfigurationProvider
    {
        private readonly EFConfigurationSource _source;

        public EFConfigurationProvider(EFConfigurationSource source)
        {
            _source = source;

            if (_source.ReloadOnChange)
                EntityChangeObserver.Instance.Changed += EntityChangeObserver_Changed;
        }

        private void EntityChangeObserver_Changed(object sender, EntityChangedEntryEventArgs e)
        {
            if (e.Entry.Entity.GetType() != typeof(ConfigurationSetting))
                return;

            Thread.Sleep(_source.ReloadDelay);

            Load();
        }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<EFConfigurationContext>();
            _source.OptionsAction(builder);

            using (var context = new EFConfigurationContext(builder.Options))
            {
                context.Database.EnsureCreated();

                Data = context.ConfigurationSettings.Any()
                    ? context.ConfigurationSettings.ToDictionary(c => c.Key, c => c.Value)
                    : CreateAndSaveDefaultValues(context).Result;
            }
        }

        private Task<Dictionary<string, string>> CreateAndSaveDefaultValues(
        EFConfigurationContext dbContext)
        {
            var startupConfiguration = GetDataFromConfigurationCenter().Result;

            dbContext.ConfigurationSettings.AddRange(startupConfiguration);
            dbContext.SaveChanges();

            return Task.FromResult(startupConfiguration.ToDictionary(e => e.Key, p => p.Value));
        }

        public async Task<List<ConfigurationSetting>> GetDataFromConfigurationCenter()
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(_source.API_URL);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new ConfigurationNotFoundException($"configuration not found brandId");

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ConfigurationSetting>>(content);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
