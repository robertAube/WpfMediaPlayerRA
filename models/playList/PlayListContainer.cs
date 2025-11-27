using System.ComponentModel;

using System.Collections.ObjectModel;
namespace Models
{
    public class PlayListContainer : INotifyPropertyChanged
    {
        private ObservableCollection<PlayListItem> _playListData = new ObservableCollection<PlayListItem>();
        public PlayListContainer()
        {
            //For UI testing purpose: 
            //_playListData = new ObservableCollection<PlayList>();
            //_playListData.Add(new PlayList("Icons/Music.ico", "test.mp3", "C:\\Musics\\test.mp3"));
            //_playListData.Add(new PlayList("Icons/Video.ico", "test.mp4", "C:\\Musics\\test.mp4"));
        }
        public PlayListContainer(ObservableCollection<PlayListItem> playListData)
        {
            this._playListData = playListData;
        }

        public ObservableCollection<PlayListItem> PlayListData
        {
            get { return _playListData; }
            set
            {
                _playListData = value;
                OnPropertyChanged(nameof(PlayListData));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
