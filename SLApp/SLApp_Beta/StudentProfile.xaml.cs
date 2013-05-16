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
    /// TODO - adjust database, types of notes allows nulls, add column of text to hold the actual note
	/// </summary>
	public partial class StudentProfile : Window
    {

        #region Database Methods, Formatting Methods, Members

        private Student student = new Student();
		private bool isEdit;
		DatabaseMethods dbMethods = new DatabaseMethods();

        private double myWidth;
        private double myHeight;

        

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
            this.Height += 100;
        }

        #endregion

        public StudentProfile(bool isAdmin)
		{
			InitializeComponent();
            if (isAdmin == false)
            {
                studentNotes_DataGrid.IsEnabled = false;
            }
            LoadStudentLearningExperiences();
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

            LoadStudentLearningExperiences();
			
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					var empty = (from exp in db.Learning_Experiences
					             where exp.Student_ID == 0
					             select exp);
					db.Learning_Experiences.DeleteAllOnSubmit(empty);
					db.SubmitChanges();
				}
			}
		}

		#region Student Buttons

			private
			void cancel_BTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

		private void save_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{
					if(studentID_TB.Text.Length > 0 && graduationYear_TB.Text.Length > 0)
					{
						var CheckExists = (from s in db.Students
						                   where s.Student_ID == Convert.ToInt32(studentID_TB.Text)
						                   select s);
						//if the user does not exists, application will create a new user
						if (CheckExists.Count() == 0)
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
							LoadStudentLearningExperiences();
						}
						else
						{
							//save student info
							Student stud = (from s in db.Students
							                where s.Student_ID == student.Student_ID
							                select s).Single();
							stud.Student_ID = Convert.ToInt32(studentID_TB.Text);
							stud.FirstName = studentFirstName_TB.Text;
							stud.LastName = studentLastName_TB.Text;
							stud.GraduationYear = Convert.ToInt32(graduationYear_TB.Text);
							stud.Email = studentemail_TB.Text;

							//get list of learning experiences
							var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
							                                                   where s.Student_ID == student.Student_ID
							                                                   select s);


							//saves experience by calling the save experiences button event

							learningExperienceSave_BTN_Click(sender, e);

							db.SubmitChanges();
						}

					}
					else
					{
						MessageBox.Show(
							"SLApp apologizes for the inconvenience, but at this time all fields must contain data before saving.",
							"Save Error!", MessageBoxButton.OK, MessageBoxImage.Error);
					}

					//this.Close();
				}
			}
		}

		private
	        void SaveAndClose_BTN_Click(object sender, RoutedEventArgs e)
        {
            save_BTN_Click(sender, e);
            this.Close();
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

        #endregion

        #region Learning Experiences

		public void LoadStudentLearningExperiences()
		{
			if (dbMethods.CheckDatabaseConnection())
			{
				using (PubsDataContext db = new PubsDataContext())
				{

					var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
																	   where s.Student_ID == student.Student_ID
																	   select s);
					if (!completionList.Any())
					{
						Learning_Experience exp = new Learning_Experience();
						exp.Student_ID = student.Student_ID;
						db.Learning_Experiences.InsertOnSubmit(exp);
						db.SubmitChanges();
						completionList.Add(exp);
					}
					studentLearningExperiences_DataGrid.DataContext = completionList;

				}
			}
		}

        private bool learningExperienceFieldsCheck(Learning_Experience expROW)
        {
            if (expROW == null)
            {
                //MessageBox.Show("You must first select a valid row before adding, saving, or deleting.",
                //                "Datagrid Row Selection Error", MessageBoxButton.OK,
                //                MessageBoxImage.Exclamation);
                return false;
            }
            else if (expROW.Semester != "Fall" && expROW.Semester != "Jan" && expROW.Semester != "Spring" && expROW.Semester != "")
            {
                MessageBox.Show("Entry in Semester column invalid. Valid entries are blank, 'Fall', 'Jan', or 'Spring'.",
                                "Datagrid Row Error", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return false;
            }
            else if (expROW.TypeofLearning != "Discipline-Based" && expROW.TypeofLearning != "Problem-Based" &&
                     expROW.TypeofLearning != "Pure Service" && expROW.TypeofLearning != "Service Internship" &&
                     expROW.TypeofLearning != "Community Based Research" && expROW.TypeofLearning != "Capstone Class"
                && expROW.TypeofLearning != "")
            {
                MessageBox.Show("Entry in Type of Learning column invalid.\n" +
                    "Valid entries are blank, 'Discipline-Based', 'Problem-Based', 'Pure Service',\n" +
                "'Service Internship', 'Community Based Research', or 'Capstone Class'",
                                "Datagrid Row Error", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return false;
            }
            return true;
        }


        //button is disabled and invisible, the "Save Profile" button calls this method to save experiences
        //HACK BUG - does not work properly with the "save" button on the right click menu
        private void learningExperienceSave_BTN_Click(object sender, RoutedEventArgs e)
        {
            Learning_Experience expROW = studentLearningExperiences_DataGrid.SelectedItem as Learning_Experience;
                if (learningExperienceFieldsCheck(expROW))
                {
                    if (dbMethods.CheckDatabaseConnection())
                    {
                        using (PubsDataContext db = new PubsDataContext())
                        {
                            //Learning_Experience exp = (from s in db.Learning_Experiences
                            //                           where s.Student_ID == expROW.Student_ID
                            //                           select s);
                            var completionList = new List<Learning_Experience>(from s in db.Learning_Experiences
                                                                               where s.ID == expROW.ID
                                                                               select s);
                            if (completionList.Count > 0)
                            {

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
	                            completion.Section = expROW.Section;
	                            completion.Professor = expROW.Professor;
	                            completion.CourseName = expROW.CourseName;

                                db.SubmitChanges();
                                LoadStudentLearningExperiences();
                            }
                            else
                            {
                                Learning_Experience exp = new Learning_Experience();

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
                }
        }

        //currently hidden and disabled, testing the new save/add combo button
        private void learningExperienceAdd_BTN_Click(object sender, RoutedEventArgs e)
        {
            Learning_Experience expROW = studentLearningExperiences_DataGrid.SelectedItem as Learning_Experience;
            if (learningExperienceFieldsCheck(expROW))
            {
                if (dbMethods.CheckDatabaseConnection())
                {
                    using (PubsDataContext db = new PubsDataContext())
                    {
                        Learning_Experience exp = new Learning_Experience();

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

        }

        //exists, but uneccesary because of the addition of the "delete" button on right click
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
                        //if (expROW != null && completionList.Count() == 1)
                        //{
                        //    var completion = completionList.First();
                        //    db.Learning_Experiences.DeleteOnSubmit(completion);

                        //    Learning_Experience exp = new Learning_Experience();
                        //    exp.Student_ID = student.Student_ID;
                        //    db.Learning_Experiences.InsertOnSubmit(exp);
                        //    db.SubmitChanges();
                        //    LoadStudentLearningExperiences();

                        //    db.SubmitChanges();
                        //}
                        //else 
                        if (expROW != null && completionList.Any())
                        {
                            var completion = completionList.First();
                            db.Learning_Experiences.DeleteOnSubmit(completion);
                            db.SubmitChanges();
                            LoadStudentLearningExperiences();
                        }
                        else
                        {
                            LoadStudentLearningExperiences();
                        }
                    }
                }
            }
        }

        private void Delete_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            learningExperienceDelete_BTN_Click(sender, e);
        }

        #endregion

		private void StudentLearningExperiences_DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			if (e.PropertyName == "ID") e.Cancel = true;
		}

        #region Notes

        

        #endregion

    }
}
