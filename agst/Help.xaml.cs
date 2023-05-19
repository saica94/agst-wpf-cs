using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;

namespace agst
{
    /// <summary>
    /// Help.xaml の相互作用ロジック
    /// </summary>
    public partial class Help : MetroWindow
    {
        public Help()
        {
            InitializeComponent();
        }

        private void btn_Twitter_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                UseShellExecute = true,
                Arguments = "/c start https://twitter.com/daikiy6111"
            });
        }

        private void btn_github_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                UseShellExecute = true,
                Arguments = "/c start https://github.com/saica94/"
            });
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}