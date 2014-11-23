using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBSampleApp.types;
using IBSampleApp.messages;
using IBSampleApp.util;

namespace IBSampleApp.ui
{
    public class AdvisorManager
    {
        private IBClient ibClient;
        private DataGridView aliasesGrid;
        private DataGridView profilesGrid;
        private DataGridView groupsGrid;

        public AdvisorManager(IBClient ibClient, DataGridView aliasesGrid, DataGridView groupsGrid, DataGridView profilesGrid)
        {
            IbClient = ibClient;
            AliasesGrid = aliasesGrid;
            GroupsGrid = groupsGrid;
            ProfilesGrid = profilesGrid;
        }

        public void UpdateUI(AdvisorDataMessage message)
        {
            switch (message.FaDataType)
            {
                case 1:
                {
                    HandleGroupsData(message.Data);
                    break;
                }
                case 2:
                {
                    HandleProfilesData(message.Data);
                    break;
                }
                case 3:
                {
                    HandleAliasesData(message.Data);
                    break;
                }
            }
        }

        private void HandleAliasesData(string aliasData)
        {
            List<AccountAlias> aliases = XmlHelper.ParseFAInformation<AccountAlias>(aliasData);
            for(int i=0; i < aliases.Count; i++)
            {
                aliasesGrid.Rows.Add(1);
                aliasesGrid[0, i].Value = aliases[i].Account;
                aliasesGrid[1, i].Value = aliases[i].Alias;
            }
        }

        private void HandleProfilesData(string profilesData)
        {
            List<AllocationProfile> profiles = XmlHelper.ParseFAInformation<AllocationProfile>(profilesData);
            for (int i = 0; i < profiles.Count; i++)
            {
                profilesGrid.Rows.Add(1);
                profilesGrid[0, i].Value = profiles[i].Name;
                profilesGrid[1, i].Value = profiles[i].Type;
                profilesGrid[2, i].Value = profiles[i].AllocationsToString();
            }
        }

        public void SaveProfiles()
        {
            string xmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                            + "<ListOfAllocationProfiles>";
            for (int i = 0; i < profilesGrid.Rows.Count-1; i++)
            {
                AllocationProfile allocProfile = new AllocationProfile((string)profilesGrid[0, i].Value, (int)profilesGrid[1, i].Value);
                allocProfile.AllocationsFromString((string)profilesGrid[2, i].Value);
                xmlData += allocProfile.ToXmlString();
            }
            xmlData += "</ListOfAllocationProfiles>";
            ibClient.ClientSocket.replaceFA((int)FinancialAdvisorDataType.Profiles.Value, xmlData);
        }

        private void HandleGroupsData(string groupsData)
        {
            List<AdvisorGroup> groups = XmlHelper.ParseFAInformation<AdvisorGroup>(groupsData);
            for (int i = 0; i < groups.Count; i++)
            {
                groupsGrid.Rows.Add(1);
                groupsGrid[0, i].Value = groups[i].Name;
                ((DataGridViewComboBoxCell)groupsGrid[1, i]).Value = groups[i].DefaultMethod;
                groupsGrid[2, i].Value = groups[i].AccountsToString();
            }
        }

        public void SaveGroups()
        {
            string xmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                        + "<ListOfGroups>";
            for (int i = 0; i < groupsGrid.Rows.Count - 1; i++)
            {
                AdvisorGroup advisorGroup = new AdvisorGroup((string)groupsGrid[0, i].Value, (string)groupsGrid[1, i].Value);
                advisorGroup.AccountsFromString((string)groupsGrid[2, i].Value);
                xmlData += advisorGroup.ToXmlString();
            }
            xmlData += "</ListOfGroups>";
            ibClient.ClientSocket.replaceFA((int)FinancialAdvisorDataType.Groups.Value, xmlData);
        }

        public void RequestFAData(IBType dataType)
        {
            ibClient.ClientSocket.requestFA((int)(dataType.Value));
        }

        public IBClient IbClient
        {
            get { return ibClient; }
            set { ibClient = value; }
        }
        public DataGridView AliasesGrid
        {
            get { return aliasesGrid; }
            set { aliasesGrid = value; }
        }

        public DataGridView GroupsGrid
        {
            get { return groupsGrid; }
            set { groupsGrid = value; }
        }

        public DataGridView ProfilesGrid
        {
            get { return profilesGrid; }
            set { profilesGrid = value; }
        }
    }
}
