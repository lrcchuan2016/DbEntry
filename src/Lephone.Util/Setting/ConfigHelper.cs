using System.Collections.Specialized;

namespace Lephone.Util.Setting
{
	public class ConfigHelper : ConfigHelperBase
	{
        public static readonly ConfigHelper AppSettings = new ConfigHelper("appSettings");
        public static readonly ConfigHelper DefaultSettings = new ConfigHelper("Lephone.Settings");

        internal ConfigHelper()
        {
            AppSettings.nvc = new ConfigHelper("appSettings").nvc;
            DefaultSettings.nvc = new ConfigHelper("Lephone.Settings").nvc;
        }

        private NameValueCollection nvc;

		public ConfigHelper(string SectionName)
		{
            nvc = ConfigReaderProxy.Instance.GetSection(SectionName);
        }

		protected override string GetString(string key)
		{
			return nvc[key];
		}
	}
}