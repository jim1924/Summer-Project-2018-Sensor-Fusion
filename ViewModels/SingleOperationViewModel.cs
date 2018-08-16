﻿using SensorFusion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.ViewModels
{
    public class SingleOperationViewModel
    {
		public long operationID { get; set; }
		public string hospitalName { get; set; }
		public string roomNO { get; set; }
		public DateTime date { get; set; }
		public Patient patient { get; set; }
		public string staff { get; set; }




		//for details
		public List<Video> videoFiles { get; set; }
		public List<Audio> audioFiles { get; set; }
		public string type { get; set; }


	}
}