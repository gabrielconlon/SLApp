using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
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

		DatabaseMethods  dbMethods = new DatabaseMethods();
		

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
				agencyRating_LBL.Visibility = Visibility.Hidden;
				agencyRating_TB.Visibility = Visibility.Hidden;
	            newAgencyProfile_BTN.IsEnabled = false;
	            newAgencyProfile_BTN.Visibility = Visibility.Hidden;
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
			AgencyProfile Agencyform = new AgencyProfile(IsAdmin);
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
                    var allStudents = (from stud in db.Students
                                       from experience in db.Learning_Experiences.Where(a => a.Student_ID == stud.Student_ID).DefaultIfEmpty()

                                       where
                                           //student search section
                                           //ABLE TO SEARCH FOR PARTIALS
                                           (studentFirstName_TB.Text.Length == 0 || stud.FirstName.Contains(studentFirstName_TB.Text)) && 
                                           (studentLastName_TB.Text.Length == 0 || stud.LastName.Contains(studentLastName_TB.Text)) && 
                                           (studentID_TB.Text.Length == 0 || stud.Student_ID.ToString().Contains(studentID_TB.Text)) && 
                                           (graduationYear_TB.Text.Length == 0 || stud.GraduationYear.ToString().Contains(graduationYear_TB.Text)) &&

                                           //course search section
                                           (course_TB.Text.Length == 0 || experience.CourseNumber.Contains(course_TB.Text)) &&
                                           (professor_TB.Text.Length == 0 || experience.Professor.Contains(professor_TB.Text)) &&
										   (CourseName_TB.Text.Length == 0 || experience.CourseName.Contains(CourseName_TB.Text)) &&
										   (Section_TB.Text.Length == 0 || experience.Section.ToString().Contains(Section_TB.Text)) &&

                                           //service and hours section
                                           (serviceType_CBX.Text.Length == 0 || experience.TypeofLearning.Contains(serviceType_CBX.Text)) && 
                                           (semester_CBX.Text.Length == 0 || experience.Semester.Contains(semester_CBX.Text)) && 
                                           (year_TB.Text.Length == 0 || experience.Year.ToString().Contains(year_TB.Text))

                                       select stud).Distinct();
					studentSearch_DataGrid.DataContext = allStudents;
				}
			}
        }

		private void StudentSearch_DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			using (PubsDataContext datab = new PubsDataContext())
			{
				Student studentRow = studentSearch_DataGrid.SelectedItem as Student;
				if (studentRow != null)
				{
					Student stud = (from s in datab.Students
					                where s.Student_ID == studentRow.Student_ID
					                select s).Single();

					StudentProfile studentForm = new StudentProfile(stud, IsAdmin, true);
					studentForm.Closed += new EventHandler((s0, e0) => studentSearch_BTN_Click(s0, null));
					studentForm.Show();
				}
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

		#region ContextMenu

		private void Add_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			StudentProfile form = new StudentProfile(IsAdmin);
			form.Show();
		}

		private void Edit_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
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
					}
				}
			}
			studentSearch_BTN_Click(sender, e);
		}

		#endregion

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
			AgencyProfile Agencyform = new AgencyProfile(IsAdmin);
			Agencyform.Closed += new EventHandler((s0, e0) => agencySearch_BTN_Click(s0, null));
            Agencyform.Show();
        }

		private void AgencySearch_DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			using (PubsDataContext datab = new PubsDataContext())
			{
				Agency agencyRow = agencySearch_DataGrid.SelectedItem as Agency;
				if (agencyRow != null)
				{
					Agency agent = (from s in datab.Agencies
					                where s.Name == agencyRow.Name
					                select s).Single();

					AgencyProfile agentForm = new AgencyProfile(agent, IsAdmin, true);
					agentForm.Closed += new EventHandler((s0, e0) => agencySearch_BTN_Click(s0, null));
					agentForm.Show();
				}
			}
		}

        private void agency_clear_BTN_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < grdAgencyTab.Children.Count; i++)
            {
                UIElement u = grdAgencyTab.Children[i];
                if (u is TextBox)
                    ((TextBox)u).Clear();
                else if (u is ComboBox)
                    ((ComboBox)u).Text = "";
            }
        }

		#region context menu

		private void Add_agency_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			//newAgencyProfile_BTN_Click(sender, e);
		}

		private void Delete_agency_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				if (MessageBox.Show("Are you sure you want to delete this agency?", "Confirm Delete!", MessageBoxButton.YesNo) ==
					MessageBoxResult.Yes)
				{

					using (PubsDataContext db = new PubsDataContext())
					{
						Agency agentRow = agencySearch_DataGrid.SelectedItem as Agency;
						Agency stud = (from a in db.Agencies
									   where a.Name == agentRow.Name
									   select a).Single();

						db.Agencies.DeleteOnSubmit(stud);
						db.SubmitChanges();
					}
					agencySearch_BTN_Click(sender, e);
				}
			}
		}

		private void Edit_agency_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					Agency agentRow = agencySearch_DataGrid.SelectedItem as Agency;
					Agency stud = (from a in db.Agencies
								   where a.Name == agentRow.Name
								   select a).Single();

					AgencyProfile agentForm = new AgencyProfile(stud, IsAdmin, true);
					agentForm.Show();
				}
			}
		}

		#endregion

		#endregion

		#region Admin Tab

        private void LoadUsers(DataGrid dg)
        {
            if (dbMethods.CheckDatabaseConnection())
            {
                using (PubsDataContext db = new PubsDataContext())
                {
#if Demo
                    var Allusers = new List<Application_User>(from users in db.Application_Users
															  where users.Username != "bwatts"
                                                              select users);
#else
					var Allusers = new List<Application_User>(from users in db.Application_Users
                                                              select users);
#endif


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

		#region QUERIES

		private void RunQuery_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			Mouse.SetCursor(Cursors.Wait);
			TotalStudents();
			UnduplicatedStudents();
			TotalHours();
			UnduplicatedHours();
			AvgHoursPerStudent();
			CourseCount();
			StudentsPerClass();
			CoursesByType();
			Mouse.SetCursor(Cursors.Arrow);
		}

		private void TotalStudents()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var totalstudents = new List<Student>(from s in db.Students
														  from e in db.Learning_Experiences
														  where e.Student_ID == s.Student_ID &&
														  (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
														  (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
					                                  select s);
					totalStudents_TB.Text = totalstudents.Count().ToString();
				}
			}
		}

		private void UnduplicatedStudents()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var totalstudents = new List<Student>(from s in db.Students
														  from e in db.Learning_Experiences
														  where e.Student_ID == s.Student_ID &&
														  (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
														  (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
														  select s).Distinct();
					unduplicatedStudents_TB.Text = totalstudents.Count().ToString();
				}
			}
		}

		private void TotalHours()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var totalhours = new List<Learning_Experience>(from e in db.Learning_Experiences
																   from s in db.Students
																   where s.Student_ID == e.Student_ID &&
																   (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
																   (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
																   select e);

					totalHours_TB.Text = totalhours.Sum(i => i.TotalHours.GetValueOrDefault(0)).ToString();
				}
			}
		}

		private void UnduplicatedHours()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var totalhours = new List<Learning_Experience>(from e in db.Learning_Experiences
																   from s in db.Students
																   where s.Student_ID == e.Student_ID &&
																   (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
																   (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
																   select e).GroupBy(s => s.Student_ID).Select(e => e.MaxBy(x => x.TotalHours));

						unduplicatedHours_TB.Text = totalhours.Sum(i => i.TotalHours.GetValueOrDefault(0)).ToString();
				}
			}
		}

		private void AvgHoursPerStudent()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var totalhours = new List<Learning_Experience>(from e in db.Learning_Experiences
																   where
																   (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
																   (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
																   select e);
					var totalstudents = new List<Student>(from s in db.Students
														  from e in db.Learning_Experiences
														  where e.Student_ID == s.Student_ID &&
														  (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
														  (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
														  select s);
					double avg_hours;
					avg_hours = (Convert.ToDouble(totalhours.Sum(i => i.TotalHours.GetValueOrDefault(0))) / Convert.ToDouble(totalstudents.Count()));
					avgHoursStudent_TB.Text = (Math.Round(avg_hours, 3)).ToString();
				}
			}
		}

		private void CourseCount()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var allcourses = new List<Learning_Experience>(from e in db.Learning_Experiences
																   where
														  (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
														  (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
															   select e).GroupBy(x => String.Format("{0}-{1}", x.CourseNumber, x.Section)).Distinct();

					courseCount_TB.Text = allcourses.Count().ToString();
				}
			}
		}

		private void StudentsPerClass()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var grps = from s in db.Students
					            from e in db.Learning_Experiences
					            where s.Student_ID == e.Student_ID &&
					                  (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
					                  (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
					            group e by new {e.CourseNumber, e.Section} into grp select new { Class = grp.Key.CourseNumber, Section = grp.Key.Section, Count = grp.Select(x => x.Student_ID).Distinct().Count() };
					dataGrid2.DataContext = grps;
				}
			}
		}

		private void CoursesByType()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var courses = (from e in db.Learning_Experiences
								   where (queryYear_TB.Text.Length == 0 || e.Year.ToString() == queryYear_TB.Text) &&
														  (querySemester_ComboBox.Text.Length == 0 || e.Semester == querySemester_ComboBox.Text)
								   group e by e.TypeofLearning into grp
								   select new {Type = grp.Key, Count = grp.Select(x => x.Student_ID).Distinct().Count()} );
					coursesByType_Datagrid.DataContext = courses;
				}
			}
		}


		#endregion


		private void studentSearch_DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
#if Demo
			if (e.PropertyName == "Student_ID") e.Cancel = true;
#endif
		}



		#endregion



        

        


    }
}
