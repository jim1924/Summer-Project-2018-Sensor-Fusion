using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Patient:Participant
    {
		public long patientID { get; set; }
		public DateTime registrationDate { get; set; }
		public int height { get; set; }
		public float weight { get; set; }

	}
}
