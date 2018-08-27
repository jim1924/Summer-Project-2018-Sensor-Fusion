using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Video:InputFile
    {
		public double duration { get; set; }
		public double aspectRatio { get; private set; }
		public double frameRate { get; private set; }
		public string scanType { get; private set; }
		public string codec { get; private set; }
	}
}
