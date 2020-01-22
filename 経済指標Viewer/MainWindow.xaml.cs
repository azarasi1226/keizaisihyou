using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Model.EconomicIndicators;

namespace 経済指標Viewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void UpdateIEsAsync(object sender, RoutedEventArgs e)
        {
            // ダウンロードする際の下準備
            this.Update_Btn.IsEnabled = false;
            this.Bar.Visibility = Visibility.Visible;
            this.ShowInfo_Dg.ItemsSource = null;
            this.TaskbarInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;

            // 経済指標のダウンロード（非同期処理）
            var ies = await EIManager.GetEIAsync();
            
            // ダウンロード後の後片付け
            this.ShowInfo_Dg.ItemsSource = ies;
            this.Update_Btn.IsEnabled = true;
            this.Bar.Visibility = Visibility.Hidden;
            this.TaskbarInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
        }
    }
}
