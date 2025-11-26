using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMediaPlayerRA.UtilWpf
{
    class SliderStartEnd
    {
        private int startLimit, endLimit;

        public int StartLimit {
            get { return startLimit; }
        }

        public SliderStartEnd() {
            startLimit = 0;
            endLimit = 1;
        }

    }
}
