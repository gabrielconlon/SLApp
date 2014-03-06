using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SLApp_Beta
{
    /// <summary>
    /// Interaction logic for AgencyProfile.xaml
    /// TODO - Adjust Database, Types of servie bits are allowing nulls
    /// </summary>
    public partial class AgencyProfile : Window
    {
        DatabaseMethods dbMethods = new DatabaseMethods();
        Agency agent = new Agency();

        public AgencyProfile( bool isAdmin)
        {
            InitializeComponent();

			if (isAdmin == false)
			{
				agencyRating_TB.Visibility = Visibility.Hidden;
				agencyRating_LBL.Visibility = Visibility.Hidden;
				save_BTN.Visibility = Visibility.Hidden;
				agencyDelete_BTN.Visibility = Visibility.Hidden;
			}
        }

        public AgencyProfile(Agency agent, bool isAdmin, bool IsEdit)
        {
            InitializeComponent();

			if (isAdmin == false)
			{
				agencyRating_TB.Visibility = Visibility.Hidden;
				agencyRating_LBL.Visibility = Visibility.Hidden;
				save_BTN.Visibility = Visibility.Hidden;
				agencyDelete_BTN.Visibility = Visibility.Hidden;
			}

			if(isAdmin)

	        this.agent = agent;

            this.agencyName_TB.Text = agent.Name;
            this.agencyAlternateName_TB.Text = agent.AlternateContact;
            this.agencyEmail_TB.Text = agent.Email;
            this.agencyCoordinatorName_TB.Text = agent.CoordinatorName;
            this.agencyRating_TB.Text = agent.Rating.ToString();
            this.agencyPhone_TB.Text = agent.Phone;
            this.agencyFax_TB.Text = agent.FaxNumber;
            this.agencyAddressCity_TB.Text = agent.City;
            this.agencyAddressState_TB.Text = agent.State;
            this.agencyAddressStreet_TB.Text = agent.StreetAddress;
            this.agencyAddressZipcode_TB.Text = agent.Zip;
            this.agencyWebsite_TB.Text = agent.WebsiteLink;
	        this.description_TB.Text = agent.Description;

        }

        private void collapseExpander(object sender, RoutedEventArgs e)
        {
            Expander expdr = (Expander)sender;
            expdr.IsExpanded = false;
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
					try
					{
						if (CheckExists.Count() == 0)
						{
							agent.Name = agencyName_TB.Text;
							agent.City = agencyAddressCity_TB.Text;
							agent.CoordinatorName = agencyCoordinatorName_TB.Text;
							agent.Description = description_TB.Text;
							agent.Email = agencyEmail_TB.Text;
							agent.FaxNumber = agencyFax_TB.Text;
							agent.Phone = agencyPhone_TB.Text;
							agent.Rating = Convert.ToInt32(agencyRating_TB.Text);
							agent.State = agencyAddressState_TB.Text;
							agent.StreetAddress = agencyAddressStreet_TB.Text;
							agent.WebsiteLink = agencyWebsite_TB.Text;
							agent.Zip = agencyAddressZipcode_TB.Text;
							agent.AlternateContact = agencyAlternateName_TB.Text;

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
							agency.City = agencyAddressCity_TB.Text;
							agency.CoordinatorName = agencyCoordinatorName_TB.Text;
							agency.Description = description_TB.Text;
							agency.Email = agencyEmail_TB.Text;
							agency.FaxNumber = agencyFax_TB.Text;
							agency.Phone = agencyPhone_TB.Text;
							agency.Rating = Convert.ToInt32(agencyRating_TB.Text);
							agency.State = agencyAddressState_TB.Text;
							agency.StreetAddress = agencyAddressStreet_TB.Text;
							agency.WebsiteLink = agencyWebsite_TB.Text;
							agency.Zip = agencyAddressZipcode_TB.Text;
							agency.AlternateContact = agencyAlternateName_TB.Text;

							db.SubmitChanges();
						}
					}
					catch (Exception)
					{
						MessageBox.Show("SLApp apologies for the inconvenience, but at this time Rating must contain contain data.",
						                "Save Error!", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

        private void cancel_BTN_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

		private void agencyDelete_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				if (MessageBox.Show("Are you sure you want to delete this agency?", "Confirm Delete!", MessageBoxButton.YesNo) ==
					MessageBoxResult.Yes)
				{
					using (PubsDataContext db = new PubsDataContext())
					{
						Agency stud = (from s in db.Agencies
										where s.Name == agent.Name
										select s).Single();
						
						db.Agencies.DeleteOnSubmit(stud);
						db.SubmitChanges();
						this.Close();
					}
				}
			}
		}

        private void communityPartnershipAgreement_LBL_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CommunityPartnershipAgreementWindow form = new CommunityPartnershipAgreementWindow();
            form.ShowDialog();
		}

		#region Service Opportunities

		private void LongTerm_Expander_OnExpanded(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{

					var completionList = new List<ServiceOpportunity>(from s in db.Types_of_Services
																	   where s.Agency == agent.Name
																	   select new ServiceOpportunity(s.Agency, "Long Term", s.Title, s.Body));
					if (!completionList.Any())
					{
						ServiceOpportunity exp = new ServiceOpportunity();
						exp.Agency = agent.Name;
						db.Types_of_Services.InsertOnSubmit(new Types_of_Service());
						db.SubmitChanges();
						completionList.Add(exp);
					}
					longTerm_DataGrid.DataContext = completionList;

				}
			}
		}

		private void AgencyLongTermServiceSave_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			Types_of_Service expROW = longTerm_DataGrid.SelectedItem as Types_of_Service;
				if (dbMethods.CheckDatabaseConnection())
				{
					using (PubsDataContext db = new PubsDataContext())
					{
						var completionList = new List<Types_of_Service>(from s in db.Types_of_Services
																		   where s.Agency == expROW.Agency
																		   select s);
						if (completionList.Count > 0)
						{

							var completion = completionList.First();
							completion.Agency = expROW.Agency;
							completion.Body = expROW.Body;
							completion.CommunityBasedResearch = expROW.CommunityBasedResearch;
							completion.LongTerm = expROW.LongTerm;
							completion.ShortTerm = expROW.ShortTerm;
							completion.Title = expROW.Title;

							db.SubmitChanges();
							LongTerm_Expander_OnExpanded(sender, e);
						}
						else
						{
							Types_of_Service exp = new Types_of_Service();

							exp.Agency = agent.Name;
							exp.Body = expROW.Body;
							exp.CommunityBasedResearch = expROW.CommunityBasedResearch;
							exp.LongTerm = expROW.LongTerm;
							exp.ShortTerm = expROW.ShortTerm;
							exp.Title = expROW.Title;

							db.Types_of_Services.InsertOnSubmit(exp);
							db.SubmitChanges();
							LongTerm_Expander_OnExpanded(sender, e);
						}
					}
				}
		}

		#endregion

		
    }
}
