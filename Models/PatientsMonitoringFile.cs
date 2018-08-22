using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class PatientsMonitoringFile
	{
		public long OperationID { get; set; }
		public long size_bytes { get; set; }
		public DateTime timeStamp { get; set; }
		public string type { get; set; }
		public string fileName { get; set; }
		public string fullPath { get; set; }

	}
}
