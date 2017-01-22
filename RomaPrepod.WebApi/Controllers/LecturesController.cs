using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RomaPrepod.WebApi.Model;

namespace RomaPrepod.WebApi.Controllers
{
	[Route("api/[controller]")]
	public class LecturesController : Controller
	{
		[HttpGet]
		public IEnumerable<Lecture> Get()
		{
			return new []
			{
				new Lecture
				{
					Title = "Лекция 1. Введение",
					Date = DateTime.Today.AddDays(-2),
					Description = "Occaecat proident exercitation eu aliquip irure ea quis ad dolor. Nulla cupidatat aute nisi enim eu ea minim labore nostrud mollit quis occaecat. Dolor aute do excepteur esse occaecat et aliqua. Aliqua aute tempor et ullamco labore Lorem excepteur. Commodo ut anim eu quis reprehenderit. Magna duis est veniam eiusmod exercitation non. Et deserunt enim duis pariatur irure id incididunt nisi."
				},

				new Lecture
				{
					Title = "Лекция 2. Продолжение",
					Date = DateTime.Today.AddDays(-1),
					Description = "Dolore fugiat ad esse cillum do sint veniam minim dolor ut ipsum et laboris. Anim proident duis Lorem anim voluptate. Ipsum amet voluptate cupidatat eiusmod ipsum laboris excepteur pariatur excepteur."
				},

				new Lecture
				{
					Title = "Лекция 3. Завершение",
					Date = DateTime.Today,
					Description = "Tempor eiusmod proident et nulla nulla occaecat dolor pariatur aute nostrud. Eu sint officia id fugiat nulla non quis aute laborum. Proident cillum et mollit id nisi adipisicing ullamco consequat aliquip proident. Proident consequat ea aliqua ex. Cillum Lorem velit ut laborum veniam in ad amet tempor laborum nisi."
				}
			};
		}
	}
}
