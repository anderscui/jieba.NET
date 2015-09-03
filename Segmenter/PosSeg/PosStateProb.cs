using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiebaNet.Segmenter.PosSeg
{
    public class PosStateProb
    {
        public string PosState { get; set; }
        public double Prob { get; set; }
    }

    public class PosStateTransProb
    {
        public string PosStateFrom { get; set; }
        public List<PosStateProb> PosStatesTo { get; set; }
    }
}
