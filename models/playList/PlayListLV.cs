using MyUtil;
using System.IO;
using System.Windows.Controls;
using WpfMediaPlayerRA.UtilWpf;
using static WpfMediaPlayerRA.AppConfig;


namespace WpfMediaPlayerRA.models.playList {
    class PlayListLV {
        private ListView listView;
        Slider sliderStart, sliderEnd;

        public PlayListLV(ListView listView, ListeVideo listeVideo, Slider sliderStart, Slider sliderEnd) {
            this.listView = listView;
            this.sliderStart = sliderStart;
            this.sliderEnd = sliderEnd;

            chargerMedia(listeVideo);
        }

        private void chargerMedia(ListeVideo listeVideo) {
            if (listeVideo != null) {
                foreach (VideoInfo infoVideo in listeVideo.VideoInfo) {
                    var playListItem = new PlayListItem("", infoVideo.VideoName, infoVideo.VideoFullPath, new SliderStartEnd(sliderStart, sliderEnd));
                    listView.Items.Add(playListItem);
                }
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
        internal void select(int index) {
            if (listView.Items.Count > 0 && index < listView.Items.Count) { 
                listView.SelectedIndex = index;
            }
        }
    }
}
