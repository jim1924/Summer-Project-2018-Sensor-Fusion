using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class SelectedHospitals
    {
		public int  ID { get; set; }
		public string SelectedHospitalIDs { get; set; }

		[NotMapped]
		public IEnumerable<Hospital> HospitalCollection { get; set; }
		[NotMapped]
		public string[] selectedIDArray { get; set; }

	}
}
 