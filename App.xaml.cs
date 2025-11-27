using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace WpfMediaPlayerRA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ListView listView { get; set; }

        protected override void OnExit(ExitEventArgs e) {
            // Utiliser les données transmises par MainWindow

            base.OnExit(e);
        }


    }

}
