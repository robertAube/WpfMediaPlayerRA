using MyUtil;
using System.IO;

namespace WpfMediaPlayerRA {
    public class AppConfig {
        public enum SourceVideo {
            FromExcel = 0,
            FromPath = 1,
        }

        public readonly string baseDir = AppDomain.CurrentDomain.BaseDirectory; // pointe vers bin\Debug ou bin\Release

        //Pointe vers un fichier Excel qui contient la liste de fichiers médias à lire par le lecteur (Celui-ci doit être fermé pour que le lecteur fonctionne)
        private string _excelMediaListPath = @"";

        private readonly string _defaultVideoName = @"remove-tool.mp4";
        private string _DefaultExcelMediaListPath;
        //private string _defaultVideoFullPath = @".\butiner.mp4"; //vidéo qui joue par défaut si le média n'existe pas. 

        //    private SourceVideo _videoSource = SourceVideo.FromPath; //sourceVideo : d'où vient les path des fichiers lus
        private SourceVideo _videoSource = SourceVideo.FromExcel; //sourceVideo : d'où vient les path des fichiers lus

        //private string _hiddenDirName = @"2ikljst!mbhoeujy4a1iqv"; //TODO _hiddenDirName _ répertoire pour mieux cacher les fichiers média sur l'ordinateur local
        private string repTempLocal = Path.Combine(Path.GetTempPath(), @"ZXD767B.tmp");

        public AppConfig() {
            if (!Directory.Exists(repTempLocal)) {
                Directory.CreateDirectory(repTempLocal);
            }
        }

        public string DefaultExcelMediaListPath {
            get {
                if (File.Exists(AppConfigLocal.EXCEL_MEDIA_LIST))
                    _DefaultExcelMediaListPath = AppConfigLocal.EXCEL_MEDIA_LIST;
                else
                    _DefaultExcelMediaListPath = Path.Combine(baseDir, "Assets", AppConfigLocal.DEFAULT_EXCEL_NAME);  // OnPropertyChanged(); }
                return _DefaultExcelMediaListPath;
            }
        }

        public string ExcelMediaListPath {
            get {
                    return _excelMediaListPath;
            }
            set {
                _excelMediaListPath = value;
            }
        }



        public string DefaultVideoFullPath {
            get => Path.Combine(baseDir, "Assets", "Videos", _defaultVideoName);
        }

        public SourceVideo VideoSource {
            get => _videoSource;
            set {
                _videoSource = value; //OnPropertyChanged(); 
            }
        }

        public string RepTempLocal { get => repTempLocal; }
    }

}
