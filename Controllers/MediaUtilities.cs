using MediaInfoLib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace SensorFusion.Controllers
{
	public class MediaUtilities
    {
		private IHostingEnvironment _env;
		string path;
		MediaInfo mi = new MediaInfo();

		public MediaUtilities(IHostingEnvironment env,string fileName)
		{
			_env = env;
			path= _env.WebRootPath +"\\TempFiles\\"+fileName;
		}
		public TimeSpan GetVideoDuration()
		{
			mi.Open(path);
			var videoInfo = new VideoInfo(mi);
			var result= videoInfo.Duration;
			mi.Close();
			return result;
		}
		public long GetVideoSize()
		{

			mi.Open(path);
			var videoInfo = new VideoInfo(mi);
			long size = videoInfo.FileSize;
			mi.Close();
			return size;
		}

		public DateTime GetVideoTaggedDate()
		{
			mi.Open(path);
			var videoInfo = new VideoInfo(mi);
			string[] taggedDate = videoInfo.TaggedDate.Split(' ');
			string[] date = taggedDate[1].Split('-');
			string[] time = taggedDate[2].Split(':');
			DateTime fullDateTime=new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]), Int32.Parse(time[0]),Int32.Parse(time[1]),Int32.Parse(time[2]));
			mi.Close();
			return fullDateTime;
		}
		public DateTime GetVideoEncodedDate()
		{
			mi.Open(path);
			var videoInfo = new VideoInfo(mi);
			string[] encodedDate = videoInfo.EncodedDate.Split(' ');
			string[] date = encodedDate[1].Split('-');
			string[] time = encodedDate[2].Split(':');
			DateTime fullDateTime = new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]), Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]));
			mi.Close();
			return fullDateTime;
		}

		public void PrintVideoAvailableProperties()
		{
			mi.Open(path);
			var videoInfo = new VideoInfo(mi);
			mi.Option("Language", "raw");
			Console.WriteLine(mi.Inform());
		}












		public DateTime GetAudioLastModifiedTime()
		{
			FileInfo file = new FileInfo(path);
			DateTime timeStamp = file.LastWriteTime;
			return timeStamp;
		}

		public DateTime GetAudioCreationTime()
		{
			FileInfo file = new FileInfo(path);
			DateTime timeStamp = file.CreationTime;
			return timeStamp;
		}

		public DateTime GetAudioLastAccessTime()
		{
			FileInfo file = new FileInfo(path);
			DateTime timeStamp = file.LastAccessTime;
			return timeStamp;
		}
		public DateTime GetAudioLastWriteTime()
		{

			DateTime timeStamp = File.GetLastWriteTimeUtc(path);
			return timeStamp;
		}



		public TimeSpan GetAudioDuration()
		{
			mi.Open(path);
			var audioInfo = new AudioInfo(mi);
			var result = audioInfo.Duration;
			mi.Close();
			return result;
		}

		public string GetAudioSize()
		{
			mi.Open(path);
			var audioInfo = new AudioInfo(mi);
			var result = audioInfo.FileSize;
			mi.Close();
			return result;
		}
		public string GetAudioStreamSize()
		{
			mi.Open(path);
			var audioInfo = new AudioInfo(mi);
			var result = audioInfo.StreamSize;
			mi.Close();
			return result;
		}

		public DateTime GetAudioEarlierDate(string fileName)
		{
			DateTime earliest = new DateTime(9000,1,1);
			earliest = ( GetDateFromFileName(fileName).CompareTo(new DateTime())!=0) ? GetDateFromFileName(fileName) : earliest;
			//earliest is later than GetAudioCreationTime
			earliest = (earliest.CompareTo(GetAudioCreationTime()) > 0) ? GetAudioCreationTime() : earliest;
			earliest = (earliest.CompareTo(GetAudioLastAccessTime()) > 0) ? GetAudioLastAccessTime() : earliest;
			earliest = (earliest.CompareTo(GetAudioLastModifiedTime()) > 0) ? GetAudioLastModifiedTime() : earliest;
			earliest = (earliest.CompareTo(GetAudioLastWriteTime()) > 0) ? GetAudioLastWriteTime() : earliest;

			mi.Open(path);
			var audioInfo = new AudioInfo(mi);
			if (audioInfo.TaggedDate!=null && audioInfo.TaggedDate!="")
			{
			string[] taggedDate = audioInfo.TaggedDate.Split(' ');
			string[] date = taggedDate[1].Split('-');
			string[] time = taggedDate[2].Split(':');
			DateTime TaggedDate = new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]), Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]));
			earliest = (earliest.CompareTo(taggedDate) > 0) ? TaggedDate : earliest;

			}
			if(audioInfo.EncodedDate != null && audioInfo.EncodedDate != "")
			{
			string[] encodedDate = audioInfo.EncodedDate.Split(' ');
			string[] date = encodedDate[1].Split('-');
			string[] time = encodedDate[2].Split(':');
			DateTime EncodedDate = new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]), Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]));
			earliest = (earliest.CompareTo(EncodedDate) > 0) ? EncodedDate : earliest;
			}
			mi.Close();
			return earliest;
		}



		public void PrintAudioAvailableProperties()
		{
			mi.Open(path);
			var videoInfo = new AudioInfo(mi);
			mi.Option("Language", "raw");
			Console.WriteLine(mi.Inform());
		}








		public static DateTime GetDateFromFileName(string fileName)
		{
			try
			{
			string[] datesArray = fileName.Split('_');
			datesArray[0] = datesArray[0].Substring(datesArray[0].Length - 4, 4);
			datesArray[5] = datesArray[5].Substring(0, 2);
				DateTime result= new DateTime(Int32.Parse(datesArray[0]), Int32.Parse(datesArray[1]), Int32.Parse(datesArray[2]), Int32.Parse(datesArray[3]), Int32.Parse(datesArray[4]), Int32.Parse(datesArray[5]));
				return result;
			}
			catch (Exception)
			{

				return new DateTime();
			}


		}





		public static void CleanTempFolder(IHostingEnvironment hostingEnvironment)
		{

			System.IO.DirectoryInfo di = new DirectoryInfo(hostingEnvironment.WebRootPath + "\\TempFiles");

			foreach (FileInfo file in di.GetFiles())
			{
				file.Delete();
			}
		}


	}





















	public class VideoInfo
	{
		public string Codec { get; private set; }
		public int Width { get; private set; }
		public int Heigth { get; private set; }
		public double FrameRate { get; private set; }
		public string FrameRateMode { get; private set; }
		public string ScanType { get; private set; }
		public TimeSpan Duration { get; private set; }
		public int Bitrate { get; private set; }
		public string AspectRatioMode { get; private set; }
		public double AspectRatio { get; private set; }
		public string TaggedDate { get; private set; }
		public string EncodedDate { get; private set; }
		public long FileSize { get; private set; }




		public VideoInfo(MediaInfo mi)
		{
			Codec = mi.Get(StreamKind.Video, 0, "Format");
			Width = int.Parse(mi.Get(StreamKind.Video, 0, "Width"));
			Heigth = int.Parse(mi.Get(StreamKind.Video, 0, "Height"));
			Duration = TimeSpan.FromMilliseconds(int.Parse(mi.Get(StreamKind.Video, 0, "Duration")));
			Bitrate = int.Parse(mi.Get(StreamKind.Video, 0, "BitRate"));
			AspectRatioMode = mi.Get(StreamKind.Video, 0, "AspectRatio/String"); //as formatted string
			AspectRatio = double.Parse(mi.Get(StreamKind.Video, 0, "AspectRatio"));
			FrameRate = double.Parse(mi.Get(StreamKind.Video, 0, "FrameRate"));
			FrameRateMode = mi.Get(StreamKind.Video, 0, "FrameRate_Mode");
			ScanType = mi.Get(StreamKind.Video, 0, "ScanType");
			TaggedDate = mi.Get(StreamKind.Video, 0, "Tagged_Date");
			EncodedDate = mi.Get(StreamKind.General, 0, "Encoded_Date");
			FileSize = Int64.Parse(mi.Get(StreamKind.General, 0, "FileSize"));


		}
	}





	public class AudioInfo
	{
		public string Codec { get; private set; }
		public string CompressionMode { get; private set; }
		public string ChannelPositions { get; private set; }
		public TimeSpan Duration { get; private set; }
		public int Bitrate { get; private set; }
		public string BitrateMode { get; private set; }
		public int SamplingRate { get; private set; }
		public string TaggedDate { get; private set; }
		public string EncodedDate { get; private set; }
		public string FileSize { get; set; }
		public string StreamSize { get; set; }




		public AudioInfo(MediaInfo mi)
		{
			Codec = mi.Get(StreamKind.Audio, 0, "Format");
			Duration = TimeSpan.FromMilliseconds(int.Parse(mi.Get(StreamKind.Audio, 0, "Duration")));
			Bitrate = int.Parse(mi.Get(StreamKind.Audio, 0, "BitRate"));
			BitrateMode = mi.Get(StreamKind.Audio, 0, "BitRate_Mode");
			CompressionMode = mi.Get(StreamKind.Audio, 0, "Compression_Mode");
			ChannelPositions = mi.Get(StreamKind.Audio, 0, "ChannelPositions");
			SamplingRate = int.Parse(mi.Get(StreamKind.Audio, 0, "SamplingRate"));
			TaggedDate = mi.Get(StreamKind.General, 0, "Tagged_Date");
			EncodedDate = mi.Get(StreamKind.General, 0, "Encoded_Date");
			FileSize = mi.Get(StreamKind.General, 0, "FileSize");
			StreamSize = mi.Get(StreamKind.Audio, 0, "StreamSize");
		}
	}
}







//var ffProbe = new NReco.VideoInfo.FFProbe();
//var videoInfo = ffProbe.GetMediaInfo(path);
//Console.WriteLine(videoInfo.FormatName);
//Console.WriteLine(videoInfo.Duration);





//IMediaInfo mediaInfo = await MediaInfo.Get(path);
//var videoDuration = mediaInfo.VideoStreams.First().Duration;
//Console.WriteLine("THE FUCKA DURATION IS "+videoDuration);