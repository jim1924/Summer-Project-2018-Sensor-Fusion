using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Patient
    {
		public long patientID { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string address { get; set; }
		public string postCode { get; set; }
		public string phoneNO { get; set; }


	}
}
