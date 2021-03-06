﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ApiPaths = Jojatekok.MoneroAPI.Paths;

namespace Jojatekok.MoneroGUI
{
    static class SettingsManager
    {
        private const string RelativePathFileUserConfiguration = "user.config";

        private static Configuration Configuration { get; set; }

        private static bool _isAutoSaveEnabled = true;
        public static bool IsAutoSaveEnabled {
            get { return _isAutoSaveEnabled; }
            set { _isAutoSaveEnabled = value; }
        }

        public static ConfigSectionGeneral General { get; private set; }
        public static ConfigSectionPaths Paths { get; private set; }
        public static ConfigSectionAppearance Appearance { get; private set; }
        public static ConfigSectionAddressBook AddressBook { get; private set; }

        static SettingsManager()
        {
            InitializeConfiguration();

            // Settings are ready to be used from here
            StaticObjects.Initialize();
        }

        private static void InitializeConfiguration()
        {
            // Directory: %LocalAppData%\[Company]\[AssemblyName]\
            var configurationPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Helper.GetAssemblyAttribute<AssemblyCompanyAttribute>().Company,
                StaticObjects.ApplicationAssemblyName.Name,
                RelativePathFileUserConfiguration
            );

            var configurationFileMap = new ExeConfigurationFileMap {
                ExeConfigFilename = configurationPath,
                LocalUserConfigFilename = configurationPath,
                RoamingUserConfigFilename = configurationPath,
            };

            Configuration = ConfigurationManager.OpenMappedExeConfiguration(configurationFileMap, ConfigurationUserLevel.PerUserRoamingAndLocal);

            // Don't include unnecessary information within ConfigurationSections' type declarations
            Configuration.TypeStringTransformer = delegate(string input) {
                var inputSplit = input.Split(',');
                return inputSplit[0] + "," + inputSplit[1];
            };

            LoadOrCreateSections();
        }

        private static void LoadOrCreateSections()
        {
            var isSaveRequired = false;

            General = Configuration.GetSection("general") as ConfigSectionGeneral;
            if (General == null) {
                isSaveRequired = true;
                General = new ConfigSectionGeneral();
                Configuration.Sections.Add("general", General);
            }

            Paths = Configuration.GetSection("paths") as ConfigSectionPaths;
            if (Paths == null) {
                isSaveRequired = true;
                Paths = new ConfigSectionPaths();
                Configuration.Sections.Add("paths", Paths);
            }

            Appearance = Configuration.GetSection("appearance") as ConfigSectionAppearance;
            if (Appearance == null) {
                isSaveRequired = true;
                Appearance = new ConfigSectionAppearance();
                Configuration.Sections.Add("appearance", Appearance);
            }

            AddressBook = Configuration.GetSection("addressBook") as ConfigSectionAddressBook;
            if (AddressBook == null) {
                isSaveRequired = true;
                AddressBook = new ConfigSectionAddressBook();
                Configuration.Sections.Add("addressBook", AddressBook);
            }

            if (isSaveRequired) SaveSettings();
        }

        public static void SaveSettings()
        {
            Configuration.Save(ConfigurationSaveMode.Modified);
            InitializeConfiguration();
        }

        private static void AutoSaveSettings()
        {
            if (IsAutoSaveEnabled) SaveSettings();
        }

        public class ConfigSectionGeneral : ConfigurationSection
        {
            public ConfigSectionGeneral()
            {
                SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
            }

