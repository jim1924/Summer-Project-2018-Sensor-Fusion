using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorFusion.Models
{
    public class Audio:InputFile
    {
		public double duration { get; set; }
		public string ChannelPositions { get; private set; }
		public int Bitrate { get; private set; }
		public int SamplingRate { get; private set; }
		public string CompressionMode { get; private set; }
		public string Codec { get; private set; }

	}
}
