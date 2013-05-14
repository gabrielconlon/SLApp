using System;
using System.Windows;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;

using System.Collections;
using System.Collections.Generic;

namespace SLApp_Beta
{

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

		private DatabaseMethods dbMethods = new DatabaseMethods();
		

		public MainWindow(bool isAdmin)
		{
            ///http://msdn.microsoft.com/en-us/library/aa969773.aspx messagebox info
            ///http://msdn.microsoft.com/en-us/library/bb386876.aspx OLE DB info
            ///http://msdn.microsoft.com/en-us/library/bb399398.aspx more OLE DB info
            ///http://msdn.microsoft.com/en-us/library/aa288452%28v=vs.71%29.aspx OLE DB tutorial from MS
			/// http://www.codeproject.com/Tips/362436/Data-binding-in-WPF-DataGrid-control datagrid info and binding
            ///

			
			InitializeComponent();
            IsAdmin = isAdmin;
            if(!isAdmin)
            {
                admin_tab.IsEnabled = false;
            }
			DatabaseMethods dbMethods = new DatabaseMethods();
            LoadUsers(users_DataGrid);

		}

        #region Menu

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void manualMenuItem_Click(object sender, RoutedEventArgs e)
        {
             UserDocumentation manual = new UserDocumentation();
			manual.Show();
        }