            [ConfigurationProperty("isStartableOnSystemLogin", DefaultValue = false)]
            public bool IsStartableOnSystemLogin {
                get { return (bool)base["isStartableOnSystemLogin"]; }
                set {
                    base["isStartableOnSystemLogin"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("isSafeShutdownEnabled", DefaultValue = true)]
            public bool IsSafeShutdownEnabled {
                get { return (bool)base["isSafeShutdownEnabled"]; }
                set {
                    base["isSafeShutdownEnabled"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("walletDefaultPassword", DefaultValue = "x")]
            public string WalletDefaultPassword {
                get { return base["walletDefaultPassword"] as string; }
                set {
                    base["walletDefaultPassword"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("transactionsDefaultMixCount", DefaultValue = 0)]
            [IntegerValidator(MinValue = 0)]
            public int TransactionsDefaultMixCount {
                get { return (int)base["transactionsDefaultMixCount"]; }
                set {
                    base["transactionsDefaultMixCount"] = value;
                    AutoSaveSettings();
                }
            }
        }

        public class ConfigSectionPaths : ConfigurationSection
        {
            public ConfigSectionPaths()
            {
                SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
            }

            [ConfigurationProperty("directoryWalletBackups", DefaultValue = ApiPaths.DefaultDirectoryWalletBackups)]
            public string DirectoryWalletBackups {
                get { return base["directoryWalletBackups"] as string; }
                set {
                    base["directoryWalletBackups"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("fileWalletData", DefaultValue = ApiPaths.DefaultFileWalletData)]
            public string FileWalletData {
                get { return base["fileWalletData"] as string; }
                set {
                    base["fileWalletData"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareDaemon", DefaultValue = ApiPaths.DefaultSoftwareDaemon)]
            public string SoftwareDaemon {
                get { return base["softwareDaemon"] as string; }
                set {
                    base["softwareDaemon"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareWallet", DefaultValue = ApiPaths.DefaultSoftwareWallet)]
            public string SoftwareWallet {
                get { return base["softwareWallet"] as string; }
                set {
                    base["softwareWallet"] = value;
                    AutoSaveSettings();
                }
            }

            [ConfigurationProperty("softwareMiner", DefaultValue = ApiPaths.DefaultSoftwareMiner)]
            public string SoftwareMiner {
                get { return base["softwareMiner"] as string; }
                set {
                    base["softwareMiner"] = value;
                    AutoSaveSettings();
                }
            }
        }

        public class ConfigSectionAppearance : ConfigurationSection
        {
            public ConfigSectionAppearance()
            {
                SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
            }

            [ConfigurationProperty("languageCode", DefaultValue = Helper.DefaultLanguageCode)]
            public string LanguageCode {
                get { return base["languageCode"] as string; }
                set {
                    base["languageCode"] = value;
                    AutoSaveSettings();
                }
            }
        }

        public class ConfigSectionAddressBook : ConfigurationSection
        {
            public ConfigSectionAddressBook()
            {
                SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
            }

            [ConfigurationProperty("elements", IsDefaultCollection = true)]
            [ConfigurationCollection(typeof(ConfigElementCollectionContact))]
            public ConfigElementCollectionContact Elements {
                get { return (ConfigElementCollectionContact)base["elements"]; }
            }
        }

        public class ConfigElementContact : ConfigurationElement
        {
            [ConfigurationProperty("label", IsRequired = true)]
            public string Label {
                get { return base["label"] as string; }
                set { base["label"] = value; }
            }

            [ConfigurationProperty("address", IsRequired = true)]
            public string Address {
                get { return base["address"] as string; }
                set { base["address"] = value; }
            }

            public ConfigElementContact(string label, string address)
            {
                Label = label;
                Address = address;
            }
        }

        public class ConfigElementCollectionContact : ConfigurationElementCollection, IEnumerable<ConfigElementContact>
        {
            public override ConfigurationElementCollectionType CollectionType {
                get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
            }

            public ConfigElementContact this[int index] {
                get { return BaseGet(index) as ConfigElementContact; }

                set {
                    IsAutoSaveEnabled = false;

                    if (BaseGet(index) != null) BaseRemoveAt(index);
                    BaseAdd(index, value);

                    IsAutoSaveEnabled = true;
                    SaveSettings();
                }
            }

            new public ConfigElementContact this[string label] {
                get { return BaseGet(label) as ConfigElementContact; }
            }

            protected override sealed ConfigurationElement CreateNewElement()
            {
                return new ConfigElementContact(string.Empty, string.Empty);
            }

            public void Add(ConfigElementContact element)
            {
                BaseAdd(element, false);
                AutoSaveSettings();
            }

            public void Add(string label, string address)
            {
                Add(new ConfigElementContact(label, address));
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                Debug.Assert(element as ConfigElementContact != null, "element as ConfigElementContact != null");
                return (element as ConfigElementContact).Label;
            }

            public int IndexOf(ConfigElementContact element)
            {
                return BaseIndexOf(element);
            }

            public void Remove(ConfigElementContact element)
            {
                if (BaseIndexOf(element) >= 0) {
                    BaseRemove(element.Label);
                    AutoSaveSettings();
                }
            }

            public void Remove(string name)
            {
                BaseRemove(name);
                AutoSaveSettings();
            }

            public void RemoveAt(int index)
            {
                BaseRemoveAt(index);
                AutoSaveSettings();
            }

            public void Clear()
            {
                BaseClear();
                AutoSaveSettings();
            }

            public new IEnumerator<ConfigElementContact> GetEnumerator()
            {
                for (var i = Count - 1; i >= 0; i--) {
                    yield return this[i];
                }
            }
        }
    }
}
