using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Operation
    {
		public long patientID { get; set; }
		public int hospitalID { get; set; }
		public string roomNO { get; set; }
		public DateTime dateStamp { get; set; }
		public int duration { get; set; }
		public long operationID { get; set; }
		public int operationTypeID { get; set; }


		//for filtering
		public DateTime fromDate { get; set; }
		public DateTime toDate { get; set; }
		public int[] staffIDs { get; set; }


	}
}
