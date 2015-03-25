/****************************** Project Header ******************************\
Project:	      QuantTrading
Author:			  Letian_zj @ Codeplex
URL:			  https://quanttrading.codeplex.com/
Copyright 2014 Letian_zj

This file is part of QuantTrading Project.

QuantTrading is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, 
either version 3 of the License, or (at your option) any later version.

QuantTrading is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with QuantTrading. 
If not, see http://www.gnu.org/licenses/.

\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingBase
{
    /// <summary>
    /// Collection of calculations
    /// </summary>
    public static class Calc
    {
        #region statistics
        /// <summary>
        /// gets minum of an array (will return MinValue if array has no elements)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal Min(decimal[] array)
        {
            if (array == null || array.Length == 0)
                return 0;

            decimal low = decimal.MaxValue;
            for (int i = 0; i < array.Length; i++)
                if (array[i] < low) low = array[i];
            return low;
        }

        /// <summary>
        /// gets maximum in an array (will return MaxValue if array has no elements)
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static decimal Max(decimal[] array)
        {
            if (array == null || array.Length == 0)
                return 0;

            decimal max = decimal.MinValue;
            for (int i = 0; i < array.Length; i++)
                if (array[i] > max) max = array[i];
            return max;
        }

        public static decimal Sum(decimal[] array) { return Sum(array, 0, array.Length); }

        /// <summary>
        /// sum part of an array
        /// </summary>
        public static decimal Sum(decimal[] array, int startindex, int length)
        {
            decimal sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i];
            return sum;
        }

        public static decimal Avg(decimal[] array) { return Avg(array, true, 0, array.Length); }

        public static decimal Avg(decimal[] array, bool returnzeroifEmpty, int start, int len)
        {
            if (returnzeroifEmpty)
            {
                if (array.Length == 0)
                    return 0;
                return Calc.Sum(array, start, len) / len;
            }
            return Calc.Sum(array, start, len) / len;
        }

        public static decimal SumSquares(decimal[] array) { return SumSquares(array, 0, array.Length); }

        public static decimal SumSquares(decimal[] array, int startindex, int length)
        {
            decimal sum = 0;
            for (int i = startindex; i < startindex + length; i++)
                sum += array[i] * array[i];
            return sum;
        }

        /// <summary>
        /// gets standard deviation for values of a population
        /// </summary>
        public static decimal StdDev(decimal[] array)
        {
            decimal avg = Avg(array);
            decimal sq = SumSquares(array);
            decimal tmp = (sq / array.Length) - (avg * avg);
            decimal stdev = (decimal)Math.Pow((double)tmp, .5);
            return stdev;

        }
        #endregion

        #region PnL
        /// <summary>
        /// Gets the open PL on a per-share basis, ignoring the size of the position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="PosSize">Size of the pos.</param>
        /// <returns></returns>
        public static decimal OpenPT(decimal LastTrade, decimal AvgPrice, int PosSize)
        {
            return (PosSize == 0) ? 0 : OpenPT(LastTrade, AvgPrice, PosSize > 0);
        }
        /// <summary>
        /// Gets the open PL on a per-share basis (also called points or PT), ignoring the size of the position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="Side">if set to <c>true</c> [side].</param>
        /// <returns></returns>
        public static decimal OpenPT(decimal LastTrade, decimal AvgPrice, bool Side)
        {
            return Side ? LastTrade - AvgPrice : AvgPrice - LastTrade;
        }
        public static decimal OpenPT(decimal LastTrade, Position Pos)
        {
            return OpenPT(LastTrade, Pos.AvgPrice, Pos.Size);
        }
        /// <summary>
        /// Gets the open PL considering all the shares held in a position.
        /// </summary>
        /// <param name="LastTrade">The last trade.</param>
        /// <param name="AvgPrice">The avg price.</param>
        /// <param name="PosSizeMultiplier">Size of the pos.</param>
        /// <returns></returns>
        public static decimal OpenPL(decimal LastTrade, decimal AvgPrice, int PosSizeMultiplier)
        {
            return PosSizeMultiplier * (LastTrade - AvgPrice);
        }
        /// <summary>
        /// get open pl for position given the last trade
        /// </summary>
        /// <param name="LastTrade"></param>
        /// <param name="Pos"></param>
        /// <returns></returns>
        public static decimal OpenPL(decimal LastTrade, Position Pos)
        {
            return OpenPL(LastTrade, Pos.AvgPrice, Pos.Size * Security.GetMultiplierFromFullSymbol(Pos.FullSymbol));
        }

        // these are for calculating closed pl
        // they do not adjust positions themselves
        /// <summary>
        /// Gets the closed PL on a per-share basis, ignoring how many shares are held.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position that's being closed/changed.</param>
        /// <returns></returns>
        public static decimal ClosePT(Position existing, Trade adjust)
        {
            if (!existing.isValid || !adjust.IsValid)
                throw new Exception("Invalid position provided. (existing:" + existing.ToString() + " adjustment:" + adjust.ToString());
            if (existing.isFlat) return 0; // nothing to close
            if (existing.isLong == adjust.Side) return 0; // if we're adding, nothing to close
            return existing.isLong ? adjust.TradePrice - existing.AvgPrice : existing.AvgPrice - adjust.TradePrice;
        }

        /// <summary>
        /// Gets the closed PL on a position basis, the PL that is registered to the account for the entire shares transacted.
        /// </summary>
        /// <param name="existing">The existing position.</param>
        /// <param name="closing">The portion of the position being changed/closed.</param>
        /// <returns></returns>
        public static decimal ClosePL(Position existing, Trade adjust)
        {
            int closedsize = Math.Abs(adjust.TradeSize) > existing.UnsignedSize ? existing.UnsignedSize : Math.Abs(adjust.TradeSize);
            return ClosePT(existing, adjust) * closedsize * Security.GetMultiplierFromFullSymbol(adjust.FullSymbol);
        }
        #endregion

        #region Performance Evaluation
        /// <summary>
        /// computes money used to purchase a portfolio of positions.
        /// uses average price for position.
        /// </summary>
        /// <param name="plist"></param>
        /// <returns></returns>
        public static decimal[] MoneyInUse(List<Position> plist)
        {
            decimal[] miu = new decimal[plist.Count];
            for (int i = 0; i < plist.Count; i++)
                miu[i] += plist[i].AvgPrice * plist[i].UnsignedSize * Security.GetMultiplierFromFullSymbol(plist[i].FullSymbol);
            return miu;
        }

        public static decimal[] MoneyInUse(Dictionary<string, Position> pdict)
        {
            decimal[] miu = new decimal[pdict.Count];
            int i = 0;
            foreach (KeyValuePair<string, Position> item in pdict)
                miu[i++] += item.Value.AvgPrice * item.Value.UnsignedSize * Security.GetMultiplierFromFullSymbol(item.Key);
            return miu;
        }

        /// <summary>
        /// calculate absolute return only for closed portions of positions
        /// </summary>
        /// <param name="pdict"></param>
        /// <returns></returns>
        public static decimal[] AbsoluteReturn(Dictionary<string, Position> pdict)
        {
            return AbsoluteReturn(pdict, new Dictionary<string, decimal>(), true);

        }

        /// <summary>
        /// returns absolute return of all positions
        /// both closed and open pl may be included
        /// </summary>
        /// <param name="pdict"></param>
        /// <param name="marketprices"> used to calculate open pl </param>
        /// <param name="countClosedPL"> is closed pl included or not </param>
        /// <returns></returns>
        public static decimal[] AbsoluteReturn(Dictionary<string, Position> pdict, Dictionary<string, decimal> marketprices, bool countClosedPL)
        {
            decimal[] aret = new decimal[pdict.Count];
            bool countOpenPL = marketprices.Count >= pdict.Count;
            int i = 0;
            foreach (KeyValuePair<string, Position> item in pdict)  // for (int i = 0; i < pdict.Count; i++)
            {
                // get position
                Position p = item.Value;

                if (countOpenPL && marketprices.ContainsKey(item.Key))
                    aret[i] += Calc.OpenPL(marketprices[item.Key], p);
                if (countClosedPL)
                    aret[i] += p.ClosedPL;
                i++;
            }
            return aret;
        }

        /// <summary>
        /// maximum drawdown as a percentage
        /// </summary>
        /// <param name="fills"></param>
        /// <returns></returns>
        public static decimal MaxDDPct(List<Trade> fills)
        {
            Portfolio portfolio = new Portfolio();
            List<decimal> ret = new List<decimal>();
            decimal mmiu = 0;
            for (int i = 0; i < fills.Count; i++)
            {
                portfolio.Adjust(fills[i]);
                decimal miu = Calc.Sum(Calc.MoneyInUse(portfolio.Positions));
                if (miu > mmiu)
                    mmiu = miu;
                ret.Add(Calc.Sum(Calc.AbsoluteReturn(portfolio.Positions, new Dictionary<string,decimal>(), true)));
            }
            decimal maxddval = MaxDDVal(ret.ToArray());
            decimal pct = mmiu == 0 ? 0 : maxddval / mmiu;
            return pct;
        }

        /// <summary>
        /// calculate maximum drawdown from a PL stream for a given security/portfolio as a dollar value
        /// </summary>
        /// <param name="ret">array containing pl values for portfolio or security</param>
        /// <returns></returns>
        public static decimal MaxDDVal(decimal[] ret)
        {
            int maxi = 0;
            int prevmaxi = 0;
            int prevmini = 0;
            for (int i = 0; i < ret.Length; i++)
            {
                if (ret[i] >= ret[maxi])
                    maxi = i;
                else
                {
                    if ((ret[maxi] - ret[i]) > (ret[prevmaxi] - ret[prevmini]))
                    {
                        prevmaxi = maxi;
                        prevmini = i;
                    }
                }
            }
            return (ret[prevmini] - ret[prevmaxi]);
        }

        /// <summary>
        /// computes sharpe ratio for a constant rate of risk free returns, give portfolio rate of return and portfolio volatility
        /// </summary>
        /// <param name="ratereturn"></param>
        /// <param name="stdevRate"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static decimal SharpeRatio(decimal ratereturn, decimal stdevRate, decimal riskFreeRate)
        {
            return (ratereturn - riskFreeRate) / stdevRate;
        }

        /// <summary>
        /// computes sortinio ratio for constant rate of risk free return, give portfolio rate of return and downside volatility
        /// </summary>
        /// <param name="ratereturn"></param>
        /// <param name="stdevRateDownside"></param>
        /// <param name="riskFreeRate"></param>
        /// <returns></returns>
        public static decimal SortinoRatio(decimal ratereturn, decimal stdevRateDownside, decimal riskFreeRate)
        {
            return (ratereturn - riskFreeRate) / stdevRateDownside;
        }
        #endregion

        #region OffsetInfo
        /// <summary>
        /// Provides an offsetting price from a position.
        /// For profit taking
        /// </summary>
        /// <param name="p">Position</param>
        /// <param name="offset">Offset amount</param>
        /// <returns>Offset price</returns>
        public static decimal OffsetPrice(Position p, decimal offset) { return OffsetPrice(p.AvgPrice, p.isLong, offset); }
        public static decimal OffsetPrice(decimal AvgPrice, bool side, decimal offset)
        {
            return side ? AvgPrice + offset : AvgPrice - offset;
        }
        /// <summary>
        /// Defaults to 100% of position at target.
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">your target</param>
        /// <returns>profit taking limit order</returns>
        public static Order PositionProfit(Position p, decimal offset) { return PositionProfit(p, offset, 1, false, 1); }
        /// <summary>
        /// Generates profit taking order for a given position, at a specified per-share profit target.  
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">target price, per share/contract</param>
        /// <param name="percent">percent of the position to close with this order</param>
        /// <returns></returns>
        public static Order PositionProfit(Position p, decimal offset, decimal percent) { return PositionProfit(p, offset, percent, false, 1); }
        /// <summary>
        /// Generates profit taking order for a given position, at a specified per-share profit target.  
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">target price, per share/contract</param>
        /// <param name="percent">percent of the position to close with this order</param>
        /// <param name="normalizesize">whether to normalize order to be an even-lot trade</param>
        /// <param name="MINSIZE">size of an even lot</param>
        /// <returns></returns>
        public static Order PositionProfit(Position p, decimal offset, decimal percent, bool normalizesize, int MINSIZE)
        {
            Order o = new Order();
            if (!p.isValid || p.isFlat) return o;
            decimal price = Calc.OffsetPrice(p, offset);
            int size = percent == 0 ? 0 : (!normalizesize ? (int)(p.FlatSize * percent) : Calc.Norm2Min(p.FlatSize * percent, MINSIZE));
            o = new LimitOrder(p.FullSymbol, p.isLong ? -size : size, price);
            return o;
        }
        /// <summary>
        /// get profit order for given position given offset information
        /// </summary>
        /// <param name="p"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Order PositionProfit(Position p, OffsetInfo offset) { return PositionProfit(p, offset.ProfitDist, offset.ProfitPercent, offset.NormalizeSize, offset.MinimumLotSize); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price.  Defaults to 100% of position.
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset) { return PositionStop(p, offset, 1, false, 1); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <param name="percent">what percent of position to close</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset, decimal percent) { return PositionStop(p, offset, percent, false, 1); }
        /// <summary>
        /// Generate a stop order for a position, at a specified per-share/contract price
        /// </summary>
        /// <param name="p">your position</param>
        /// <param name="offset">how far away stop is</param>
        /// <param name="percent">what percent of position to close</param>
        /// <param name="normalizesize">whether to normalize size to even-lots</param>
        /// <param name="MINSIZE">size of an even lot</param>
        /// <returns></returns>
        public static Order PositionStop(Position p, decimal offset, decimal percent, bool normalizesize, int MINSIZE)
        {
            Order o = new Order();
            if (!p.isValid || p.isFlat) return o;
            decimal price = Calc.OffsetPrice(p, offset * -1);
            int size = percent == 0 ? 0 : (!normalizesize ? (int)(p.FlatSize * percent) : Calc.Norm2Min(p.FlatSize * percent, MINSIZE));
            o = new StopOrder(p.FullSymbol, p.isLong ? -size : size, price);
            return o;
        }
        /// <summary>
        /// get a stop order for a position given offset information
        /// </summary>
        /// <param name="p"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Order PositionStop(Position p, OffsetInfo offset) { return PositionStop(p, offset.StopDist, offset.StopPercent, offset.NormalizeSize, offset.MinimumLotSize); }
        #endregion

        #region other
        /// <summary>
        /// Normalizes any order size to the minimum lot size specified by MinSize.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static int Norm2Min(decimal size, int MINSIZE) { return Norm2Min(size, MINSIZE, true); }
        public static int Norm2Min(decimal size, int MINSIZE, bool roundup)
        {
            int sign = size >= 0 ? 1 : -1;
            int mult = (int)Math.Floor(size / MINSIZE);
            if (roundup)
            {
                mult = (int)Math.Ceiling(size / MINSIZE);
            }

            int result = mult * MINSIZE;
            int final = sign * Math.Max(Math.Abs(result), MINSIZE);
            return final;
        }

        public static decimal[] Double2Decimal(double[] a)
        {
            decimal[] b = new decimal[a.Length];
            for (int i = 0; i < a.Length; i++)
                b[i] = (decimal)a[i];
            return b;
        }

        public static double[] Decimal2Double(decimal[] a)
        {
            double[] b = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
                b[i] = (double)a[i];
            return b;
        }
        #endregion
    }
}
