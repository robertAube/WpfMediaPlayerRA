using LibVLCSharp.Shared;
using Models;
using MyUtil;
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
            //listViewG.supprimerFichierLocal();
            //AppConfig.FileScanFullPath;
            //MainWindow.AppConfig.
            DirectoryCleaner.DeleteAllFiles(Path.Combine(Path.GetTempPath(), AppConfig.DIR_TEMP_NAME));
            base.OnExit(e);
        }

        protected override async void OnStartup(StartupEventArgs e) {

            base.OnStartup(e);

            var splash = new SplashWindow();
            splash.Show();

            var mainWindow = new MainWindow();

            // Lancer l'init asynchrone sans bloquer l'UI
            await mainWindow.InitializeAsync();

            //mainWindow.init();
            //TODO            DirectoryCleaner.DeleteAllFiles(MainWindow.AppConfig.RepTempLocal, false);
            //attendre la fin du Init
            while (!mainWindow.Ready) {
                Thread.Sleep(500);
            }
            mainWindow.initView();
            mainWindow.Show(); // rendre l'UI
            splash.timer_stop();
            splash.Close();
            mainWindow.premierDepart();
        }

        //protected override void OnStartupOld(StartupEventArgs e) {
        //    base.OnStartup(e);

        //    var splash = new SplashWindow();
        //    splash.Show();
        //    listViewG = new LViewGlobal();

        ////    Thread.Sleep(6000);
        //    // peut être long
        //    var mainWindow = new MainWindow(); //tous les inits sont fais dont charger VLC, les vidéos etc...

        //    mainWindow.Show();

        //    splash.timer_stop();
        //    splash.Close();

        //}

        public class LViewGlobal {
//            public ListView listView { get; set; }
            public MediaPlayer _mediaPlayer { get; set; }
            public string pathLocal { get; set; }

            internal void supprimerFichierLocal() {
                _mediaPlayer.Stop();
                _mediaPlayer.Dispose();
                //_libVLC.Dispose(); //TODO ce serait plus propre de disposer?

                //foreach (var item in listView.Items) {
                //    var playListItem = item as PlayListItem;
                //    if (playListItem != null) {
                //        if (File.Exists(playListItem.FullName)) {
                //            SupprimerFichierQuandLibre(playListItem.FullName);
                //        }
                //    }
                //}

                DirectoryCleaner.DeleteAllFiles(pathLocal, false);
            }

            private void supprimerFichiersRepertoireLocal(string cheminDossier) {
                if (Directory.Exists(cheminDossier)) {
                    string[] fichiers = Directory.GetFiles(cheminDossier);

                    foreach (string fichier in fichiers) {
                        File.Delete(fichier);
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
