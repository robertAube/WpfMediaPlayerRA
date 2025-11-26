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

        public void initSlider() {
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
            if (endDeplaceVersGauche()) {
                if (EndLimit == 0) {
                    EndLimit = 0 + NEAR_GAP_LIMIT;
                }
                if (StartLimit > EndLimit) {
                    EndLimit = StartLimit + NEAR_GAP_LIMIT;
                }
            }

            savePosition();
        }


        private bool startDeplaceVersDroite() {
            return oldStartLimit < StartLimit;
        }

        private bool endDeplaceVersGauche() {
            return oldEndLimit > EndLimit;
        }

        private bool startLimite_depasse_mediaPosition(float mediaPosition) {
            bool depasse = false;
            depasse = StartLimit > mediaPosition;
            return depasse;
        }


        internal bool haveToChangeMediaPosition(float mediaPosition) { 
            return startLimite_depasse_mediaPosition(mediaPosition) ||
                mediaPosition_reachedLimitDroite(mediaPosition)
                ;
        }

        private bool mediaPosition_reachedLimitDroite(float mediaPosition) {
            bool limiteAtteinte = false;
            limiteAtteinte = mediaPosition > EndLimit;
            return limiteAtteinte;
        }

        internal float getNewPosition(float mediaPosition) {
            float newPosition = 0;

            if (startLimite_depasse_mediaPosition(mediaPosition) ) {
                newPosition = StartLimit;
            }
            if (mediaPosition_reachedLimitDroite(mediaPosition)) {
                newPosition = StartLimit;
            }

            savePosition();

            return newPosition;
        }

    }
}
