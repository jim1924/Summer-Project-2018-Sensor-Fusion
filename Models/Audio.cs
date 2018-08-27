using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Audio:InputFile
    {
		public double duration { get; set; }
		public string channelPositions { get; private set; }
		public int bitrate { get; private set; }
		public int samplingRate { get; private set; }
		public string compressionMode { get; private set; }
		public string codec { get; private set; }

	}
}
