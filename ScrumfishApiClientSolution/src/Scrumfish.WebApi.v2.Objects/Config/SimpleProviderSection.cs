using System.Configuration;

namespace Scrumfish.WebApi.v2.Objects.Config
{
    public class SimpleProviderSection : ConfigurationSection
    {
        public static SimpleProviderSection GetProviderSection()
        {
            return
                (SimpleProviderSection)
                    System.Configuration.ConfigurationManager.GetSection("scrumfish/simpleProvider");
        }

        [ConfigurationProperty("users")]
        [ConfigurationCollection(typeof(ConfigUserCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ConfigUserCollection Users
        {
            get { return (ConfigUserCollection)this["users"]; }
            set { this["users"] = value; }
        }
    }

    public class ConfigUserCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigUserElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigUserElement) element).UserName;
        }
    }

    public class ConfigUserElement : ConfigurationElement
    {
        [ConfigurationProperty("userName", IsKey = true)]
        public string UserName
        {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("certificate")]
        public string Certificate
        {
            get { return (string)this["certificate"]; }
            set { this["certificate"] = value; }
        }

        [ConfigurationProperty("thumbprint")]
        public string Thumbprint
        {
            get { return (string)this["thumbprint"]; }
            set { this["thumbprint"] = value; }
        }

        [ConfigurationProperty("claims")]
        [ConfigurationCollection(typeof(ClaimCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ClaimCollection Claims
        {
            get { return (ClaimCollection)this["claims"]; }
            set { this["claims"] = value; }
        }
    }

    public class ClaimCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ClaimElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClaimElement)element).Key;
        }
    }

    public class ClaimElement : ConfigUserElement
    {
        [ConfigurationProperty("key", IsKey = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("claim")]
        public string Claim
        {
            get { return (string) this["claim"]; }
            set { this["claim"] = value; }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string) this["value"]; }
            set { this["value"] = value; }
        }
    }
}
