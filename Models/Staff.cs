using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Staff
    {
		public int staffID { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string speciality { get; set; }
		public string address { get; set; }
		public DateTime hiringDate { get; set; }
		public string phoneNo { get; set; }
	}
}
