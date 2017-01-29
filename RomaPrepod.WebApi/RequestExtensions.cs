using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RomaPrepod.WebApi
{
	public static class RequestExtensions
	{
		public static Task<T> GetJsonResponseAsync<T>(this HttpWebRequest request)
		{
			return request.GetJsonResponseAsync<T>(new JsonSerializerSettings());
		}

		public static async Task<T> GetJsonResponseAsync<T>(this HttpWebRequest request, JsonSerializerSettings settings)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			if (settings == null) throw new ArgumentNullException(nameof(settings));

			using (var response = await request.GetResponseAsync())
			using (var stream = response.GetResponseStream())
			using (var reader = new StreamReader(stream))
			{
				var json = await reader.ReadToEndAsync();
				return JsonConvert.DeserializeObject<T>(json, settings);
			}
		}

		public static Task WriteJsonBodyAsync<T>(this HttpWebRequest request, T body)
		{
			return request.WriteJsonBodyAsync(body, new JsonSerializerSettings());
		}

		public static async Task WriteJsonBodyAsync<T>(this HttpWebRequest request, T body, JsonSerializerSettings settings)
		{
			using (var stream = await request.GetRequestStreamAsync())
			using (var writer = new StreamWriter(stream))
			{
				var json = JsonConvert.SerializeObject(body, settings);
				await writer.WriteAsync(json);
			}
		}
	}
}
