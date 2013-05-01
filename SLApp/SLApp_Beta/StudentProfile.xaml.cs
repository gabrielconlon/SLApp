using System;
using System.Collections;
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
	/// Interaction logic for CreateStudentProfile.xaml
	/// </summary>
	public partial class StudentProfile : Window
	{
		private Student student = new Student();
		private bool isEdit;
		DatabaseMethods dbMethods = new DatabaseMethods();

        private double myWidth;
        private double myHeight;

		public StudentProfile(bool isAdmin)
		{
			InitializeComponent();
            if (isAdmin == false)
            {
                studentNotes_DataGrid.IsEnabled = false;
            }
		}

		public StudentProfile(Student stud, bool isAdmin, bool IsEdit)
		{
			InitializeComponent();
			if (isAdmin == false)
			{
				studentNotes_DataGrid.IsEnabled = false;
			}
			student = stud;
			isEdit = IsEdit;
			

			this.studentFirstName_TB.Text = stud.FirstName;
			this.studentLastName_TB.Text = stud.LastName;
			this.studentID_TB.Text = stud.Student_ID.ToString();
			this.studentemail_TB.Text = stud.Email;
			this.graduationYear_TB.Text = stud.GraduationYear.ToString();


			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
					                                  where s.Student_ID == student.Student_ID
					                                  select s);
					studentLearningExperiences_DataGrid.DataContext = completionList;
				}
			}
		}

		public void LoadStudentLearningExperiences()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
																	   where s.Student_ID == student.Student_ID
																	   select s);
					studentLearningExperiences_DataGrid.DataContext = completionList;
				}
			}
		}

        public bool areStudentNotesLocked()
        {
            if (studentNotes_DataGrid.IsEnabled)
                return true;
            else
                return false;
        }

      

        private void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

		public IEnumerable<System.Windows.Controls.DataGridRow> GetDataGridRows(System.Windows.Controls.DataGrid grid)
		{
			var itemsSource = grid.ItemsSource as IEnumerable;
			if (null == itemsSource) yield return null;
			foreach (var item in itemsSource)
			{
				var row = grid.ItemContainerGenerator.ContainerFromItem(item) as System.Windows.Controls.DataGridRow;
				if (null != row) yield return row;
			}
		}

        private void save_BTN_Click(object sender, RoutedEventArgs e)
        {
	        if (dbMethods.CheckDatabaseConnection())
	        {
		        using (PubsDataContext db = new PubsDataContext())
		        {
			        if (!isEdit)
			        {
				        student.Student_ID = Convert.ToInt32(studentID_TB.Text);
				        student.FirstName = studentFirstName_TB.Text;
				        student.LastName = studentLastName_TB.Text;
				        student.GraduationYear = Convert.ToInt32(graduationYear_TB.Text);
				        student.Email = studentemail_TB.Text;


				        Learning_Experience exp = new Learning_Experience();
				        exp.Student_ID = Convert.ToInt32(studentID_TB.Text);
				        db.Students.InsertOnSubmit(student);
				        db.Learning_Experiences.InsertOnSubmit(exp);
				        db.SubmitChanges();
			        }
			        else
			        {
				        Student stud = (from s in db.Students
				                        where s.Student_ID == student.Student_ID
				                        select s).Single();
				        stud.Student_ID = Convert.ToInt32(studentID_TB.Text);
				        stud.FirstName = studentFirstName_TB.Text;
				        stud.LastName = studentLastName_TB.Text;
				        stud.GraduationYear = Convert.ToInt32(graduationYear_TB.Text);
				        stud.Email = studentemail_TB.Text;

						//Learning_Experience exp = (from s in db.Learning_Experiences
						//                           where s.Student_ID == student.Student_ID
						//                           select s).Single();
						var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
																		   where s.Student_ID == student.Student_ID
																		   select s);
						var completion = completionList.First();

				        db.SubmitChanges();
			        }

		        }
	        }
            //this.Close();
        }

		private void delete_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete!", MessageBoxButton.YesNo) ==
				    MessageBoxResult.Yes)
				{
					using (PubsDataContext db = new PubsDataContext())
					{
						Student stud = (from s in db.Students
						                where s.Student_ID == student.Student_ID
						                select s).Single();
						var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
																		   where s.Student_ID == student.Student_ID
																		   select s);
						db.Students.DeleteOnSubmit(stud);
						db.Learning_Experiences.DeleteAllOnSubmit(completionList);
						db.SubmitChanges();
						this.Close();
					}
				}
			}	
		}

        private void expanderCollapsedMinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.Width = myWidth;
            this.Height = myHeight;
        }

        private void expanderExpandedOpenWindow(object sender, RoutedEventArgs e)
        {
            myHeight = this.Height;
            myWidth = this.Width;

            this.Width += 200;
            this.Height += 200;
        }

        private void Delete_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Add_MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

		private void Edit_MenuItem_Click(object sender, MouseButtonEventArgs e)
		{
			//using (PubsDataContext db = new PubsDataContext())
			//{
			//Learning_Experience expROW = studentLearningExperiences_DataGrid.SelectedItem as Learning_Experience;
			//var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
			//                                                   where s.Student_ID == student.Student_ID
			//                                                   select s);
			////Learning_Experience exp = (from s in db.Learning_Experiences
			////                            where s.Student_ID == expROW.Student_ID
			////                            select s).Single();
			//    exp.ConfirmedHours = expROW.ConfirmedHours;
			//    exp.CourseNumber = expROW.CourseNumber;
			//    db.SubmitChanges();
			//}
		}

		private void learningExperienceSave_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (studentLearningExperiences_DataGrid.SelectedValue.Equals(null))
			{
				MessageBox.Show("You must first select a row before saving.",
				                "Datagrid Row Selection Error", MessageBoxButton.OK,
				                MessageBoxImage.Exclamation);
			}
			else
			{

				if (dbMethods.CheckDatabaseConnection())
				{
					using (PubsDataContext db = new PubsDataContext())
					{
						Learning_Experience expROW = studentLearningExperiences_DataGrid.SelectedItem as Learning_Experience;
						//Learning_Experience exp = (from s in db.Learning_Experiences
						//                           where s.Student_ID == expROW.Student_ID
						//                           select s);
						var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
						                                                   where s.ID == expROW.ID
						                                                   select s);
						var completion = completionList.First();
						completion.Student_ID = student.Student_ID;
						completion.ConfirmedHours = expROW.ConfirmedHours;
						completion.CourseNumber = expROW.CourseNumber;
						completion.LiabilityWaiver = expROW.LiabilityWaiver;
						completion.ProjectAgreement = expROW.ProjectAgreement;
						completion.Semester = expROW.Semester;
						completion.Year = expROW.Year;
						completion.TimeLog = expROW.TimeLog;
						completion.TotalHours = expROW.TotalHours;
						completion.TypeofLearning = expROW.TypeofLearning;

						db.SubmitChanges();
						LoadStudentLearningExperiences();

					}
				}
			}
		}

		private void learningExperienceAdd_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					Learning_Experience expROW = studentLearningExperiences_DataGrid.SelectedItem as Learning_Experience;
					Learning_Experience exp = new Learning_Experience();

					//HACK works accept student_ID cannot be the key
					exp.Student_ID = student.Student_ID;
					exp.ConfirmedHours = expROW.ConfirmedHours;
					exp.CourseNumber = expROW.CourseNumber;
					exp.LiabilityWaiver = expROW.LiabilityWaiver;
					exp.ProjectAgreement = expROW.ProjectAgreement;
					exp.Semester = expROW.Semester;
					exp.Year = expROW.Year;
					exp.TimeLog = expROW.TimeLog;
					exp.TotalHours = expROW.TotalHours;
					exp.TypeofLearning = expROW.TypeofLearning;

					db.Learning_Experiences.InsertOnSubmit(exp);
					db.SubmitChanges();
					LoadStudentLearningExperiences();
				}
			}

		}

		private void learningExperienceDelete_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				if (MessageBox.Show("Are you sure you want to delete this learning experience?", "Confirm Delete!", MessageBoxButton.YesNo) ==
					MessageBoxResult.Yes)
				{
					using (PubsDataContext db = new PubsDataContext())
					{
						Learning_Experience expROW = studentLearningExperiences_DataGrid.SelectedItem as Learning_Experience;
						var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
																		   where s.ID == expROW.ID
																		   select s);
						var completion = completionList.First();
						db.Learning_Experiences.DeleteOnSubmit(completion);
						db.SubmitChanges();
						LoadStudentLearningExperiences();
					}
				}
			}
		}
	}
}
