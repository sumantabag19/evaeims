
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EVA.EIMS.Logging
{
	public static class KibanaLogger
	{
		/// <summary>
		/// Add log details by types(Info, Error and Warning)
		/// </summary>
		/// <param name="type"></param>
		/// <param name="className"></param>
		/// <param name="methodName"></param>
		/// <param name="message"></param>
		/// <param name="stackTrace"></param>
		 /// <param name="applicationName"></param>
		public static async Task Log(LogType type, string className, string methodName, string message,
			string stackTrace, string useLogging, string logUrl,string applicationName, HttpClient client)
		{
			string fileName = string.Empty;
			string dir = string.Empty;
			LogClass logClass = new LogClass
			{
				Application = applicationName,
				LogLevel = type.ToString(),
				ClassName = className,
				MethodName = methodName,
				Message = message,
				StackTrace = stackTrace,
				LogDateTime = DateTime.UtcNow,
			};

			var logClassJson = JsonConvert.SerializeObject(logClass);
			try
			{
				string requestUri = logUrl;
				var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
				var settings = new JsonSerializerSettings
				{
					MissingMemberHandling = MissingMemberHandling.Ignore,
					Formatting = Formatting.Indented
				};

				if (logClass != null)
				{
					var jsonContent = JsonConvert.SerializeObject(logClass, settings);

					request.Content = new StringContent(jsonContent);
					request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				}

				var response = client.SendAsync(request);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public class LogClass
		{
			public string ClassName { get; set; }
			public string MethodName { get; set; }
			public string Message { get; set; }
			public string StackTrace { get; set; }
			public DateTime LogDateTime { get; set; }
			public string Application { get; set; }
			public string APIName { get; set; }
			public string IPAddress { get; set; }
			public string LogLevel { get; set; }

		}
	}
}
