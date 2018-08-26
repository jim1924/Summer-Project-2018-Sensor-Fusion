using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Video:InputFile
    {
		public double duration { get; set; }
		public double AspectRatio { get; private set; }
		public double FrameRate { get; private set; }
		public string ScanType { get; private set; }
		public string Codec { get; private set; }
	}
}
