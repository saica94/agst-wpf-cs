using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Windows;

namespace agst
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Option ow;
        private Help hlp;

        // レジストリ登録 //
        private string Mouse_acc_Path = @"HKEY_CURRENT_USER\Control Panel\Mouse";

        private string Mouse_acc_Key1 = "MouseSpeed";
        private string Mouse_acc_Key2 = "MouseThreshold1";
        private string Mouse_acc_Key3 = "MouseThreshold2";

        // タスクバー関連
        private string TaskBar_Path = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

        private string TaskBar_Size_Key1 = "TaskbarSi";
        private string TaskIcon_Position_Key1 = "TaskbarAl";
        private string TaskBar_ClassicMode_Key1 = "Start_ShowClassicMode";
        private string TaskBar_Search_Disabled_Path = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Search";
        private string TaskBar_Search_Disabled_Key1 = "SearchboxTaskbarMode";
        private string TaskBar_Widget_Key1 = "Taskbarda";

        // XBox Game bar 関連
        private string XBox_Game_bar_Path = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR";

        private string XBox_Game_bar_Key1 = "AppCaptureEnabled";

        // 固定キー関連
        private string StickyKeys_Path = @"HKEY_CURRENT_USER\Control Panel\Accessibility\StickyKeys";

        private string StickyKeys_Key1 = "Flags";

        // Windows 広告関連
        private string AdvertisingInfo_Path = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo";

        private string AdvertisingInfo_Key1 = "Enabled";
        private string SyncProviderNotif_Key1 = "ShowSyncProviderNotifications";
        private string ContentDeliver_Path = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";
        private string ContentDeliver_Key1 = "SubscribedContent-338388Enabled";
        private string ContentDeliver_Key2 = "SubscribedContent-310093Enabled";
        private string ContentDeliver_Key3 = "SubscribedContent-338389Enabled";
        private string LockScreenOverlay_Key1 = "RotatingLockScreenOverlayEnabled";
        private string UserProfileEngagement_Path = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\UserProfileEngagement";
        private string UserProfileEngagement_Key1 = "ScoobeSystemSettingEnabled";
        private string Privacy_Path = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy";
        private string Privacy_Key1 = "TailoredExperiencesWithDiagnosticDataEnabled";

        // ウィンドウのスナップ関連
        private string Window_snap_Path = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

        private string Window_snap_key1 = "EnableSnapBar";

        public MainWindow()
        {
            InitializeComponent();
            ow = new Option();
            ow.Visibility = Visibility.Hidden;
        }

        public void Options_Click(object sender, RoutedEventArgs e)
        {
            ow.Owner = this;
            ow.Visibility = Visibility.Visible;
        }

        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            string str;

            if (Chrome.IsChecked == true)
            {
                string path = System.IO.Path.GetTempPath();
                string installer = "chrome_installer.exe";
                string url = "https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26browser%3D0%26usagestats%3D1%26appname%3DGoogle%2520Chrome%26needsadmin%3Dprefers%26brand%3DGTPM/update2/installers/ChromeSetup.exe";

                HttpClient httpClient = new HttpClient();
                // WebClient webClient = new(); <- 旧式だとVSに怒られたので変更
                str = "Google Chromeのダウンロードを行います";
                AppendTextBox(str);
                byte[] data = await httpClient.GetByteArrayAsync(url);
                await using(FileStream stream = new FileStream(System.IO.Path.Combine(path, installer), FileMode.Create))
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
                //webClient.DownloadFile(url, System.IO.Path.Combine(path, installer)); <- 旧式だとVSに怒られたので変更

                str = "Google Chromeのインストールを行います";
                AppendTextBox(str);
                Process process = new();
                process.StartInfo.FileName = System.IO.Path.Combine(path, installer);
                process.StartInfo.Arguments = "/silent /install";
                process.StartInfo.Verb = "runas";
                process.Start();
                process.WaitForExit();

                Output.AppendText("インストーラーの削除を行います");
                System.IO.File.Delete(System.IO.Path.Combine(path, installer));
            }

            if (ow != null)
            {
                // マウス加速の設定
                if (ow.off.IsChecked == true)
                {
                    str = "マウス加速を無効化します。";
                    AppendTextBox(str);
                    Registry_change(Mouse_acc_Path, Mouse_acc_Key1, "0");
                    Registry_change(Mouse_acc_Path, Mouse_acc_Key2, "0");
                    Registry_change(Mouse_acc_Path, Mouse_acc_Key3, "0");
                }
                else
                {
                    str = "マウス加速を有効化します。";
                    AppendTextBox(str);
                    Registry_change(Mouse_acc_Path, Mouse_acc_Key1, "1");
                    Registry_change(Mouse_acc_Path, Mouse_acc_Key2, "1");
                    Registry_change(Mouse_acc_Path, Mouse_acc_Key3, "1");
                }

                // タスクバーサイズの設定
                if (ow.LARGE.IsChecked == true)
                {
                    str = "タスクバーのサイズを\"大\"に変更します。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskBar_Size_Key1, "2");
                }
                else if (ow.MEDIUM.IsChecked == true)
                {
                    str = "タスクバーのサイズを\"中\"に変更します。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskBar_Size_Key1, "1");
                }
                else
                {
                    str = "タスクバーのサイズを\"小\"に変更します。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskBar_Size_Key1, "0");
                }

                // スタートメニュー
                if(ow.start_Win10.IsChecked == true)
                {
                    str = "Windows10仕様のスタートメニューに変更します。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskBar_ClassicMode_Key1, "0");
                }
                else
                {
                    str = "Windows11仕様のスタートメニューに変更します。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskBar_ClassicMode_Key1, "1");
                }

                // XBOX Gamer barの設定
                if (ow.xbox_enable.IsChecked == true)
                {
                    str = "XBOX Gamer barを有効化します。";
                    AppendTextBox(str);
                    Registry_change(XBox_Game_bar_Path, XBox_Game_bar_Key1, "1");
                }
                else
                {
                    str = "XBOX Gamer barを無効化します。";
                    AppendTextBox(str);
                    Registry_change(XBox_Game_bar_Path, XBox_Game_bar_Key1, "0");
                }

                // Windows 広告の設定
                if (ow.Ads_enable.IsChecked == true)
                {
                    str = "Windows 広告を有効にします。";
                    AppendTextBox(str);
                    Registry_change(AdvertisingInfo_Path, AdvertisingInfo_Key1, "1");
                    Registry_change(AdvertisingInfo_Path, SyncProviderNotif_Key1, "1");
                    Registry_change(ContentDeliver_Path, ContentDeliver_Key1, "1");
                    Registry_change(ContentDeliver_Path, ContentDeliver_Key2, "1");
                    Registry_change(ContentDeliver_Path, ContentDeliver_Key3, "1");
                    Registry_change(ContentDeliver_Path, LockScreenOverlay_Key1, "1");
                    Registry_change(UserProfileEngagement_Path, UserProfileEngagement_Key1, "1");
                    Registry_change(Privacy_Path, Privacy_Key1, "1");
                }
                else
                {
                    str = "Windows 広告を無効にします。";
                    AppendTextBox(str);
                    Registry_change(AdvertisingInfo_Path, AdvertisingInfo_Key1, "0");
                    Registry_change(AdvertisingInfo_Path, SyncProviderNotif_Key1, "0");
                    Registry_change(ContentDeliver_Path, ContentDeliver_Key1, "0");
                    Registry_change(ContentDeliver_Path, ContentDeliver_Key2, "0");
                    Registry_change(ContentDeliver_Path, ContentDeliver_Key3, "0");
                    Registry_change(ContentDeliver_Path, LockScreenOverlay_Key1, "0");
                    Registry_change(UserProfileEngagement_Path, UserProfileEngagement_Key1, "0");
                    Registry_change(Privacy_Path, Privacy_Key1, "0");
                }

                // 検索アイコンの設定
                if (ow.search_show.IsChecked == true)
                {
                    str = "タスクバーの検索アイコンを表示します。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Search_Disabled_Path, TaskBar_Search_Disabled_Key1, "1");
                }
                else
                {
                    str = "タスクバーの検索アイコンを非表示にします。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Search_Disabled_Path, TaskBar_Search_Disabled_Key1, "0");
                }

                // ウィジェットアイコンの設定
                if (ow.widget_show.IsChecked == true)
                {
                    str = "タスクバーのウィジェットアイコンを表示します。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskBar_Widget_Key1, "1");
                }
                else
                {
                    str = "タスクバーのウィジェットアイコンを非表示にします。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskBar_Widget_Key1, "0");
                }

                // ウィンドウスナップ機能の設定
                if (ow.window_snap_enable.IsChecked == true)
                {
                    str = "ウィンドウスナップの一部機能を有効化します。";
                    AppendTextBox(str);
                    Registry_change(Window_snap_Path, Window_snap_key1, "1");
                }
                else
                {
                    str = "ウィンドウスナップの一部機能を無効化します。";
                    AppendTextBox(str);
                    Registry_change(Window_snap_Path, Window_snap_key1, "0");
                }

                // コンテキストメニューの設定
                if (ow.context_win10.IsChecked == true)
                {
                    str = "コンテキストメニューをWindows10仕様にします。";
                    AppendTextBox(str);
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", "", "");
                }
                else
                {
                    str = "コンテキストメニューをWindows11仕様にします。";
                    AppendTextBox(str);
                    bool contextReg = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", null, null) != null;
                    if (contextReg)
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
                    }
                }

                // エクスプローラの再起動
                str = "一部の設定を適応させるため、Explorer.exeを再起動します。";
                AppendTextBox(str);
                // プロセスIDの取得
                Process[] processes = Process.GetProcessesByName("explorer");
                int explorerProcessId = processes.Length > 0 ? processes[0].Id : -1;

                // explorer.exeを停止する
                if (explorerProcessId > -1)
                {
                    Process explorerProcess = Process.GetProcessById(explorerProcessId);
                    explorerProcess.Kill();
                    explorerProcess.WaitForExit();
                }
                // explorer.exeを起動する
                Process.Start("explorer.exe");

                // タスクアイコンの設定
                if (ow.task_left.IsChecked == true)
                {
                    str = "タスクアイコンを左揃えにします。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskIcon_Position_Key1, "0");
                }
                else
                {
                    str = "タスクアイコンを中央揃えにします。";
                    AppendTextBox(str);
                    Registry_change(TaskBar_Path, TaskIcon_Position_Key1, "1");
                }

                if(ow.static_enable.IsChecked == true)
                {
                    str = "固定キー機能を有効化します。";
                    AppendTextBox(str);
                    Registry_change(StickyKeys_Path, StickyKeys_Key1, "1");
                }
                else
                {
                    str = "固定キー機能を無効化します。";
                    AppendTextBox(str);
                    Registry_change(StickyKeys_Path, StickyKeys_Key1, "0");
                }

                // 電源プランの変更
                if (ow.pwr_on.IsChecked == true)
                {
                    str = "電源プランを変更します。\r\n一部のRyzen CPUではこの処理はスキップされます。";
                    AppendTextBox(str);
                    string cpuName = "";
                    string[] ryzenModels = { "AMD Ryzen 9 5950X", "AMD Ryzen 9 5900X", "AMD Ryzen 7 5800X3D",
                         "AMD Ryzen 7 5800X", "AMD Ryzen 7 5700X", "AMD Ryzen 7 5700G",
                         "AMD Ryzen 5 5600X", "AMD Ryzen 5 5600G", "AMD Ryzen 5 5600",
                         "AMD Ryzen 5 5500" };
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            cpuName = obj["Name"].ToString();
                            break;
                        }
                    }
                    if (ryzenModels.Any(model => cpuName.Contains(model)))
                    {
                        System.Diagnostics.Process.Start("powercfg", "/hibernate off");
                        System.Diagnostics.Process.Start("powercfg", "-setacvalueindex scheme_balanced sub_buttons pbuttonaction 3");
                    }
                    else
                    {
                        // 電源プランを高パフォーマンスに変更する
                        System.Diagnostics.Process.Start("powercfg", "/setactive scheme_min");
                        System.Diagnostics.Process.Start("powercfg", "/hibernate off");
                        System.Diagnostics.Process.Start("powercfg", "-setacvalueindex scheme_min sub_buttons pbuttonaction 3");
                    }
                }
                str = "全ての処理が完了しました。\r\nこのウィンドウを閉じても構いません。\r\nなお、一部の設定を正しく反映させるためにPCの再起動を行ってください。";
                AppendTextBox(str);
            }
        }

        private void AppendTextBox(string str)
        {
            str = str + "\r\n";
            Output.AppendText(str);
        }

        private void Registry_change(string path_name, string key_name, string value)
        {
            Microsoft.Win32.Registry.SetValue(path_name, key_name, value, Microsoft.Win32.RegistryValueKind.DWord);
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            hlp = new Help();
            hlp.Owner = this;
            hlp.ShowDialog();
            // hlp.Visibility = Visibility.Visible;
        }

        protected virtual void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GC.Collect();
        }
    }

    public partial class Option : MetroWindow
    {
        public Option()
        {
            InitializeComponent();
        }
    }
}