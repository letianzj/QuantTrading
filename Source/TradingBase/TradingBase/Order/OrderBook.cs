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
    public class OrderBook
    {
        public const int MAXBOOK = 40;
        public OrderBook(string sym)
        {
            UpdateTime = 0;
            ActualDepth = 0;
            maxbook = MAXBOOK;
            Sym = sym;
            bidprice = new decimal[maxbook];
            bidsize = new int[maxbook];
            askprice = new decimal[maxbook];
            asksize = new int[maxbook];
            bidex = new string[maxbook];
            askex = new string[maxbook];
        }
        public OrderBook(OrderBook copy)
        {
            ActualDepth = copy.ActualDepth;
            UpdateTime = copy.UpdateTime;
            Sym = copy.Sym;
            maxbook = copy.maxbook;
            bidprice = new decimal[copy.askprice.Length];
            bidsize = new int[copy.askprice.Length];
            askprice = new decimal[copy.askprice.Length];
            asksize = new int[copy.askprice.Length];
            bidex = new string[copy.askprice.Length];
            askex = new string[copy.askprice.Length];
            Array.Copy(copy.bidprice, bidprice, copy.bidprice.Length);
            Array.Copy(copy.bidsize, bidsize, copy.bidprice.Length);
            Array.Copy(copy.askprice, askprice, copy.bidprice.Length);
            Array.Copy(copy.asksize, asksize, copy.bidprice.Length);
            for (int i = 0; i < copy.ActualDepth; i++)
            {
                bidex[i] = copy.bidex[i];
                askex[i] = copy.askex[i];
            }
        }
        public int UpdateTime;
        public int ActualDepth;
        int maxbook;
        
        public bool isValid { get { return Sym != null; } }
        public string Sym;
        public decimal[] bidprice;
        public int[] bidsize;
        public decimal[] askprice;
        public int[] asksize;
        public string[] bidex;
        public string[] askex;
        public void Reset()
        {
            ActualDepth = 0;
            for (int i = 0; i < maxbook; i++)
            {
                bidex[i] = null;
                askex[i] = null;
                bidprice[i] = 0;
                bidsize[i] = 0;
                askprice[i] = 0;
                asksize[i] = 0;
            }
        }
        public void GotTick(Tick k)
        {
            // ignore trades
            if (k.IsTrade) return;
            // make sure depth is valid for this book
            if ((k.Depth < 0) || (k.Depth >= maxbook)) return;
            if (Sym == null)
                Sym = k.FullSymbol;
            // make sure symbol matches
            if (k.FullSymbol != Sym) return;
            // if depth is zero, must be a new book
            if (k.Depth == 0) Reset();
            // update buy book
            if (k.HasBid)
            {
                bidprice[k.Depth] = k.BidPrice;
                bidsize[k.Depth] = k.BidSize;
                bidex[k.Depth] = "an exchange";
                if (k.Depth > ActualDepth)
                    ActualDepth = k.Depth;
            }
            // update sell book
            if (k.HasAsk)
            {
                askprice[k.Depth] = k.AskPrice;
                asksize[k.Depth] = k.AskSize;
                askex[k.Depth] = "an exchange";
                if (k.Depth > ActualDepth)
                    ActualDepth = k.Depth;
            }
        }
    }
}
