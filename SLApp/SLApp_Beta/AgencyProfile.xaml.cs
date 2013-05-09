using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SLApp_Beta
{
    /// <summary>
    /// Interaction logic for AgencyProfile.xaml
    /// </summary>
    public partial class AgencyProfile : Window
    {
        DatabaseMethods dbMethods = new DatabaseMethods();
        Agency agent = new Agency();

        public AgencyProfile()
        {
            InitializeComponent();
        }

        public AgencyProfile(Agency agent, bool isAdmin, bool IsEdit)
        {
            InitializeComponent();

	        this.agent = agent;

            this.agencyName_TB.Text = agent.Name;
            this.agencyEmail_TB.Text = agent.Email;
            this.agencyCoordinatorName_TB.Text = agent.CoordinatorName;
            this.agencyRating_TB.Text = agent.Rating.ToString();
            this.agencyPhone_TB.Text = agent.Phone;

        }

        private void collapseExpander(object sender, RoutedEventArgs e)
        {
            Expander expdr = (Expander)sender;
            expdr.IsExpanded = false;
        }

        private void cancel_BTN_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void communityPartnershipAgreement_LBL_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CommunityPartnershipAgreementWindow form = new CommunityPartnershipAgreementWindow();
            form.ShowDialog();
        }

        private void save_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (dbMethods.CheckDatabaseConnection())
            {
                using (PubsDataContext db = new PubsDataContext())
                {
                    var CheckExists = (from s in db.Agencies
                                       where s.Name == agencyName_TB.Text
                                       select s);
                    //if the agency does not exists, application will create a new agency
                    if (CheckExists.Count() == 0)
                    {
                            agent.Name = agencyName_TB.Text;
                            //HACK DATABASE - need to add textbox for AlternateContact, description to AgencyProfile
                            //agency.AlternateContact = agencyCoordinatorName_TB;
                            agent.City = agencyAddressCity_TB.Text;
                            agent.CoordinatorName = agencyCoordinatorName_TB.Text;
                            //agency.Description;
                            agent.Email = agencyEmail_TB.Text;
                            agent.FaxNumber = agencyFax_TB.Text;
                            agent.Phone = agencyPhone_TB.Text;
	                        agent.Rating = Convert.ToInt32(agencyRating_TB.Text);
                            agent.State = agencyAddressState_TB.Text;
                            agent.StreetAddress = agencyAddressStreet_TB.Text;
                            agent.WebsiteLink = agencyWebsite_TB.Text;
                            agent.Zip = agencyAddressZipcode_TB.Text;

                            db.Agencies.InsertOnSubmit(agent);
                            db.SubmitChanges();
                    }
                    else
                    {
                        //save agency info
                        Agency agency = (from s in db.Agencies
                                        where s.Name == agent.Name
                                        select s).Single();
                        agency.Name = agencyName_TB.Text;
                        //agency.AlternateContact = agencyCoordinatorName_TB;
                        agency.City = agencyAddressCity_TB.Text;
                        agency.CoordinatorName = agencyCoordinatorName_TB.Text;
                        //agency.Description;
                        agency.Email = agencyEmail_TB.Text;
                        agency.FaxNumber = agencyFax_TB.Text;
                        agency.Phone = agencyPhone_TB.Text;
                        agency.Rating = Convert.ToInt32(agencyRating_TB.Text);
                        agency.State = agencyAddressState_TB.Text;
                        agency.StreetAddress = agencyAddressStreet_TB.Text;
                        agency.WebsiteLink = agencyWebsite_TB.Text;
                        agency.Zip = agencyAddressZipcode_TB.Text;

                        db.SubmitChanges();
                    }


                }
            }
        }
    }
}
