using MyUtil;
using System.IO;
using static WpfMediaPlayerRA.AppConfig;

namespace WpfMediaPlayerRA.models.playList {
    public class ListeVideo {
        private List<VideoInfo> _videoInfo;

        internal List<VideoInfo> VideoInfo { get => _videoInfo; }

        public ListeVideo(SourceVideo sourceVideo, string path = "") {
            _videoInfo = new List<VideoInfo>();

            DirectoryCleaner.DeleteAllFiles(MainWindow.AppConfig.RepTempLocal);
            if (sourceVideo == SourceVideo.FromExcel) {
                chargerMediaFromExcel();
            }
            else {
                chargerMediaFromDir(path);
            }
        }
        private void chargerMediaFromExcel() {
            List<string> listePath = new List<string>();
            List<string> listeNom = new List<string>();

            try {
                listeNom = FichierExcel.lireColonneExcel(MainWindow.AppConfig.ExcelMediaListPath, 1, 1);
                listePath = FichierExcel.lireColonneExcel(MainWindow.AppConfig.ExcelMediaListPath, 1, 2);
                int i = 0;
                foreach (string nom in listeNom) {
                    var videoInfo = new VideoInfo(nom, listePath[i++]);
                    _videoInfo.Add(setFichierLocal(videoInfo));
                }
            }
            catch (Exception ex) {
                var videoInfo = new VideoInfo("erreur fichier excel", MainWindow.AppConfig.DefaultVideoFullPath);
                _videoInfo.Add(setFichierLocal(videoInfo));

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
                    var videoInfo = new VideoInfo(Path.GetFileNameWithoutExtension(fullPath), fullPath);
                    _videoInfo.Add(setFichierLocal(videoInfo));
                }
            }
        }

        private VideoInfo setFichierLocal(VideoInfo videoInfo) {
            string randomName = Path.GetRandomFileName();
            string destinationFullPath;
            byte[] mediaData;

            //Path.GetTempPath() donne le dossier temporaire du système (ex. C:\Users\robert.aube\AppData\Local\Temp)
            //       do { //s'Assurer de créer un nom de fichier inexistant
            destinationFullPath = Path.Combine(MainWindow.AppConfig.RepTempLocal, randomName); // + Path.GetExtension(path));
                                                                                               //     } while (!File.Exists(destinationFullPath));

            //tester si fichier origine absent
            //path = "kdsl;fks;lkf;alk";

            if (File.Exists(videoInfo.VideoFullPath)) {
                mediaData = File.ReadAllBytes(videoInfo.VideoFullPath);
            }
            else {
                mediaData = File.ReadAllBytes(MainWindow.AppConfig.DefaultVideoFullPath);
                videoInfo.VideoName = "fichier origine absent";

            }
            File.WriteAllBytes(destinationFullPath, mediaData);

            return videoInfo;
        }

        internal IEnumerable<VideoInfo> getListe() {
            throw new NotImplementedException();
        }
    }
}
