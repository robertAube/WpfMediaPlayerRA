using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfMediaPlayerRA.UtilWpf
{
    class SliderStartEnd
    {
        private float oldStartLimit, oldEndLimit;
        private Slider startSlider, endSlider;
        private readonly float MAX_END_LIMIT = 1;
        private readonly float NEAR_GAP_LIMIT = (float)0.02;
        private readonly double TIMER_SPEED = 100 * 2;


        public float StartLimit {
            set {
                startSlider.Value = value; 
            }
                    
            get { return (float)startSlider.Value; }
        }
        public float EndLimit {
            set {
                endSlider.Value = value; 
            }
            get { return (float)endSlider.Value; }
        }

        public SliderStartEnd(Slider startSlider, Slider endSlider) {
            this.startSlider = startSlider;
            this.endSlider = endSlider;
            initSlider();
        }

        private void initSlider() {
            StartLimit = 0;
            EndLimit = MAX_END_LIMIT;
            savePosition();
        }

        private void savePosition() {
            oldStartLimit = StartLimit;
            oldEndLimit = EndLimit;
        }

        public void gererSlider() {
            if (startDeplaceVersDroite()) {
                if (StartLimit == MAX_END_LIMIT) {
                    StartLimit = MAX_END_LIMIT - NEAR_GAP_LIMIT;
                }
                if (StartLimit > EndLimit) {
                    StartLimit = EndLimit - NEAR_GAP_LIMIT;
                }
            }
        }

        private bool startDeplaceVersDroite() {
            return oldStartLimit < StartLimit;
        }

        internal bool startSlider_Or_EndSlider_Moved() {
            return startDeplaceVersDroite();
        }


        internal float getNewPosition() {
            savePosition();

            return StartLimit;
        }
    }
}
