using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.types
{
    public class AccountAlias
    {
        private string account;
        private string alias;

        public AccountAlias(string account, string alias)
        {
            Account = account;
            Alias = alias;
        }

        public string ToXmlString()
        {
            string xml =
                 "\t<AccountAlias>"
                + "\t\t<Account>" + account + "</Account>"
                + "\t\t\t<Alias>" + alias + "</Alias>"
                + "\t</AccountAlias>";          
            
            return xml;
        }

        public string Account
        {
            get { return account; }
            set { account = value; }
        }
        
        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }
    }

    public class AdvisorGroup
    {
        private string name;
        private string defaultMethod;
        private List<string> accounts;

        public AdvisorGroup(string name, string defaultMethod)
        {
            Name = name;
            Accounts = new List<string>();
            DefaultMethod = defaultMethod;
        }

        public string ToXmlString()
        {
            string xml =
                 "  <Group>"
                +"      <name>"+name+"</name>"
                +"      <ListOfAccts varName=\"list\">";
            foreach(string account in accounts)
                xml+="         <String>"+account+"</String>";

            xml +=
                 "      </ListOfAccts>"
                +"      <defaultMethod>"+defaultMethod+"</defaultMethod>"
                +"  </Group>";
            
            return xml;
        }

        public String AccountsToString()
        {
            string accountStr = Accounts[0];
            for (int i = 1; i < Accounts.Count; i++)
                accountStr += "," + Accounts[i];
            return accountStr;
        }

        public void AccountsFromString(string accStr)
        {
            string[] accts = accStr.Split(',');
            foreach (string s in accts)
                Accounts.Add(s);
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
       
        public string DefaultMethod
        {
            get { return defaultMethod; }
            set { defaultMethod = value; }
        }
        
        public List<string> Accounts
        {
            get { return accounts; }
            set { accounts = value; }
        }
    }

    public class AllocationProfile
    {
        private string name;
        private int type;
        private List<Allocation> allocations;

        public AllocationProfile(string name, int type)
        {
            Name = name;
            Type = type;
            allocations = new List<Allocation>();
        }

        public string ToXmlString()
        {
            string xml = 
                 "  <AllocationProfile>"
                +"      <name>"+name+"</name>"
                +"      <type>"+Type+"</type>"
                +"      <ListOfAllocations varName=\"listOfAllocations\">";

            foreach (Allocation profileAllocation in allocations)
                xml += profileAllocation.ToXmlString();

            xml +=
                 "      </ListOfAllocations>"
                +"  </AllocationProfile>";

            return xml;
        }

        public string AllocationsToString()
        {
            string str = Allocations[0].Account+"/"+Allocations[0].Amount;
            for(int i=1; i<allocations.Count; i++)
            {
                str += "," + Allocations[i].Account + "/" + Allocations[i].Amount;
            }
            return str;
        }

        public bool AllocationsFromString(string allocString)
        {
            try
            {
                string[] allocations = allocString.Split(',');
                foreach (string s in allocations)
                {
                    string[] accountAndValue = s.Split('/');
                    Allocations.Add(new Allocation(accountAndValue[0], Double.Parse(accountAndValue[1])));
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }                
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }
       
        public List<Allocation> Allocations
        {
            get { return allocations; }
            set { allocations = value; }
        }
    }

    public class Allocation
    {
        private string account;
        private double amount;

        public Allocation(string account, double amount)
        {
            Account = account;
            Amount = amount;
        }

        public string ToXmlString()
        {
            string xml = 
                 "          <Allocation>"
                +"              <acct>"+account+"</acct>"
                +"              <amount>"+amount+"</amount>"
                +"              <posEff>O</posEff>"
                +"          </Allocation>";

            return xml;
        }

        public string Account
        {
            get { return account; }
            set { account = value; }
        }
        
        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }
    }
}
