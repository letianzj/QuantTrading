using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingBase
{
    /// <summary>
    /// instructions used to control offset amounts and distances.
    /// </summary>
    public class OffsetInfo
    {
        /// <summary>
        /// copy an existing offset to this one
        /// </summary>
        /// <param name="copy"></param>
        public OffsetInfo(OffsetInfo copy) : this(copy.ProfitDist, copy.StopDist, copy.ProfitPercent, copy.StopPercent, copy.NormalizeSize, copy.MinimumLotSize) { }
        /// <summary>
        /// create an offset instruction
        /// </summary>
        /// <param name="profitdist">in cents</param>
        /// <param name="stopdist">in cents</param>
        /// <param name="profitpercent">in percent (eg .1 = 10%)</param>
        /// <param name="stoppercent">in percent (eg .1 = 10%)</param>
        /// <param name="NormalizeSize">true or false</param>
        /// <param name="MinSize">minimum lot size when normalize size is true</param>
        public OffsetInfo(decimal profitdist, decimal stopdist, decimal profitpercent, decimal stoppercent, bool NormalizeSize, int MinSize)
        {
            ProfitDist = profitdist;
            StopDist = stopdist;
            ProfitPercent = profitpercent;
            StopPercent = stoppercent;
            this.NormalizeSize = NormalizeSize;
            MinimumLotSize = MinSize;
        }
        public bool NormalizeSize = false;
        public int MinimumLotSize = 1;
        public OffsetInfo(decimal profitdist, decimal stopdist, decimal profitpercent, decimal stoppercent) : this(profitdist, stopdist, profitpercent, stoppercent, false, 1) { }
        public OffsetInfo(decimal profitdist, decimal stopdist) : this(profitdist, stopdist, 1, 1, false, 1) { }
        public OffsetInfo() : this(0, 0, 1, 1, false, 1) { }
        public long ProfitId = 0;
        public long StopId = 0;
        public decimal ProfitDist = 0;
        public decimal StopDist = 0;
        public decimal ProfitPercent = 1;
        public decimal StopPercent = 1;
        public int SentProfitSize = 0;
        public int SentStopSize = 0;
        public bool isOffsetCurrent(Position p)
        {
            return isStopCurrent(p) && isProfitCurrent(p);
        }
        public bool isStopCurrent(Position p)
        {
            Order s = Calc.PositionStop(p, this);
            return (s.OrderSize == SentStopSize);
        }
        public bool isProfitCurrent(Position p)
        {
            Order l = Calc.PositionProfit(p, this);
            return (l.OrderSize == SentProfitSize);
        }
        public bool hasProfit { get { return ProfitId != 0; } }
        public bool hasStop { get { return StopId != 0; } }
        public bool StopcancelPending = false;
        public bool ProfitcancelPending = false;
        public override string ToString()
        {
            return ToString(2);
        }
        public string ToString(int decimals)
        {
            return string.Format("p{0}/{1:p0} s{2}/{3:p0}", ProfitDist.ToString("N" + decimals.ToString()), ProfitPercent, StopDist.ToString("N" + decimals.ToString()), StopPercent);
        }

        public static string Serialize(OffsetInfo oi)
        {
            string m =
                string.Format("{0},{1},{2},{3},{4},{5}", oi.ProfitDist, oi.StopDist, oi.ProfitPercent, oi.StopPercent, oi.NormalizeSize, oi.MinimumLotSize);
            return m;

        }

        public static OffsetInfo Deserialize(string msg)
        {
            string[] r = msg.Split(',');
            if (r.Length < System.Enum.GetNames(typeof(OffsetInfoFields)).Length) return null;

            decimal pd;
            decimal sd;
            decimal pp;
            decimal sp;
            bool n = false;
            int min = 1;
            decimal.TryParse(r[(int)OffsetInfoFields.ProfitDist], out pd);
            decimal.TryParse(r[(int)OffsetInfoFields.StopDist], out sd);
            decimal.TryParse(r[(int)OffsetInfoFields.ProfitPercent], out pp);
            decimal.TryParse(r[(int)OffsetInfoFields.StopPercent], out sp);
            bool.TryParse(r[(int)OffsetInfoFields.Normalize], out n);
            int.TryParse(r[(int)OffsetInfoFields.MinSize], out min);
            return new OffsetInfo(pd, sd, pp, sd, n, min);
        }
        /// <summary>
        /// set an offset to this to disable it
        /// </summary>
        /// <returns></returns>
        public static OffsetInfo DISABLEOFFSET()
        {
            return new OffsetInfo(0, 0, 0, 0, false, 1);
        }
    }

    public enum OffsetInfoFields
    {
        ProfitDist,
        StopDist,
        ProfitPercent,
        StopPercent,
        Normalize,
        MinSize,
    }
}
