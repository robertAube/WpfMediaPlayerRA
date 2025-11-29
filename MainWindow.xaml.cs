using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using LibVLCSharp.Shared;
using MirzaMediaPlayer;
using Models;
using System;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfMakeBack.Utilitaires;
using WpfMediaPlayerRA.models.playList;
using WpfMediaPlayerRA.myutil;
using WpfMediaPlayerRA.UtilWpf;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;


namespace WpfMediaPlayerRA {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static AppConfig appConfig = new AppConfig();
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
        private DispatcherTimer _timerUI;

        private bool _isSeekingByUser = false;
        private bool _endReached = false;

        public static AppConfig AppConfig {
            get { return appConfig; }
        }

        // Chemin d'exemple : remplace par ta vidéo
        private static string MEDIA_PATH = @"Q:\zulu\Release\demo";
        private PlayListItem? selectedPlayListItem;
        private string _mediaPath;
        private SliderStartEnd sliderStartEnd;
        private readonly double TIMER_SPEED = 100;

        private PlayListLV playListLV;

        public MainWindow() {
            InitializeComponent();

            init();
        }
        #region init
        private void init() {
            initListView();
            timer_init();
            initMediaPlayer();
            initButton();
            initFermeture();
        }

        private void initFermeture() { //pour supprimer les fichiers à la fin
            var app = (App)Application.Current;
            app.listViewG.listView = FilesListView;
            app.listViewG._mediaPlayer = _mediaPlayer;
        }

        private void initListView() {
            playListLV = new PlayListLV(FilesListView, MEDIA_PATH, StartLimitSlider, EndLimitSlider); // Charge les fichiers dans la ListView
                                                                                                      //            sliderStartEnd = new SliderStartEnd(StartLimitSlider, EndLimitSlider);
        }

        private void initMediaPlayer() {
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            VideoView.Loaded += (sender, e) => VideoView.MediaPlayer = _mediaPlayer;
            // Abonner aux événements
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
            //            _mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;

        }
        #endregion init

