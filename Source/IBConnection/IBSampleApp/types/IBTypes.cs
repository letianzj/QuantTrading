using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace IBSampleApp.types
{
    public class IBType
    {
        private string name;
        private object value;

        public IBType(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }
        
        public object Value
        {
            get { return this.value; }
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class TriggerMethod
    {
        public static object[] GetAll()
        {
            return new object[] { Default, DoubleBidAsk, Last, DoubleLast,  BidAsk, LastBidOrAsk, Midpoint};
        }
        public static IBType Default = new IBType("Default", 0);
        public static IBType DoubleBidAsk = new IBType("DoubleBidAsk", 1);
        public static IBType Last = new IBType("Last", 2);
        public static IBType DoubleLast = new IBType("DoubleLast", 3);
        public static IBType BidAsk = new IBType("BidAsk", 4);
        public static IBType LastBidOrAsk = new IBType("LastBidOrAsk", 5);
        public static IBType Midpoint = new IBType("Midpoint", 6);
    }

    public class Rule80A
    {
        public static object[] GetAll()
        {
            return new object[] { None, IndivArb, IndivBigNonArb, IndivSmallNonArb, INST_ARB, InstBigNonArb, InstSmallNonArb };
        }
        public static IBType None = new IBType("None", "");
        public static IBType IndivArb = new IBType("IndivArb", "J");
        public static IBType IndivBigNonArb = new IBType("IndivBigNonArb", "K");
        public static IBType IndivSmallNonArb = new IBType("IndivSmallNonArb", "I");
        public static IBType INST_ARB = new IBType("INST_ARB", "U");
        public static IBType InstBigNonArb = new IBType("InstBigNonArb", "Y");
        public static IBType InstSmallNonArb = new IBType("InstSmallNonArb", "A");
    }

    public class OCAType
    {
        public static object[] GetAll()
        {
            return new object[] {None, CancelWithBlocking, ReduceWithBlocking, ReduceWithoutBlocking };
        }
        public static IBType None = new IBType("None", 0);
        public static IBType CancelWithBlocking = new IBType("CancelWithBlocking", 1);
        public static IBType ReduceWithBlocking = new IBType("ReduceWithBlocking", 2);
        public static IBType ReduceWithoutBlocking = new IBType("ReduceWithoutBlocking", 3);
    }

    public class HedgeType
    {
        public static object[] GetAll()
        {
            return new object[] { None, Delta, Beta, Fx, Pair };
        }
        public static IBType None = new IBType("None", "");
        public static IBType Delta = new IBType("Delta", "D");
        public static IBType Beta = new IBType("Beta", "B");
        public static IBType Fx = new IBType("Fx", "F");
        public static IBType Pair = new IBType("Pair", "P");
    }

    public class VolatilityType
    {
        public static object[] GetAll()
        {
            return new object[] { None, Daily, Annual};
        }
        public static IBType None = new IBType("None", 0);
        public static IBType Daily = new IBType("Daily", 1);
        public static IBType Annual = new IBType("Annual", 1);
    }

    public class ReferencePriceType
    {
        public static object[] GetAll()
        {
            return new object[] {None, Midpoint, BidOrAsk };
        }
        public static IBType None = new IBType("None", 0);
        public static IBType Midpoint = new IBType("Midpoint", 1);
        public static IBType BidOrAsk = new IBType("BidOrAsk", 2);
    }

    public class FaMethod
    {
        public static object[] GetAll()
        {
            return new object[] { None, EqualQuantity, AvailableEquity, NetLiq, PctChange };
        }
        public static IBType None = new IBType("None", "");
        public static IBType EqualQuantity = new IBType("EqualQuantity", "EqualQuantity");
        public static IBType AvailableEquity = new IBType("AvailableEquity", "AvailableEquity");
        public static IBType NetLiq = new IBType("NetLiq", "NetLiq");
        public static IBType PctChange = new IBType("PctChange", "PctChange");
    }

    public class ContractRight
    {
        public static object[] GetAll()
        {
            return new object[] { None, Put, Call};
        }

        public static IBType None = new IBType("None", "");
        public static IBType Put = new IBType("Put", "P");
        public static IBType Call = new IBType("Call", "C");
    }

    public class FundamentalsReport
    {
        public static object[] GetAll()
        {
            return new object[] { ReportSnapshot, FinancialSummary, Ratios, FinStatements, RESC, CalendarReport };
        }
        public static IBType ReportSnapshot = new IBType("Company overview", "ReportSnapshot");
        public static IBType FinancialSummary = new IBType("Financial summary", "ReportsFinSummary");
        public static IBType Ratios = new IBType("Financial ratios", "ReportRatios");
        public static IBType FinStatements = new IBType("Financial statements", "ReportsFinStatements");
        public static IBType RESC = new IBType("Analyst estimates", "RESC");
        public static IBType CalendarReport = new IBType("Company calendar", "CalendarReport");
    }

    public class FinancialAdvisorDataType
    {
        public static object[] GetAll()
        {
            return new object[] { Groups, Profiles, Aliases };
        }

        public static IBType Groups = new IBType("Groups", 1);
        public static IBType Profiles = new IBType("Profiles", 2);
        public static IBType Aliases = new IBType("Alias", 3);
    }

    public class AllocationGroupMethod
    {
        //The DataTable will then properly populate the grid's ComboBox cell
        public static DataTable GetAsData()
        {
            DataTable faDefaultMethods = new DataTable();
            faDefaultMethods.Columns.Add(new DataColumn("Name", typeof(string)));
            faDefaultMethods.Columns.Add(new DataColumn("Value", typeof(string)));
            faDefaultMethods.Rows.Add("Equal quantity", "EqualQuantity");
            faDefaultMethods.Rows.Add("Available equity", "AvailableEquity");
            faDefaultMethods.Rows.Add("Net liquidity", "NetLiquidity");
            faDefaultMethods.Rows.Add("Percent change", "PctChange");
            return faDefaultMethods;
        }

        public static IBType EqualQuantity = new IBType("Equal quantity", "EqualQuantity");
        public static IBType AvailableEquity = new IBType("Available equity", "AvailableEquity");
        public static IBType NetLiquidity = new IBType("Net liquidity", "NetLiquidity");
        public static IBType PercentChange = new IBType("Percent change", "PctChange");
    }

    public class AllocationProfileType
    {
        public static DataTable GetAsData()
        {
            DataTable allocationProfileTypes = new DataTable();
            allocationProfileTypes.Columns.Add(new DataColumn("Name", typeof(string)));
            allocationProfileTypes.Columns.Add(new DataColumn("Value", typeof(int)));
            allocationProfileTypes.Rows.Add("Percentages", 1);
            allocationProfileTypes.Rows.Add("Financial Ratios", 2);
            allocationProfileTypes.Rows.Add("Shares", 3);
            return allocationProfileTypes;
        }
    }
}
