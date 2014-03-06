using System.Windows;

namespace SLApp_Beta
{
    /// <summary>
    /// Interaction logic for CommunityPartnershipAgreementWindow.xaml
    /// </summary>
    public partial class CommunityPartnershipAgreementWindow : Window
    {
		DatabaseMethods dbMethods = new DatabaseMethods();

        public CommunityPartnershipAgreementWindow()
        {
            InitializeComponent();
        }

		private void save_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using(PubsDataContext db = new PubsDataContext())
					{
						
					}
			}
		}
    }
}
