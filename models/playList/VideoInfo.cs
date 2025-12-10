using MyUtil;


namespace WpfMediaPlayerRA.models.playList {
    internal class VideoInfo {
        private string videoName, videoFullPath;

        public string VideoName {
            get { return videoName; }
            set { videoName = value; }
        }
        public string VideoFullPath {
            get { return videoFullPath; }
        }

        public VideoInfo(string name, string path) {
            videoName = name;
            videoFullPath = Util.ConvertToAbsolutePath(path);
        }

    }
}
