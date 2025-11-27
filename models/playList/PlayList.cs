using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfMediaPlayerRA.models.playList
{
    class PlayList
    {
        private ListView listView;
        private List<PlayListItem> playListItems;

        public PlayList(ListView listView, string path) {
            this.listView = listView;
            LoadVideoFiles(path);
        }

        private void LoadVideoFiles(string folderPath) {
            if (Directory.Exists(folderPath)) {

                // Common video file extensions
                string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" };

                // Get all files and filter by extension
                var videoFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                                          .Where(file => videoExtensions.Contains(Path.GetExtension(file).ToLower()))
                                          .ToList();


                foreach (var file in videoFiles) {
                    listView.Items.Add(Path.GetFileName(file));
                }
            }
        }

    }
}
