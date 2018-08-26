using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Staff: Participant
    {
		public int staffID { get; set; }
		public string speciality { get; set; }
		public DateTime hiringDate { get; set; }
	}
}
