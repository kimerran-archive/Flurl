﻿using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Flurl.Http
{
	public static class DownloadExtensions
	{
		/// <summary>
		/// Asynchronously downloads a file at the specified URL.
		/// </summary>
		/// <param name="localFolderPath">Path of local folder where file is to be downloaded.</param>
		/// <param name="localFileName">Name of local file. If not specified, the source filename (last segment of the URL) is used.</param>
		/// <param name="bufferSize">Buffer size in bytes. Default is 4096.</param>
		/// <returns>A Task whose result is the local path of the downloaded file.</returns>
		public static async Task<string> DownloadFileAsync(this FlurlClient client, string localFolderPath, string localFileName = null, int bufferSize = 4096) {
			if (localFileName == null)
				localFileName = client.Url.Path.Split('/').Last();

			if (!Directory.Exists(localFolderPath))
				Directory.CreateDirectory(localFolderPath);

			var filePath = Path.Combine(localFolderPath, localFileName);

			// http://developer.greenbutton.com/downloading-large-files-with-the-net-httpclient
			var response = await client.HttpClient.GetAsync(client.Url, HttpCompletionOption.ResponseHeadersRead);

			// http://codereview.stackexchange.com/a/18679
			using (var httpStream = await response.Content.ReadAsStreamAsync())
			using (var filestream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true)) {
				await httpStream.CopyToAsync(filestream, bufferSize);
			}

			return filePath;
		}

		/// <summary>
		/// Asynchronously downloads a file at the specified URL.
		/// </summary>
		/// <param name="localFolderPath">Path of local folder where file is to be downloaded.</param>
		/// <param name="localFileName">Name of local file. If not specified, the source filename (last segment of the URL) is used.</param>
		/// <param name="bufferSize">Buffer size in bytes. Default is 4096.</param>
		/// <returns>A Task whose result is the local path of the downloaded file.</returns>
		public static Task<string> DownloadFileAsync(this string url, string localFolderPath, string localFileName = null, int bufferSize = 4096) {
			return new FlurlClient(url).DownloadFileAsync(localFolderPath, localFileName, bufferSize);
		}

		/// <summary>
		/// Asynchronously downloads a file at the specified URL.
		/// </summary>
		/// <param name="localFolderPath">Path of local folder where file is to be downloaded.</param>
		/// <param name="localFileName">Name of local file. If not specified, the source filename (last segment of the URL) is used.</param>
		/// <param name="bufferSize">Buffer size in bytes. Default is 4096.</param>
		/// <returns>A Task whose result is the local path of the downloaded file.</returns>
		public static Task<string> DownloadFileAsync(this Url url, string localFolderPath, string localFileName = null, int bufferSize = 4096) {
			return new FlurlClient(url).DownloadFileAsync(localFolderPath, localFileName, bufferSize);
		}
	}
}
