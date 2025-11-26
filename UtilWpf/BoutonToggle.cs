using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static WpfMakeBack.Utilitaires.BoutonToggle;

namespace WpfMakeBack.Utilitaires {
    internal class BoutonToggle {
        public enum OnOff {
            On = 0,
            Off = 1,
        }
        private OnOff State { get; set; } = OnOff.On;

        private Button btn;
        private string strOn;
        private string strOff;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="btn">Boutton wpf</param>
        /// <param name="strOff">string quand off</param>
        /// <param name="strOn">string quand on</param>
        /// <param name="etat"></param>
        public BoutonToggle(Button btn, string strOff, string strOn, OnOff etat = OnOff.Off) {
            this.btn = btn;
            this.strOn = strOn;
            this.strOff = strOff;
            State = etat;
            setContent();
        }

        public OnOff toggle() {
            OnOff oldState = State;
            toggleOnOff();
            setContent();
            return oldState;
        }

        public void setOff() {
            State = OnOff.Off;
            setContent();
        }

        public void setOn() {
            State = OnOff.On;
            setContent();
        }

        private void toggleOnOff() {
            State = State == OnOff.On ? OnOff.Off : OnOff.On;
        }

        private void setContent() {
            btn.Content = State == OnOff.Off ? strOff : strOn;
        }
    }
}
