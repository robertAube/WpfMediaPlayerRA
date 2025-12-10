using DocumentFormat.OpenXml.Office2010.PowerPoint;
using LibVLCSharp.Shared;
using Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using WpfMediaPlayerRA.models.playList;
using WpfMediaPlayerRA.myutil;
using WpfMediaPlayerRA.UtilWpf;
using static WpfMediaPlayerRA.AppConfig;
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
        private DispatcherTimer _slowTimerUI;
        private int nbErreur = 0;

        private bool _isSeekingByUser = false;
        private bool _endReached = false;

        public static AppConfig AppConfig {
            get { return appConfig; }
        }

        private PlayListItem? selectedPlayListItem;
        private string _mediaPath;
        private SliderStartEnd sliderStartEnd;
        private readonly double TIMER_SPEED = 100;

        private PlayListLV playListLV;
        private long _dureeMiliSeconde;
        private bool _ready;

        private ListeVideo listeVideo;
        private bool _ancienneEtat;

        public bool Ready {
            get { return _ready; }
        }
        public MainWindow() {
            InitializeComponent();
            _ready = false;
        }

        public void premierDepart() {
            if (allowToPlay()) {
                demarrerPremiereVideo();
            }
        }
        //Rendre le constructeur léger et déplacer l’init dans Loaded 

        public async Task InitializeAsync() {
            await Task.Run(() =>
            {
                init();
                _ready = true;
            });

        }


        private bool allowToPlay() {
            return AppConfig.SlowTimer && Path.Exists(appConfig.FilePathGo);
        }

        private void demarrerPremiereVideo() {
            FilesListView.SelectedItem = null;
//            FilesListView.SelectedItem = 1;
            playListLV.select(0);
        }
        #region init
        public void init() {
            gererArguments(); //TODO gérerArguments devrait être
            initListVideo();
            initVLC();
        }


        public void initView() {
            initListView(); 
            timer_init();
            slowTimer_init();
            initMediaPlayer();
            initButton();
//            initFermeture(); //pour supprimer les fichiers à la fin
        }
        private void initListVideo() {
            listeVideo = new ListeVideo(AppConfig.VideoSource, AppConfig.FileScanFullPath);
        }

        public void gererArguments() {
            string[] args = Environment.GetCommandLineArgs();

            traiterNomDeFichierExcel(args);
        }

        private void traiterNomDeFichierExcel(string[] args) {
            string? fichierExcel = args.FirstOrDefault(a => a.EndsWith(".xlsm", StringComparison.OrdinalIgnoreCase));
            if (fichierExcel != null) { //si fichier excel reçu en argument premier pris en compte
                AppConfig.ExcelMediaListPath = fichierExcel;
                AppConfig.VideoSource = SourceVideo.FromExcel;
            } //fichier excelDefaut existe et qu'on demande un fichier excel
            else if (AppConfig.VideoSource == SourceVideo.FromExcel) {
                if (File.Exists(AppConfig.DefaultExcelMediaListPath)) {
                    AppConfig.ExcelMediaListPath = AppConfig.DefaultExcelMediaListPath;
                }
            }
            //            bool safeMode = args.Any(a => a.Equals("--safe", StringComparison.OrdinalIgnoreCase));
        }

        private void initFermeture() { //pour supprimer les fichiers à la fin
                                       //            var app = (App)Application.Current;
                                       ////            app.listViewG.listView = FilesListView;
                                       //            app.listViewG._mediaPlayer = _mediaPlayer;
                                       //            app.listViewG.pathLocal = MainWindow.AppConfig.RepTempLocal;
            //var app = (App)Application.Current; 
            //app.listViewG.pathLocal = AppConfig.RepTempLocal;
        }

        public void initListView() {
            playListLV = new PlayListLV(FilesListView, listeVideo, StartLimitSlider, EndLimitSlider); // Charge les fichiers dans la ListView
                                                                                                                      //            sliderStartEnd = new SliderStartEnd(StartLimitSlider, EndLimitSlider);
        }

        public void initVLC() {
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
        }

        private void initMediaPlayer() {

            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            VideoView.Loaded += (sender, e) => VideoView.MediaPlayer = _mediaPlayer;
            // Abonner aux événements
            //            _mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
        }
        #endregion init

        #region timerVideo
        //private void timer_start() {
        //    _timerUI.Start();
        //    // TODO Certains conteneurs mal muxés (MKV sans cues, MP3 VBR sans index, AVI incomplet) peuvent ne pas exposer une durée fiable.
        //}
        private void timer_init() {
            // Timer UI : met à jour le slider de position et les textes
            _timerUI = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_SPEED)
            };
            _timerUI.Tick += timer_event;
        }

        private void timer_event(object? sender, EventArgs e) {
            if (_mediaPlayer == null || _isSeekingByUser)
                return;

            try { //TODO ajouté parce qu'il arrive que mon lecteur a planté: cause inconnu :/ 
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
            //    throw new Exception("Pour tester");
            }
            catch (Exception ex) {
                //on passe le clic
                MessageBox.Show("Une erreur (" + ++nbErreur + "x) est survenue dans timer_event.\nAppelez votre enseignant!" + Environment.NewLine + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion timerVideo

        #region slowTimer
        private void slowTimer_init() {
            if (AppConfig.SlowTimer) {
                // Timer UI : met à jour le slider de position et les textes
                _slowTimerUI = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(TIMER_SPEED * 10)
                };
                _slowTimerUI.Tick += slowTimer_event;
                _slowTimerUI.Start();
            }
        }
        private void slowTimer_event(object? sender, EventArgs e) {
            bool nouvelEtat;

            if (_mediaPlayer == null)
                return;
            try { //TODO ajouté parce qu'il arrive que mon lecteur a planté: cause inconnu :/ 
                nouvelEtat = allowToPlay();
                if (!nouvelEtat) { //on ne veut pas que ça joue
                    var app = (App)Application.Current;
                    FilesListView.SelectedItem = null;
                    stopMedia();
                }
                else if (_ancienneEtat != nouvelEtat) {
                    premierDepart();
                }
                _ancienneEtat = nouvelEtat;
            }
            catch (Exception ex) {
                //on passe le clic
                MessageBox.Show("Une erreur (\" + ++nbErreur + \"x) est survenue dans slowTimer_event.\nAppelez votre enseignant!" + Environment.NewLine + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion slowTimer


        #region main events
        private void MediaPlayer_EndReached(object? sender, EventArgs e) { //Fin de lecture de la vidéo
            // ⚠️ Cet événement se produit sur un thread de lecture, pas le thread UI.
            ThreadPool.QueueUserWorkItem(_ => _endReached = true);
        }


        void StopButton_Click(object sender, RoutedEventArgs e) {
            stopMedia();
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


        private void btnPlayPause_Click(object sender, RoutedEventArgs e) {
            if (_mediaPlayer == null)
                return;
            if (btnPlayPause.IsChecked == true) {
                btnPlayPauseSetPlay();
            }
            else {
                btnPlayPauseSetPause();
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

        private void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
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
                _ = OpenAndPlayAsync(_mediaPath);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            // fermer les fichier temporaire local
            playListLV.deleteTempFile();
        }
        #endregion main events
        
        private void btnPlayPauseSetPlay() {
            btnPlayPause.IsChecked = true;
            if (_mediaPlayer.Media == null) {
                //                OpenAndPlay(_mediaPath);
            }
            else {
                _mediaPlayer.Play();
                _timerUI.Start();
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
                _timerUI.Stop();
                btnPlayPause.Content = "Lecture";
                btnPlayPause.IsChecked = false;
            }
        }

        private void stopMedia() {
            if (VideoView.MediaPlayer != null) {
                if (VideoView.MediaPlayer.IsPlaying) {
                    VideoView.MediaPlayer.Stop();
                    _timerUI.Stop();

                    PositionSlider.Value = 0;
                    CurrentTimeText.Text = "00:00";
                    btnPlayPauseSetPause();
                }
            }
        }

        //private void OpenAndPlay(string path) {
        //    var media = new Media(_libVLC, path, FromType.FromPath);
        //    _mediaPlayer.Play(media);
        //    TotalTimeText.Text = FormatTime(_mediaPlayer.Length);
        //}
        private async Task OpenAndPlayAsync(string filePath) {
            if (AppConfig.SlowTimer && allowToPlay()) {
                var media = new LibVLCSharp.Shared.Media(_libVLC, filePath, FromType.FromPath);

                btnPlayPause.Content = "Pause";
                btnPlayPause.IsChecked = true;
                _timerUI.Start();

                await media.Parse(MediaParseOptions.ParseLocal);

                int retries = 20;
                while (media.Duration == 0 && retries-- > 0) {
                    await Task.Delay(100);
                }
                _dureeMiliSeconde = media.Duration;
                TotalTimeText.Text = UtilDateTime.FormatTime(_dureeMiliSeconde);

                _mediaPlayer.Play(media);
            }
        }


        //private async Task mettreAJourDureeAsync(LibVLCSharp.Shared.Media media, System.Windows.Controls.TextBlock totalTimeText) {
        //    // Parse le média pour obtenir la durée
        //    media.ParsedChanged += (sender, args) =>
        //    {
        //        long longeurMedia;
        //        if (media.Duration > 0) {

        //        }
        //    };
        //    await media.Parse(MediaParseOptions.ParseLocal);
        //    _dureeMiliSeconde = media.Duration;
        //}
        //private async Task mettreAJourDureeAsync(LibVLCSharp.Shared.Media media) {
        //    // Parse le média pour obtenir la durée

        //    media.Playing += (sender, args) =>
        //    {
        //        _dureeMiliSeconde = media.Duration;
        //    }

        //}


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
