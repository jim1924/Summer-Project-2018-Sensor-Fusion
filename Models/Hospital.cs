using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Hospital
    {

		public int hospitalID { get; set; }
		public string name { get; set; }
		public string locale { get; set; }
		public string address { get; set; }
		public string postCode { get; set; }
		public string city { get; set; }

	}
}
