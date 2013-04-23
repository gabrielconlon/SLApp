using System;
using System.Windows;
using System.Data.OleDb;

namespace SLApp_Beta
{

    ///code to build and run if Debug mode
    ///TODO: code to be run in debug mode
    ///HACK ASK PETE, is this better than using a unit test?
#if (DEBUG)
    public class Conditionals
    {
        private bool mustbe(bool condition1, bool condition2)
        {
            if (condition1 = condition2)
                return true;
            else
                return false;
        }
    }
#else
#endif

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Application State Variables

		/// <summary>
		/// this bool will be used to determine the state of the application
		/// if someone other than an admin (i.e. student worker) is signed in
		/// then all notes fields, and any other fields deemed FERPA protected
		/// will have IsEnabled = false
        /// 
        /// FERPA Fields currently include:
        /// -Notes
		/// </summary>
		private bool IsAdmin;

		#endregion

		public MainWindow()
		{
            ///HACK ASK PETE if this is appropriate or if there is a better way
            /// Attempt Db connection, catch all exceptions and handle with a Dialog Window to user
            ///http://msdn.microsoft.com/en-us/library/aa969773.aspx messagebox info
            ///http://msdn.microsoft.com/en-us/library/bb386876.aspx OLE DB info
            ///http://msdn.microsoft.com/en-us/library/bb399398.aspx more OLE DB info
            ///http://msdn.microsoft.com/en-us/library/aa288452%28v=vs.71%29.aspx OLE DB tutorial from MS
            try
            {
                string stConnect = "Proivder=sqloledb; Data Source=cs1;" + "Initial Catalog=SLDatabase; Integrated Security=SSPI;";
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = new OleDbConnection(stConnect);
                cmd.Connection.Open();
            }
            catch (Exception ex)
            {
                string messageBoxText = "The database could not be opened.  This application cannot load any information without a database connection."+ 
                    "\n\nContinue anyways?";
                string caption = "Database Error";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;

                //show a dialog indicating the database has not opened successfully
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                //handles the user input
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        //user pressed yes, wants to open App anyways
                        
                        break;
                    case MessageBoxResult.No:
                        this.Close();
                        break;
                }
            }
            ///TODO: create a login for the app, so the database can be
            ///accessed without a user ID (e.g. Ross or student worker putting in a university login)
            ///


			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            ///TODO: setup for a check in the login window
            ///will need to check against the database, so the database must
            ///be open and a 'users' table exists with the App login present
			this.IsEnabled = false;
			LoginWindow lgn = new LoginWindow(IsAdmin);
			lgn.Closed += new EventHandler(lgn_Closed);
			lgn.ShowDialog();
		}

		private void lgn_Closed(object sender, EventArgs e)
		{
			this.IsEnabled = true;
		}

		private void menuCreateStudent_Click (object sender, RoutedEventArgs e)
		{
			StudentProfile Studentform = new StudentProfile(IsAdmin);
			Studentform.Show();
		}

        private void menuCreateAgency_Click(object sender, RoutedEventArgs e)
        {
            AgencyProfile Agencyform = new AgencyProfile();
            Agencyform.Show();
        }

        private void newStudentProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile form = new StudentProfile(IsAdmin);
            form.Show();
        }

        private void studentSearch_BTN_Click(object sender, RoutedEventArgs e)
        {
            ///TODO: Present a way for the prototype to populate some basic data 
            ///which can be clicked to show what a student profile window will look like
            ///

            /// <summary>
            ///Data grid based off of which fields have data entered into them,
            ///if a box has no txt entered then it is not included in the query generation
            ///
            /// Fields are: studentID_TB, studentLastName_TB, studentMiddleName_TB, graduatinoYear_TB
            /// HACK FIX THIS CODE: all matching needs to be reset to match the database (i.e. stud.First_Name)
            /// HACK BUG: Need to properly link the database so the SQL works
            /// </summary>
            studentSearch_DataGrid.DataContext = (from stud in student
                                                  where (studentID_TB.Text.Length == 0 || studentID_TB.Text == stud.First_Name) &&
                                                  (studentLastName_TB.Text.Length == 0 || studentLastName_TB.Text == stud.Last_Name) &&
                                                  (graduationYear_TB.Text.Length == 0 || graduationYear_TB.Text == stud.graduation_Year)

                                                  select new
                                                  {
                                                      /// <summary>
                                                      ///HACK Fill in the select to match the database,
                                                      ///this involves returning more information than was originally put in
                                                      /// </summary>
                                                      Name = String.Format("{0} {1}", stud.First_Name, stud.Last_Name),
                                                  });
                                                      
            
        }

        #region Search Button Examples from Athletic Recruitement App
        //private void AthleteSearch_BTN_Click(object sender, EventArgs e)
        //{
        //    DateTime contact;

        //    if (txtDateLastContacted.Text.Length > 0)
        //    {
        //        contact = Convert.ToDateTime(txtDateLastContacted.Text);      //if there is text in the date last contacted field, convert it to type DateTime (need to add data validation)
        //    }
        //    else
        //    {
        //        contact = DateTime.MinValue;                                  //if not date inputed then set date to minimum value
        //    }

        //    Athlete_dataview.DataSource = (from a in database.Athletes
        //                                   from c in database.Coaches
        //                                   from ev in database.Events
        //                                   from ae in database.Ath_Events
        //                                   where (txtFName.Text.Length == 0 || txtFName.Text == a.First_Name) &&
        //                                         (txtLName.Text.Length == 0 || txtLName.Text == a.Last_Name) &&
        //                                         (txtHighSchool.Text.Length == 0 || txtHighSchool.Text == a.High_School) &&
        //                                         (txtCity.Text.Length == 0 || txtCity.Text == a.City) &&
        //                                         (txtState.Text.Length == 0 || txtState.Text == a.State) &&
        //                                         (ddSkillRating.Text.Length == 0 || ddSkillRating.Text == a.Skill_Rating) &&
        //                                         (ddClass.Text.Length == 0 || ddClass.Text == a.Class) &&
        //                                         (ddPrimaryWhitworthCoach.Text.Length == 0 || ddPrimaryWhitworthCoach.Text == c.Last_Name) &&
        //                                         (ddEvent.Text.Length == 0 || ddEvent.Text == ev.Event_Name) &&
        //                                         (a.Coach_id == c.Coach_id) &&
        //                                         (ev.Event_id == ae.Event_Id) &&
        //                                         (ae.Athlete_Id == a.Athlete_id) &&
        //                                         (txtDateLastContacted.Text.Length == 0 || contact >= a.Date_Last_Contacted) //our user wants to query for all athletes who have not been contacted
        //                                   //since the inputed date.
        //                                   //the or statements make it so that the user can choose what he wants to query over. If he leaves a textbox blank, then the first part of the or
        //                                   //will return true. If text is entered in the textbox, then we want to query for results that match the inputed text.

        //                                   select new //select clause to output the desired data
        //                                   {
        //                                       Name = String.Format("{0} {1}", a.First_Name, a.Last_Name), //combine first and last name into one Name column
        //                                       Skill_Rating = a.Skill_Rating,
        //                                       Date_Last_Contacted = a.Date_Last_Contacted,
        //                                       Class = a.Class,
        //                                       High_School = a.High_School,
        //                                       High_School_Coach = a.High_School_Coach,
        //                                       Address = String.Format("{0} {1}, {2}", a.Address, a.City, a.State),
        //                                       Phone = a.Phone_Number,
        //                                       Email = a.Email,
        //                                       Height = a.Height,
        //                                       Weight = a.Weight,
        //                                       Whitworth_Coach = c.Last_Name,
        //                                   }).Distinct(); //without asking for only Distinct athletes, those athletes who are listed in the Ath_Event table for multiple events will 
        //    //be displayed by this query multiple times

        //}

        //private void N_search_b_Click(object sender, EventArgs e)
        //{
        //    Note_dataview.DataSource = (from a in database.Athletes
        //                                from c in database.Coaches
        //                                from nt in database.Note_Types
        //                                from rn in database.Recruit_Notes
        //                                where (N_txtFname.Text.Length == 0 || N_txtFname.Text == a.First_Name) &&
        //                                      (N_txtLname.Text.Length == 0 || N_txtLname.Text == a.Last_Name) &&
        //                                      (N_ddNoteType.Text.Length == 0 || N_ddNoteType.Text == nt.Note_Type_Name) &&
        //                                      (c.Coach_id == rn.Coach_id) &&
        //                                      (a.Athlete_id == rn.Athlete_id) &&
        //                                      (nt.Note_Type_id == rn.Note_type_id) //&&
        //                                //(a.Coach_id == c.Coach_id)
        //                                //similar to the athlete query, we want the user to be able to enter whichever field he wants, and leave others blank
        //                                select new
        //                                {
        //                                    Name = String.Format("{0} {1}", a.First_Name, a.Last_Name),
        //                                    Note_Type = nt.Note_Type_Name,
        //                                    Date_of_Note = rn.Date_of_Note,
        //                                    Note = rn.Note,
        //                                    Written_By = c.Last_Name,
        //                                }).Distinct();
        //}
        #endregion

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

	}
}
