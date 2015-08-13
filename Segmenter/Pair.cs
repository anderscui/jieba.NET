using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiebaNet.Segmenter
{
    public class Pair<TKey>
    {
        public TKey key;
        public Double freq = 0.0;

        public Pair(TKey key, double freq)
        {
            this.key = key;
            this.freq = freq;
        }

        public override String ToString()
        {
            return "Candidate [key=" + key + ", freq=" + freq + "]";
        }
    }
}
