using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfMediaPlayerRA.myutil;
using WpfMediaPlayerRA.UtilWpf;

namespace WpfMediaPlayerRA {
    /// <summary>
    /// Logique d'interaction pour SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window {
        public string[] _tabTxt = { "Chargement", "Chargement.", "Chargement..", "Chargement..." };
        private DispatcherTimer _timerUI;
        private int _iTxt = 0;
        private readonly double TIMER_SPEED = 750;

        public SplashWindow() {
            InitializeComponent();
            init();
        }

        private void init() {
            timer_init();
            timer_start();
            txtChargement.Text = _tabTxt[++_iTxt % _tabTxt.Length];
        }
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
        }

        public void timer_stop() {
            _timerUI.Stop();
        }


        private void timer_event(object sender, EventArgs e) {
            txtChargement.Text = _tabTxt[++_iTxt % _tabTxt.Length];
        }
        #endregion timer
    }
}
