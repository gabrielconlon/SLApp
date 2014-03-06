using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLApp_Beta
{
	/// <summary>
	/// possible link to help with understanding the LINQ method of joining tables
	/// http://stackoverflow.com/questions/10747800/wpf-linq-binding-join-qry-to-datagrid
	/// </summary>

	public class TableStudent
	{
		public int studentID { get; set; }
		public int graduationYear { get; set; }
		public string FristName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

	}

	public class TableLearningExperience
	{
		public int studentID;
		public int Year;
		public int totalHours;
		public bool confirmedHours;
		public bool liabilityWaiver;
		public bool projectAgreement;
		public bool timeLog;
		public string courseNumber;
		public enum Semester {Fall, Jan, Spring};
		public string TypeofLearning;


	}

	public class STRONGclass
	{
		//Student

		public TableStudent Student { get; set; }
		public TableLearningExperience Experience;
	}
}
