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
					Description = "В этой лекции студентов знакомят с тематикой предмета, предметной областью целями и задачами интернет приложений. Основной уроп идет на общее понимание среды существовония интренет проектов. Закрладывается фундамеент для дальнейшего освоения метриала курса. Так же на первом занятие предусмотрено входное тетирование, нацеленное на выяснение остаточных знаний студентов и общего уровня подготовки.",
					Downloads = new Lecture.DownloadsList
					{
						Presentation = "http://example.com/lecture-1.pptx",
						Manual = "http://example.com/lecture-1.docx"
					}
				},

				new Lecture
				{
					Title = "Лекция 2. Обзор сиситем управления контентом и знакомство с 1С-Битрикс",
					Date = DateTime.Today.AddDays(-1),
					Description = "Лекция включает в себя обзор систем управления контентом существующих на рынке. Обзор функций и возможностей 1С-Битрикс:Управление сайтом и инфраскруктуры постороенной вокруг системы."
				},

				new Lecture
				{
					Title = "Лекция 3. Внедрение шаблона дизайна.",
					Date = DateTime.Today,
					Description = "Рассматривается примение дизайна, правила интеграции гипертекстовой разметки в шаблон сайта построенного на 1С-Битрикс."
				}
			};
		}
	}
}
