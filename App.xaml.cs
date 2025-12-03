using LibVLCSharp.Shared;
using Models;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WpfMediaPlayerRA {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public LViewGlobal listViewG { get; set; }
        protected override void OnExit(ExitEventArgs e) {
            // Utiliser les données transmises par MainWindow
            listViewG.supprimerFichierLocal();
            base.OnExit(e);
        }

        protected override async void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var splash = new SplashWindow();
            splash.Show();
            listViewG = new LViewGlobal();


            var mainWindow = new MainWindow();
            // Charger en arrière-plan sans bloquer l'UI
            await Task.Run(() =>
            {
                // Simuler un chargement lourd
      //           Thread.Sleep(8000);
                mainWindow.gererArguments();
//                mainWindow.initListView();
                mainWindow.initVLC();
 //               mainWindow.initListView();
            });

            mainWindow.Show();
            splash.timer_stop();
            splash.Close();
            
        }
    }

    public class LViewGlobal {
        public ListView listView { get; set; }
        public MediaPlayer _mediaPlayer { get; set; }

        internal void supprimerFichierLocal() {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
            //_libVLC.Dispose(); //TODO ce serait plus propre de disposer?

            foreach (var item in listView.Items) {
                var playListItem = item as PlayListItem;
                if (playListItem != null) {
                    if (File.Exists(playListItem.FullName)) {
                        SupprimerFichierQuandLibre(playListItem.FullName);
                    }
                }
            }
        }

        public static void SupprimerFichierQuandLibre(string chemin, int timeoutMs = 10000) {
            var start = DateTime.Now;

            while (true) {
                try {
                    if (File.Exists(chemin)) {
                        File.Delete(chemin);
                        Console.WriteLine("Fichier supprimé !");
                        break;
                    }
                    else {
                        Console.WriteLine("Fichier introuvable.");
                        break;
                    }
                }
                catch (IOException) {
                    // Fichier verrouillé, on attend
                    if ((DateTime.Now - start).TotalMilliseconds > timeoutMs) {
                        Console.WriteLine("Timeout : impossible de supprimer le fichier.");
                        break;
                    }
                    Thread.Sleep(500); // Attendre 0,5 seconde avant de réessayer
                }
            }
        }
    }
}
