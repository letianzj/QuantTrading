using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using IBSampleApp.types;

namespace IBSampleApp.util
{
    public class XmlHelper
    {
        private static string LIST_OF_ALIASES = "ListOfAccountAliases";
        private static string LIST_OF_GROUPS = "ListOfGroups";
        private static string LIST_OF_PROFILES = "ListOfAllocationProfiles";

        public static List<T> ParseFAInformation<T>(string faInformation)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(faInformation);
            if (document.DocumentElement.Name.Equals(LIST_OF_ALIASES))
                return GetAliasesList(document).Cast<T>().ToList();
            if (document.DocumentElement.Name.Equals(LIST_OF_GROUPS))
                return GetGroupsList(document).Cast<T>().ToList();
            if (document.DocumentElement.Name.Equals(LIST_OF_PROFILES))
                return GetProfilesList(document).Cast<T>().ToList();
            return null;
        }

        public static string ListToXML<T>(List<T> objects)
        {
            return "";
        }

        private static List<AccountAlias> GetAliasesList(XmlDocument xmlDocument)
        {
            List<AccountAlias> accountAliases = new List<AccountAlias>();
            XmlNode accountListNode = xmlDocument.GetElementsByTagName(LIST_OF_ALIASES).Item(0);
            XmlNodeList aliasesList = accountListNode.ChildNodes;
            for (int i = 0; i < aliasesList.Count; i++)
            {
                XmlNode aliasNode = aliasesList.Item(i);
                accountAliases.Add(new AccountAlias(aliasNode.ChildNodes[0].InnerText, aliasNode.ChildNodes[1].InnerText));
            }
            return accountAliases;
        }

        private static List<AdvisorGroup> GetGroupsList(XmlDocument xmlDocument)
        {
            List<AdvisorGroup> advisorGroups = new List<AdvisorGroup>();
            XmlNode groupsListNode = xmlDocument.GetElementsByTagName(LIST_OF_GROUPS).Item(0);
            XmlNodeList groupsList = groupsListNode.ChildNodes;
            for (int i = 0; i < groupsList.Count; i++)
            {
                AdvisorGroup advisorGroup = new AdvisorGroup(groupsList.Item(i).ChildNodes[0].InnerText, groupsList.Item(i).ChildNodes[2].InnerText);
                XmlNodeList accountNodes = groupsList.Item(i).ChildNodes[1].ChildNodes;
                for (int j = 0; j < accountNodes.Count; j++)
                {
                    advisorGroup.Accounts.Add(accountNodes[j].InnerText);
                }
                advisorGroups.Add(advisorGroup);
            }
            return advisorGroups;
        }

        private static List<AllocationProfile> GetProfilesList(XmlDocument xmlDocument)
        {
            List<AllocationProfile> advisorProfiles = new List<AllocationProfile>();
            XmlNode profilesListNode = xmlDocument.GetElementsByTagName(LIST_OF_PROFILES).Item(0);
            XmlNodeList profilesList = profilesListNode.ChildNodes;
            for (int i=0; i<profilesList.Count; i++)
            {
                AllocationProfile allocationProfile = new AllocationProfile(profilesList.Item(i).ChildNodes[0].InnerText, Int32.Parse(profilesList.Item(i).ChildNodes[1].InnerText));
                XmlNodeList allocationNodes = profilesList[i].ChildNodes[2].ChildNodes;
                for (int j = 0; j < allocationNodes.Count; j++)
                {
                    allocationProfile.Allocations.Add(new Allocation(allocationNodes[j].ChildNodes[0].InnerText, Double.Parse(allocationNodes[j].ChildNodes[1].InnerText)));
                }
                advisorProfiles.Add(allocationProfile);
            }
            return advisorProfiles;
        }
    }

    
}
