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
        public AgencyProfile()
        {
            InitializeComponent();
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
    }
}
