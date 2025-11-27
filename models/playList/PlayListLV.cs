using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfMediaPlayerRA.models.playList {
    class PlayListLV {
        private ListView listView;

        public PlayListLV(ListView listView, string path) {
            this.listView = listView;
            LoadVideoFiles(path);
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

        private void LoadVideoFiles(string folderPath) {
            if (Directory.Exists(folderPath)) {

                // Common video file extensions
                string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" };

                // Get all files and filter by extension
                var videoFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                                          .Where(file => videoExtensions.Contains(Path.GetExtension(file).ToLower()))
                                          .ToList();


                foreach (var fullPath in videoFiles) {
                    var playListItem = new PlayListItem("", Path.GetFileNameWithoutExtension(fullPath), fullPath);
                    listView.Items.Add(playListItem); 
                }
            }
        }

    }
}
