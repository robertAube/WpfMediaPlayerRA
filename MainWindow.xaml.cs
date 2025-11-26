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
using WpfMediaPlayerRA.myutil;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;


namespace WpfMediaPlayerRA {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
        private DispatcherTimer _uiTimer;
        private bool _isSeekingByUser = false;
        private bool _endReached = false;


        internal static AppConfig AppConfig { get; private set; }

        // Chemin d'exemple : remplace par ta vidéo
        private string _mediaPath = @"Q:\zulu\Release\demo\Q1.mkv";

        public MainWindow() {
            InitializeComponent();
            Core.Initialize();
            //var label = new Label
            //{
            //    Content = "TEST",
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    VerticalAlignment = VerticalAlignment.Bottom,
            //    Foreground = new SolidColorBrush(Colors.Red)
            //};
            //test.Children.Add(label);

            AppConfig = new AppConfig();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            init();
        }
        #region init
        private void init() {
            initListView();
            initButton();
            initTimer();
            initMediaPlayer();
        }

        private void initListView() {
            LoadVideoFiles(@"Q:\zulu\Release\demo"); // Charge les fichiers dans la ListView
        }

        private void initMediaPlayer() {
            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            VideoView.Loaded += (sender, e) => VideoView.MediaPlayer = _mediaPlayer;
            //            Unloaded += Example2_Unloaded;
            //_mediaPlayer.Opening += (s, e) => Dispatcher.Invoke(() => PlayPauseButton.IsChecked = true);
            //_mediaPlayer.Playing += (s, e) =>
            //{
            //    Dispatcher.Invoke(() =>
            //    {
            //        PlayPauseButton.Content = "Pause";
            //        PlayPauseButton.IsChecked = true;
            //        UpdateTotalTimeLabel();
            //        _uiTimer.Start();
            //    });
            //};


            // Abonner aux événements
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
            //            _mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;

        }

        private void initButton() {
            // Événements media utiles
            _mediaPlayer.Paused += (s, e) => Dispatcher.Invoke(() =>
            {
                PlayPauseButton.Content = "Lecture";
                PlayPauseButton.IsChecked = false;
            });
            _mediaPlayer.Stopped += (s, e) => Dispatcher.Invoke(() =>
            {
                PlayPauseButton.Content = "Lecture";
                PlayPauseButton.IsChecked = false;
                PositionSlider.Value = 0;
                CurrentTimeText.Text = "00:00";
                //                _uiTimer.Stop();
            });
        }
        #endregion init

        private void LoadVideoFiles(string folderPath) {
            if (Directory.Exists(folderPath)) {

                // Common video file extensions
                string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" };

                // Get all files and filter by extension
                var videoFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                                          .Where(file => videoExtensions.Contains(Path.GetExtension(file).ToLower()))
                                          .ToList();


                foreach (var file in videoFiles) {
                    FilesListView.Items.Add(System.IO.Path.GetFileName(file));
                }
            }
        }


        private void Example2_Unloaded(object sender, RoutedEventArgs e) {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
            //          _libVLC.Dispose();
        }
        #region timer
        private void initTimer() {
            // Timer UI : met à jour le slider de position et les textes
            _uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _uiTimer.Tick += UiTimer_Tick;
        }

        private void UiTimer_Tick(object sender, EventArgs e) {

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
            float pos = _mediaPlayer.Position; // 0..1
            PositionSlider.Value = pos * 100.0;

            // Temps courant (ms)
            long currentMs = _mediaPlayer.Time;
            CurrentTimeText.Text = UtilDateTime.FormatTime(currentMs);
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
                _uiTimer.Stop();
            }
        }
        private void PositionSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            //       _isDragging = true;
        }



        private void PositionSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            //       _isDragging = true;
        }

        private void PositionSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            // Appliquer le seek à la position voulue
            //if (mediaElementMain.NaturalDuration.HasTimeSpan && _mediaCanSeek) {
            //    var target = TimeSpan.FromMilliseconds(sliderDuration.Value);
            //    _isDragging = false;
            //    mediaElementMain.Position = target;
            //    textBlockProgress.Text = FormatTime(target);
            //}
            //_isDragging = false;
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


        //void PlayButton_Click(object sender, RoutedEventArgs e) {
        //    if (!VideoView.MediaPlayer.IsPlaying) {
        //        using (var media = new Media(_libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")))
        //            VideoView.MediaPlayer.Play(media);
        //        //VideoView.Height = SPAutre.Height;
        //        //VideoView.Width = SPAutre.Width;
        //    }
        //}

        protected override void OnClosed(EventArgs e) {
            VideoView.Dispose();
        }

        private void PlayPauseButton_Checked(object sender, RoutedEventArgs e) {

        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_mediaPlayer != null) {
                _mediaPlayer.SetRate((float)e.NewValue);
                SpeedValueText.Text = $"{e.NewValue:F2}x";
            }

        }
        // Toggle Play/Pause
        private void PlayPauseButton_Click(object sender, RoutedEventArgs e) {
            if (_mediaPlayer == null)
                return;

            if (_mediaPlayer.IsPlaying) {
                _mediaPlayer.Pause();
                PlayPauseButton.Content = "Lecture";
                PlayPauseButton.IsChecked = false;
            }
            else {
                // Si aucun média n'est chargé, on peut relire le dernier ou réouvrir
                if (_mediaPlayer.Media == null) {
                    OpenAndPlay(_mediaPath);
                }
                else {
                    _mediaPlayer.Play();
                }
                PlayPauseButton.Content = "Pause";
                PlayPauseButton.IsChecked = true;
            }
        }
        private void StartLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            //_startLimit = (float)(e.NewValue / 100);
            //StartLimitText.Text = $"{e.NewValue:F0}%";
        }

        private void EndLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            //_endLimit = (float)(e.NewValue / 100);
            //EndLimitText.Text = $"{e.NewValue:F0}%";
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
                string selectedFile = FilesListView.SelectedItem.ToString();
                _mediaPath = Path.Combine(@"Q:\zulu\Release\demo", selectedFile);
                OpenAndPlay(_mediaPath);
            }
        }
        #endregion main events

        //private void OpenAndPlay(string path) {
        //    var media = new Media(_libVLC, path, FromType.FromPath);
        //    _mediaPlayer.Play(media);
        //    TotalTimeText.Text = FormatTime(_mediaPlayer.Length);
        //}
        private void OpenAndPlay(string filePath) {
            var media = new Media(_libVLC, filePath, FromType.FromPath);
            TotalTimeText.Text = UtilDateTime.FormatTime(_mediaPlayer.Length);
            _mediaPlayer.Play(media);
            PlayPauseButton.Content = "Pause";
            PlayPauseButton.IsChecked = true;
            _uiTimer.Start();
        }

    }
}