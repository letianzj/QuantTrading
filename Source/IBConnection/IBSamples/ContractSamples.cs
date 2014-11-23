using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;

namespace Samples
{
    public class ContractSamples
    {
        public static Contract getOption()
        {
            Contract contract = new Contract();
            contract.Symbol = "BAYN";
            contract.SecType = "OPT";
            contract.Exchange = "DTB";
            contract.Currency = "EUR";
            contract.Expiry = "201512";
            contract.Strike = 100;
            contract.Right = "CALL";
            contract.Multiplier = "100";
            return contract;
        }

        public static Contract GetWrongContract()
        {
            Contract contract = new Contract();
            contract.Symbol = " IJR ";//note the spaces in the symbol!
            contract.ConId = 9579976;
            contract.SecType = "STK";
            contract.Exchange = "SMART";
            contract.Currency = "USD";
            return contract;
        }

        public static Contract GetSANTOption()
        {
            Contract contract = new Contract();
            contract.Symbol = "SANT";
            contract.SecType = "OPT";
            contract.Exchange = "MEFFRV";
            contract.Currency = "EUR";
            contract.Expiry = "20131220";
            contract.Strike = 6;
            contract.Right = "CALL";
            contract.Multiplier = "100";
            //this contract for example requires the trading class too in order to prevent any ambiguity.
            contract.TradingClass = "SANEU";
            return contract;
        }

        public static Contract GetbyIsin()
        {
            Contract contract = new Contract();
            //contract.SecIdType = "ISIN";
            //contract.SecId = "XS0356687219";
            //contract.Exchange = "EURONEXT";
            contract.Currency = "EUR";
            contract.Symbol = "IBCID127317301";
            contract.SecType = "BOND";
            return contract;
        }

        public static Contract getOptionForQuery()
        {
            Contract contract = new Contract();
            contract.Symbol = "IBM";
            contract.SecType = "OPT";
            contract.Exchange = "SMART";
            contract.Currency = "USD";
            return contract;
        }

        public static Contract getEurUsdForex()
        {
            Contract contract = new Contract();
            //contract.Symbol = "EUR";
            //contract.SecType = "CASH";
            //contract.Currency = "USD";
            //we can also give the conId instead of the whole description
            //but we still need the exchange though
            contract.ConId = 12087792;
            contract.Exchange = "SMART";
            return contract;
        }

        public static Contract getEurGbpForex()
        {
            Contract contract = new Contract();
            contract.Symbol = "EUR";
            contract.SecType = "CASH";
            contract.Currency = "GBP";
            contract.Exchange = "IDEALPRO";
            return contract;
        }

        public static Contract getEuropeanStock()
        {
            Contract contract = new Contract();
            contract.Symbol = "SMTPC";
            contract.SecType = "STK";
            contract.Currency = "EUR";
            contract.Exchange = "SMART";
            return contract;
        }

        public static Contract GetUSStock()
        {
            Contract contract = new Contract();
            contract.Symbol = "AMZN";
            contract.SecType = "STK";
            contract.Currency = "USD";
            contract.Exchange = "SMART";
            return contract;
        }

        public static Contract GetBond()
        {
            Contract contract = new Contract();
            contract.Symbol = "MS";
            contract.SecType = "BOND";
            contract.Currency = "USD";
            contract.Exchange = "SMART";
            return contract;
        }

        public static Contract getComboContract()
        {
            Contract contract = new Contract();
            contract.Symbol = "MCD";
            contract.SecType = "BAG";
            contract.Currency = "USD";
            contract.Exchange = "SMART";

            ComboLeg leg1 = new ComboLeg();
            leg1.ConId = 109385219;//Burger King's stocks
            leg1.Ratio = 1;
            leg1.Action = "BUY";
            leg1.Exchange = "SMART";

            ComboLeg leg2 = new ComboLeg();
            leg2.ConId = 9408;//McDonald's stocks
            leg2.Ratio = 1;
            leg2.Action = "SELL";
            leg2.Exchange = "SMART";

            contract.ComboLegs = new List<ComboLeg>();
            contract.ComboLegs.Add(leg1);
            contract.ComboLegs.Add(leg2);

            return contract;
        }
    }
}
