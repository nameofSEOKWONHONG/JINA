using Jina.Lang.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Jina.Lang
{
	public interface ILocalizerClient
	{
		Task Init();
	}

	/// <summary>
	/// client side localizer
	/// </summary>
	public class LocalizerClient : ILocalizerClient, ILocalizer
	{
        private readonly HttpClient _httpClient;
        public LocalizerClient(IHttpClientFactory httpClientFactory)
        {
			_httpClient = httpClientFactory.CreateClient();
		}

		public string this[string name] => throw new NotImplementedException();

		public async Task Init()
		{
			var res = await _httpClient.GetAsync("");
			res.EnsureSuccessStatusCode();

			var result = await res.Content.ReadFromJsonAsync<Dictionary<string, string>>();
		}


    }
}
