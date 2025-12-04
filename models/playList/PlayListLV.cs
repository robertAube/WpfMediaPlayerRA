using MyUtil;
using Models;
using System.IO;
using System.Windows.Controls;
using WpfMediaPlayerRA.UtilWpf;
using static WpfMediaPlayerRA.AppConfig;

namespace WpfMediaPlayerRA.models.playList {
    class PlayListLV {
        private ListView listView;
        Slider sliderStart, sliderEnd;

        public PlayListLV(ListView listView, string path, Slider sliderStart, Slider sliderEnd) {
            this.listView = listView;
            this.sliderStart = sliderStart;
            this.sliderEnd = sliderEnd;

            if (MainWindow.AppConfig.VideoSource == SourceVideo.FromExcel) {
                chargerMediaFromExcel();
            }
            else {
                chargerMediaFromDir(path);
            }
        }

        internal void deleteTempFile() {
            foreach (var item in listView.Items) {
                var playListItem = listView.SelectedItem as PlayListItem;
                if (playListItem != null) {
                    if (File.Exists(playListItem.FullName)) {
                        File.Delete(playListItem.FullName);
                    }
                }
            }
        }

        private void chargerMediaFromExcel() {
            List<string> listePath = new List<string>();
            List<string> listeNom = new List<string>();

            try {
                listeNom = FichierExcel.lireColonneExcel(MainWindow.AppConfig.ExcelMediaListPath, 1, 1);
                listePath = FichierExcel.lireColonneExcel(MainWindow.AppConfig.ExcelMediaListPath, 1, 2);
            }
            catch (Exception ex) {
                var playListItem = new PlayListItem("", "erreur fichier excel", MainWindow.AppConfig.DefaultVideoFullPath, new SliderStartEnd(sliderStart, sliderEnd));
                listView.Items.Add(playListItem);
            }
            int i = 0;
            foreach (string nom in listeNom) {
                var playListItem = new PlayListItem("", nom, listePath[i++], new SliderStartEnd(sliderStart, sliderEnd));
                listView.Items.Add(playListItem);
            }

            //videoInfos.Add(getVideo("question 1", @"..\..\!fichiers\butiner.mp4"));
            //videoInfos.Add(getVideo("question 2", @"..\..\!fichiers\M30-1356.mp4"));
            //videoInfos.Add(getVideo("formatif", @"..\..\!fichiers\DémoFormatif11.mkv"));
        }


        private void chargerMediaFromDir(string folderPath) {
            if (Directory.Exists(folderPath)) {

                // Common video file extensions
                string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" };

                // Get all files and filter by extension
                var videoFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                                          .Where(file => videoExtensions.Contains(Path.GetExtension(file).ToLower()))
                                          .ToList();


                foreach (var fullPath in videoFiles) {
                    var playListItem = new PlayListItem("", Path.GetFileNameWithoutExtension(fullPath), fullPath, new SliderStartEnd(sliderStart, sliderEnd));
                    listView.Items.Add(playListItem);
                }
            }
        }

        internal void select(int index) {
            if (listView.Items.Count > 0 && index < listView.Items.Count) { 
                listView.SelectedIndex = index;
            }
        }
    }
}