        private void menuCreateStudent_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile Studentform = new StudentProfile(IsAdmin);
			Studentform.Closed += new EventHandler((s0, e0) => { studentSearch_BTN_Click(s0, null); });
            Studentform.Show();
        }

		private void menuCreateAgency_Click(object sender, RoutedEventArgs e)
		{
			AgencyProfile Agencyform = new AgencyProfile();
			Agencyform.Show();
		}

        private void logoffMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow lgn = new LoginWindow();
            lgn.Show();
            this.Close();

        }

        #endregion

        #region Student Tab

        private void newStudentProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile form = new StudentProfile(IsAdmin);
			form.Closed += new EventHandler((s0, e0) => studentSearch_BTN_Click(s0, null));
            form.Show();
        }

        private void studentSearch_BTN_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            ///Data grid based off of which fields have data entered into them,
            ///if a box has no txt entered then it is not included in the query generation
            ///
            /// Fields are: studentID_TB, studentLastName_TB, studentMiddleName_TB, graduatinoYear_TB
            /// </summary>
            /// 
			if(dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
                    //HACK BUG - for some reason user profiles are lost if the last learning experience is deleted.
                    var allStudents = (//HACK this makes the experience appear, not the student
                                       //from stud in db.Students
                                       //join experiences in db.Learning_Experiences on stud.Student_ID equals experiences.Student_ID into experience
                                       //from item in experience.DefaultIfEmpty()
                                       from experience in db.Learning_Experiences
                                       join students in db.Students on experience.Student_ID equals students.Student_ID into studs
                                       from stud in studs.DefaultIfEmpty()

                                       //TODO: almost works, the join forces students to come up who have a course that matches an experience
                                       ///HACK got this working by searcing the course number from the experiences table do we actually need a "course" table?
                                       ///add section and professor to the experiences table? combine again?
                                       //join course in db.Courses on experience.CourseNumber equals course.CourseNumber

                                       where
                                           //student search section
                                           //NOW ABLE TO SEARCH FOR PARTIALS, accept for the ints
                                           (studentFirstName_TB.Text.Length == 0 || stud.FirstName.Contains(studentFirstName_TB.Text)) && //studentFirstName_TB.Text == stud.FirstName
                                           (studentLastName_TB.Text.Length == 0 || stud.LastName.Contains(studentLastName_TB.Text)) && //studentLastName_TB.Text == stud.LastName
                                           (studentID_TB.Text.Length == 0 || stud.Student_ID.ToString().Contains(studentID_TB.Text)) && //== stud.Student_ID.ToString()) &&
                                           (graduationYear_TB.Text.Length == 0 || stud.GraduationYear.ToString().Contains(graduationYear_TB.Text)) &&

                                           //course search section
                                           (course_TB.Text.Length == 0 || experience.CourseNumber.Contains(course_TB.Text)) &&
                                           (professor_TB.Text.Length == 0 || experience.Professor.Contains(professor_TB.Text)) &&
										   (CourseName_TB.Text.Length == 0 || experience.CourseName.Contains(CourseName_TB.Text)) &&
										   (Section_TB.Text.Length == 0 || experience.Section.ToString().Contains(Section_TB.Text)) &&

                                           //service and hours section
                                           (serviceType_CBX.Text.Length == 0 || experience.TypeofLearning.Contains(serviceType_CBX.Text)) && //serviceType_CBX.Text == experience.TypeofLearning
                                           (semester_CBX.Text.Length == 0 || experience.Semester.Contains(semester_CBX.Text)) && //semester_CBX.Text == experience.Semester
                                           (year_TB.Text.Length == 0 || experience.Year.ToString().Contains(year_TB.Text))

                                       select stud).Distinct();
					studentSearch_DataGrid.DataContext = allStudents;
				}
			}
        }

		/// <summary>
		/// http://stackoverflow.com/questions/11070873/how-to-get-value-of-a-cell-from-datagrid-in-wpf
		/// http://stackoverflow.com/questions/5549321/how-to-read-value-from-a-cell-from-a-wpf-datagrid
		/// http://www.scottlogic.co.uk/blog/colin/2008/12/wpf-datagrid-detecting-clicked-cell-and-row/
		/// http://www.codeproject.com/Articles/24192/Simple-Demo-of-Binding-to-a-Database-in-WPF-using
		/// http://www.codeproject.com/Articles/46422/A-LINQ-Tutorial-Adding-Updating-Deleting-Data
		/// http://stackoverflow.com/questions/3913580/get-selected-row-item-in-datagrid-wpf
		/// http://www.youtube.com/watch?v=LCfvcBObX8k
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void StudentSearch_DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			using (PubsDataContext datab = new PubsDataContext())
			{
				Student studentRow = studentSearch_DataGrid.SelectedItem as Student;
				Student stud = (from s in datab.Students
				                where s.Student_ID == studentRow.Student_ID
				                select s).Single();

				StudentProfile studentForm = new StudentProfile(stud, IsAdmin, true);
				studentForm.Closed += new EventHandler((s0, e0) => studentSearch_BTN_Click(s0, null));
				studentForm.Show();
			}
		}

		private void clear_BTN_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < grdStudentTab.Children.Count; i++)
			{
				UIElement u = grdStudentTab.Children[i];
				if (u is TextBox)
					((TextBox)u).Clear();
				else if (u is ComboBox)
					((ComboBox)u).Text = "";
			}
		}

        #endregion


        #region Agency Tab

        private void agencySearch_BTN_Click(object sender, RoutedEventArgs e)
        {
            using (PubsDataContext db = new PubsDataContext())
                  
            
            
            {
                var allAgency = (from agency in db.Agencies
                                 where (agencyName_TB.Text.Length == 0 || agency.Name.Contains(agencyName_TB.Text)) &&
                                 (agencyPhone_TB.Text.Length == 0 || agency.Phone.Contains(agencyPhone_TB.Text)) &&
                                 (agencyFax_TB.Text.Length == 0 || agency.FaxNumber.Contains(agencyFax_TB.Text)) &&
                                 (agencyRating_TB.Text.Length == 0||agency.Rating.ToString().Contains(agencyRating_TB.Text)) &&
                                 (agencyWebsite_TB.Text.Length == 0 || agency.WebsiteLink.Contains(agencyWebsite_TB.Text)) &&
                                 (agencyCoordinatorName_TB.Text.Length == 0 || agency.CoordinatorName.Contains(agencyCoordinatorName_TB.Text)) &&
                                 (agencyAddressStreet_TB.Text.Length == 0 || agency.StreetAddress.Contains(agencyAddressStreet_TB.Text)) &&
                                 (agencyAddressCity_TB.Text.Length == 0 || agency.City.Contains(agencyAddressCity_TB.Text)) &&
                                 (agencyAddressState_TB.Text.Length == 0 || agency.State.Contains(agencyAddressState_TB.Text)) &&
                                 (agencyAddressZipcode_TB.Text.Length == 0 || agency.Zip.Contains(agencyAddressZipcode_TB.Text))

                                 select agency);
                agencySearch_DataGrid.DataContext = allAgency;
            }
        }

        private void newAgencyProfile_BTN_Click(object sender, RoutedEventArgs e)
        {
            AgencyProfile Agencyform = new AgencyProfile();
            Agencyform.Show();
        }

		private void AgencySearch_DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			using (PubsDataContext datab = new PubsDataContext())
			{
				Agency agencyRow = agencySearch_DataGrid.SelectedItem as Agency;
				Agency agent = (from s in datab.Agencies
								where s.Name == agencyRow.Name
								select s).Single();

				AgencyProfile agentForm = new AgencyProfile(agent, IsAdmin, true);
				agentForm.Closed += new EventHandler((s0, e0) => agencySearch_BTN_Click(s0, null));
				agentForm.Show();
			}
		}

        #endregion

        #region Admin Tab

        //private void admin_tab_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    LoadUsers(users_DataGrid);
        //}

        private void LoadUsers(DataGrid dg)
        {
            if (dbMethods.CheckDatabaseConnection())
            {
                using (PubsDataContext db = new PubsDataContext())
                {
                    var Allusers = new List<Application_User>(from users in db.Application_Users
                                                              select users);
                    dg.DataContext = Allusers;
                }
            }
        }

        private void saveUser_BTN_Click(object sender, RoutedEventArgs e)
        {
            Application_User userROW = users_DataGrid.SelectedItem as Application_User;
            if (userROW != null)
            {
                if (dbMethods.CheckDatabaseConnection())
                {
                    using (PubsDataContext db = new PubsDataContext())
                    {
                        var completionList = new List<Application_User>(from s in db.Application_Users
                                                                           where s.Username == userROW.Username
                                                                           select s);
                        if (completionList.Count > 0)
                        {

                            var completion = completionList.First();
                            completion.Username = userROW.Username;
                            completion.Password = userROW.Password;
                            completion.LastName = userROW.LastName;
                            completion.IsAdmin = userROW.IsAdmin;
                            completion.FirstName = userROW.FirstName;

                            db.SubmitChanges();
                            LoadUsers(users_DataGrid);
                        }
                        else
                        {
                            Application_User exp = new Application_User();

                            exp.Username = userROW.Username;
                            exp.Password = userROW.Password;
                            exp.LastName = userROW.LastName;
                            exp.IsAdmin = userROW.IsAdmin;
                            exp.FirstName = userROW.FirstName;

                            db.Application_Users.InsertOnSubmit(exp);
                            db.SubmitChanges();
                            LoadUsers(users_DataGrid);
                        }
                    }
                }
            }
        }

        private void deleteUser_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (dbMethods.CheckDatabaseConnection())
            {
                if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete!", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    using (PubsDataContext db = new PubsDataContext())
                    {
                        Application_User expROW = users_DataGrid.SelectedItem as Application_User;

                        var completionList = new List<Application_User>(from s in db.Application_Users
                                                                        where s.Username == expROW.Username
                                                                        select s);
                        if (expROW != null && completionList.Any())
                        {
                            var completion = completionList.First();
                            db.Application_Users.DeleteOnSubmit(completion);
                            db.SubmitChanges();
                            LoadUsers(users_DataGrid);
                        }
                        else
                        {
                            LoadUsers(users_DataGrid);
                        }
                    }
                }
            }
        }

        #endregion

        #region ContextMenu

        private void Add_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            StudentProfile form = new StudentProfile(IsAdmin);
            form.Show();
        }

        private void Edit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (PubsDataContext datab = new PubsDataContext())
            {
                Student studentRow = studentSearch_DataGrid.SelectedItem as Student;
                Student stud = (from s in datab.Students
                                where s.Student_ID == studentRow.Student_ID
                                select s).Single();

                StudentProfile studentForm = new StudentProfile(stud, IsAdmin, true);
                studentForm.Show();
            }
        }

        private void Delete_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dbMethods.CheckDatabaseConnection())
            {
                if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete!", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    
                    using (PubsDataContext db = new PubsDataContext())
                    {
                        Student studentRow = studentSearch_DataGrid.SelectedItem as Student;
                        Student stud = (from s in db.Students
                                        where s.Student_ID == studentRow.Student_ID
                                        select s).Single();
                        Learning_Experience exp = (from ex in db.Learning_Experiences
                                                   where ex.Student_ID == stud.Student_ID
                                                   select ex).Single();
                        db.Students.DeleteOnSubmit(stud);
                        db.Learning_Experiences.DeleteOnSubmit(exp);
                        db.SubmitChanges();
                        this.Close();
                    }
                }
            }
        }

        #endregion

        

        


    }
}
