using System;

namespace SensorFusion.Models
{
	public class Staff: Participant
    {
		public int staffID { get; set; }
		public string speciality { get; set; }
		public DateTime hiringDate { get; set; }
	}
}
