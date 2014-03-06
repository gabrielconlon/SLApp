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
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            this.textBox1.Text = "Severice Learning Application (SLApp)\n" +
				"Version 1.0 - Beta" + "\n© 2013\n\n" +
				"This application has been created and maintained by the Computer Science students of Whitworth University.";
        }
    }
}