        #region timer
        private void timer_init() {
            // Timer UI : met à jour le slider de position et les textes
            _timerUI = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_SPEED)
            };
            _timerUI.Tick += timer_event;
        }

        private void timer_start() {
            _timerUI.Start();
            // TODO Certains conteneurs mal muxés (MKV sans cues, MP3 VBR sans index, AVI incomplet) peuvent ne pas exposer une durée fiable.
        }

        private void timer_event(object sender, EventArgs e) {
            if (_mediaPlayer == null || _isSeekingByUser)
                return;

            // fin atteinte
            if (_endReached) {
                _endReached = false;
                _mediaPlayer.Stop();
                _mediaPlayer.Position = 0f;
                _mediaPlayer.Play();
            }
            // Position : 0..1 (float). Slider : 0..100
            sliderStartEnd.gererSlider();

            if (sliderStartEnd.haveToChangeMediaPosition(_mediaPlayer.Position)) {
                _mediaPlayer.Position = sliderStartEnd.getNewPosition(_mediaPlayer.Position);
            }


            float pos = _mediaPlayer.Position; // 0..1
            PositionSlider.Value = pos;



            // Temps courant (ms)
            {
                long currentMs = _mediaPlayer.Time;
                CurrentTimeText.Text = UtilDateTime.FormatTime(currentMs);
            }

            // Temps longueur média
            { //TODO Appellé toujours : on peut faire mieux?
                long longeurMedia;
                // Parse le média pour obtenir la durée
                longeurMedia = _mediaPlayer.Length;
                TotalTimeText.Text = UtilDateTime.FormatTime(_mediaPlayer.Length);
            }
        }
        #endregion timer

        #region main events
        private void MediaPlayer_EndReached(object? sender, EventArgs e) {
            // ⚠️ Cet événement se produit sur un thread de lecture, pas le thread UI.
            ThreadPool.QueueUserWorkItem(_ => _endReached = true);
        }


        void StopButton_Click(object sender, RoutedEventArgs e) {
            if (VideoView.MediaPlayer.IsPlaying) {
                VideoView.MediaPlayer.Stop();
                PositionSlider.Value = 0;
                CurrentTimeText.Text = "00:00";
                _timerUI.Stop();
            }
        }

        // Début du drag sur le slider de position
        private void PositionSlider_DragStarted(object sender, DragStartedEventArgs e) {
            _isSeekingByUser = true;
        }

        // Fin du drag : on applique la position au lecteur
        private void PositionSlider_DragCompleted(object sender, DragCompletedEventArgs e) {
            if (_mediaPlayer == null) return;

            double percent = PositionSlider.Value / PositionSlider.Maximum; // 0..1
            _mediaPlayer.Position = (float)percent;

            // Mettre à jour le temps courant affiché
            long len = _mediaPlayer.Length;
            CurrentTimeText.Text = UtilDateTime.FormatTime((long)(len * percent));

            _isSeekingByUser = false;
        }



        protected override void OnClosed(EventArgs e) {
            VideoView.Dispose();
        }


        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_mediaPlayer != null) {
                _mediaPlayer.SetRate((float)e.NewValue);
                SpeedValueText.Text = $"{e.NewValue:F2}x";
            }

        }
        // Toggle Play/Pause
        private void btnPlayPause_Checked(object sender, RoutedEventArgs e) {

        }
        private void initButton() {
            btnPlayPauseSetPlay();
        }

        private void btnPlayPauseSetPlay() {
            btnPlayPause.IsChecked = true;
            if (_mediaPlayer.Media == null) {
                //                OpenAndPlay(_mediaPath);
            }
            else {
                _mediaPlayer.Play();
                btnPlayPause.Content = "Pause";
                btnPlayPause.IsChecked = true;
            }
        }
        private void btnPlayPauseSetPause() {
            btnPlayPause.IsChecked = true;
            if (_mediaPlayer.Media == null) {
                //                OpenAndPlay(_mediaPath);
            }
            else {
                _mediaPlayer.Pause();
                btnPlayPause.Content = "Lecture";
                btnPlayPause.IsChecked = false;
            }
        }

        private void btnPlayPause_Click(object sender, RoutedEventArgs e) {
            if (_mediaPlayer == null)
                return;
            //        btnPlayPause.IsChecked = !btnPlayPause.IsChecked;
            if (btnPlayPause.IsChecked == true) {
                btnPlayPauseSetPlay();
            }
            else {
                btnPlayPauseSetPause();
            }
        }
        private void StartLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            var startLimit = (float)(e.NewValue * 100);
            StartLimitText.Text = $"{startLimit:F0}%";
        }

        private void EndLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            var endLimit = (float)(e.NewValue * 100);
            EndLimitText.Text = $"{endLimit:F0}%";
        }

        // Ajuster la position si l'utilisateur clique sans drag (ValueChanged)
        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_mediaPlayer == null) return;

            // Si l'utilisateur clique (MoveToPointEnabled), ValueChanged peut survenir sans drag
            if (_isSeekingByUser)
                return;

            if (!IsMouseOver) // éviter les boucles, laisser le timer piloter en lecture
                return;
        }

        private void FilesListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (FilesListView.SelectedItem != null) {
                //if (selectedPlayListItem != null) {
                //    selectedPlayListItem.Debut = sliderStartEnd.StartLimit;
                //    selectedPlayListItem.Fin = sliderStartEnd.EndLimit;
                //}
                selectedPlayListItem = FilesListView.SelectedItem as PlayListItem;
                sliderStartEnd = selectedPlayListItem.SliderStartEnd;
                selectedPlayListItem.SliderStartEnd.reloadSlider();
                //                setLimitStartEnd();

                _mediaPath = selectedPlayListItem.FullName;
                OpenAndPlay(_mediaPath);
            }
        }

        private void setLimitStartEnd() {
            sliderStartEnd.StartLimit = selectedPlayListItem.Debut;
            sliderStartEnd.EndLimit = selectedPlayListItem.Fin;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            // fermer les fichier temporaire local
            playListLV.deleteTempFile();
        }
        #endregion main events

        //private void OpenAndPlay(string path) {
        //    var media = new Media(_libVLC, path, FromType.FromPath);
        //    _mediaPlayer.Play(media);
        //    TotalTimeText.Text = FormatTime(_mediaPlayer.Length);
        //}
        private async void OpenAndPlay(string filePath) {
            var media = new LibVLCSharp.Shared.Media(_libVLC, filePath, FromType.FromPath);
            _mediaPlayer.Play(media);
            btnPlayPause.Content = "Pause";
            btnPlayPause.IsChecked = true;
            timer_start();
        }

    }
}
//void PlayButton_Click(object sender, RoutedEventArgs e) {
//    if (!VideoView.MediaPlayer.IsPlaying) {
//        using (var media = new Media(_libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")))
//            VideoView.MediaPlayer.Play(media);
//        //VideoView.Height = SPAutre.Height;
//        //VideoView.Width = SPAutre.Width;
//    }
//}
