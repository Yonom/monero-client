﻿using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Jojatekok.MoneroGUI.Windows
{
    public partial class AboutWindow
    {
        private static readonly string ThirdPartyLicensesPath = StaticObjects.ApplicationDirectory + "Licenses";

        private static string LicenseText { get; set; }

        public AboutWindow()
        {
            Icon = StaticObjects.ApplicationIcon;

            InitializeComponent();

            TextBlockVersion.Text = "v" + StaticObjects.ApplicationVersionString;
            CheckThirdPartyLicensesAvailability();

            if (LicenseText == null) {
                Task.Factory.StartNew(LoadLicenseText);
            } else {
                TextBoxLicenseText.Text = LicenseText;
            }
        }

        public AboutWindow(Window owner) : this()
        {
            Owner = owner;
        }

        private void LoadLicenseText()
        {
            var licenseFiles = Directory.GetFiles(StaticObjects.ApplicationDirectory, "LICENSE*", SearchOption.TopDirectoryOnly);

            if (licenseFiles.Length != 0) {
                using (var stream = new StreamReader(licenseFiles[0])) {
                    LicenseText = stream.ReadToEnd();
                    LicenseText = LicenseText.ReWrap();
                }

                Dispatcher.Invoke(() => TextBoxLicenseText.Text = LicenseText);

            } else {
                Dispatcher.Invoke(() => TextBoxLicenseText.Text = Properties.Resources.AboutWindowLicenseFileNotFound);
            }
        }

        private bool CheckThirdPartyLicensesAvailability()
        {
            var output = Directory.Exists(ThirdPartyLicensesPath);
            ButtonThirdPartyLicenses.IsEnabled = output;
            return output;
        }

        private void ButtonThirdPartyLicenses_Click(object sender, RoutedEventArgs e)
        {
            if (CheckThirdPartyLicensesAvailability()) {
                Process.Start(ThirdPartyLicensesPath);
            }
        }
    }
}
