using MyUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirzaMediaPlayer {
    public class AppConfig {
        public enum SourceVideo {
            FromExcel = 0,
            FromPath = 1,
        }

        private string _excelMediaListPath = @"Q:\zulu\Nouveau dossier\A134V3.xlsm";  //liste de fichiers média dans un fichier excel
        private string _defaultVideoFullPath = @".\butiner.mp4"; //vidéo qui joue par défaut si le média n'existe pas. 
    //    private SourceVideo _videoSource = SourceVideo.FromPath; //sourceVideo : d'où vient les path des fichiers lus
        private SourceVideo _videoSource = SourceVideo.FromExcel; //sourceVideo : d'où vient les path des fichiers lus
        //private string _hiddenDirName = @"2ikljst!mbhoeujy4a1iqv"; //TODO _hiddenDirName _ répertoire pour mieux cacher les fichiers média sur l'ordinateur local

        public string ExcelMediaListPath {
            get => Util.ConvertToAbsolutePath(_excelMediaListPath);
            set { _excelMediaListPath = Util.ConvertToAbsolutePath(value); } // OnPropertyChanged(); }
        }

        public string DefaultVideoFullPath {
            get => Util.ConvertToAbsolutePath(_defaultVideoFullPath);
        }

        public SourceVideo VideoSource {
            get => _videoSource;
            set { _videoSource = value; //OnPropertyChanged(); 
            }
        }
    }

}
