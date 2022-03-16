using System.ComponentModel.DataAnnotations;

namespace EFConfiguration
{
    public class ConfigurationSetting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
