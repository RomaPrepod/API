using System;

namespace RomaPrepod.WebApi.Model
{
	public class Lecture
	{
		public string Title { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public DownloadsList Downloads { get; set; }

		public class DownloadsList
		{
			public string Presentation { get; set; }
			public string Manual { get; set; }
		}
	}
}
