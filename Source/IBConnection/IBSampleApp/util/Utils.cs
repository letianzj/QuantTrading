using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;

namespace IBSampleApp.util
{
    public class Utils
    {
        public static string ContractToString(Contract contract)
        {
            if (contract.PrimaryExch != null)
                return contract.Symbol + " " + contract.SecType + " " + contract.Currency + " @ " + contract.PrimaryExch;
            else
                return contract.Symbol + " " + contract.SecType + " " + contract.Currency;
        }

        public static bool ContractsAreEqual(Contract contractA, Contract contractB)
        {
            if (contractA.Symbol.Equals(contractB.Symbol) && contractA.SecType.Equals(contractB.SecType) && contractA.Currency.Equals(contractB.Currency))
            {
                if (contractA.Expiry != null && contractB.Expiry != null)
                {
                    if (contractA.Expiry.Equals(contractB.Expiry))
                    {
                        if (contractA.Multiplier != null && contractB.Multiplier != null)
                        {
                            return contractA.Multiplier.Equals(contractB.Multiplier);
                        }
                        else
                            return true;
                    }
                }
                else
                    return true;
            }

            return false;
        }
    }
}
