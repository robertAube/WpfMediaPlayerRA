
//using LibVLCSharp.Shared;
//using LibVLCSharp.WPF;
//using System;
//using System.IO;
//using System.Windows;
//using System.Windows.Threading;

//namespace LibVLCSharpDemo {
//    public partial class MainWindow : Window {
//        private LibVLC _libVLC;
//        private MediaPlayer _mediaPlayer;
//        private DispatcherTimer _uiTimer;
//        private bool _isSeekingByUser = false;

//        private float _startLimit = 0f; // en pourcentage (0..1)
//        private float _endLimit = 1f;   // en pourcentage (0..1)

//        public MainWindow() {
//            InitializeComponent();
//            Core.Initialize();

//            _libVLC = new LibVLC();
//            _mediaPlayer = new MediaPlayer(_libVLC);
//            VideoView.MediaPlayer = _mediaPlayer;

//            _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
//            _uiTimer.Tick += UiTimer_Tick;

//            LoadVideoFiles(@"C:\Videos");
//        }

//        private void LoadVideoFiles(string folderPath) {
//            if (Directory.Exists(folderPath)) {
//                var files = Directory.GetFiles(folderPath, "*.mp4");
//                foreach (var file in files) {
//                    FilesListView.Items.Add(Path.GetFileName(file));
//                }
//            }
//        }

//        private void FilesListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
//            if (FilesListView.SelectedItem != null) {
//                string selectedFile = FilesListView.SelectedItem.ToString();
//                string fullPath = Path.Combine(@"C:\Videos", selectedFile);
//                PlayVideo(fullPath);
//            }
//        }

//        private void PlayVideo(string filePath) {
//            var media = new Media(_libVLC, filePath, FromType.FromPath);
//            _mediaPlayer.Play(media);
//            PlayPauseButton.Content = "Pause";
//            PlayPauseButton.IsChecked = true;
//            _uiTimer.Start();
//        }

//        private void PlayPauseButton_Click(object sender, RoutedEventArgs e) {
//            if (_mediaPlayer.IsPlaying) {
//                _mediaPlayer.Pause();
//                PlayPauseButton.Content = "Lecture";
//            }
//            else {
//                _mediaPlayer.Play();
//                PlayPauseButton.Content = "Pause";
//            }
//        }

//        private void StopButton_Click(object sender, RoutedEventArgs e) {
//            _mediaPlayer.Stop();
//            PlayPauseButton.Content = "Lecture";
//            PositionSlider.Value = 0;
//            CurrentTimeText.Text = "00:00";
//            _uiTimer.Stop();
//        }

//        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
//            _mediaPlayer.SetRate((float)e.NewValue);
//            SpeedValueText.Text = $"{e.NewValue:F2}x";
//        }

//        private void UiTimer_Tick(object sender, EventArgs e) {
//            if (_mediaPlayer.Length > 0 && !_isSeekingByUser) {
//                float pos = _mediaPlayer.Position;
//                PositionSlider.Value = pos * 100;
//                CurrentTimeText.Text = FormatTime(_mediaPlayer.Time);
//                TotalTimeText.Text = FormatTime(_mediaPlayer.Length);

//                // Vérifier la limite de fin
//                if (pos >= _endLimit) {
//                    _mediaPlayer.Pause();
//                    PlayPauseButton.Content = "Lecture";
//                }
//            }
//        }

//        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
//            if (_isSeekingByUser) return;
//            if (_mediaPlayer.Length > 0 && PositionSlider.IsMouseOver) {
//                _mediaPlayer.Position = (float)(PositionSlider.Value / 100);
//            }
//        }

//        private void StartLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
//            _startLimit = (float)(e.NewValue / 100);
//            StartLimitText.Text = $"{e.NewValue:F0}%";
//        }

//        private void EndLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
//            _endLimit = (float)(e.NewValue / 100);
//            EndLimitText.Text = $"{e.NewValue:F0}%";
//        }

//        private static string FormatTime(long ms) {
//            var ts = TimeSpan.FromMilliseconds(ms);
//            return ts.Hours > 0 ? $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}" : $"{ts.Minutes:D2}:{ts.Seconds:D2}";
//        }

//        protected override void OnClosed(EventArgs e) {
//            base.OnClosed(e);
//            _uiTimer.Stop();
//            _mediaPlayer.Dispose();
//            _libVLC.Dispose();
//        }
//    }
//}
