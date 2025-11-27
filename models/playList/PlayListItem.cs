using System.ComponentModel;
using System.IO;
namespace Models {
    public class PlayListItem : INotifyPropertyChanged {
        private string _icon, _name, _fullName;
        private float _debut, _fin;

        public PlayListItem() { }
        public PlayListItem(string icon, string name, string fullname, float debut = 0, float fin = 1) {
            Icon = icon;
            Name = name;
            FullName = fullname;
            Debut = debut;
            Fin = fin;
        }
        public string Icon {
            get { return _icon; }
            set {
                if (_icon != value) {
                    _icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }
        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string FullName {
            get { return _fullName; }
            set {
                if (_fullName != value) {
                    _fullName = setFichierLocal(value);
                    OnPropertyChanged(nameof(FullName));
                }
            }
        }

        public float Debut { get => _debut; set => _debut = value; }
        public float Fin { get => _fin; set => _fin = value; }

        private string setFichierLocal(string path) {
            string randomName = Path.GetRandomFileName();
            string destinationFullPath;

            //Path.GetTempPath() donne le dossier temporaire du système (ex. C:\Users\robert.aube\AppData\Local\Temp)
     //       do { //s'Assurer de créer un nom de fichier inexistant
                destinationFullPath = Path.Combine(Path.GetTempPath(), randomName + Path.GetExtension(path));
       //     } while (!File.Exists(destinationFullPath));
            byte[] mediaData = File.ReadAllBytes(path);
            File.WriteAllBytes(destinationFullPath, mediaData);

            return destinationFullPath;
        }

        public string toString() { return _name; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
