using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace StorefrontApps_Outlook.Classes.CitrixHelper
{
	public partial class Storefront
	{
		public async static Task<byte[]> GetImage(string SFURL, CitrixApplicationInfo Application)
		{
			HttpClient _client = new HttpClient();

			string _imageResource = (SFURL.EndsWith("/")) ? string.Format("{0}",Application.AppIconUrl) : string.Format("/{0}", Application.AppIconUrl);

			string _imageResourceUrl = string.Format("{0}{1}", SFURL, _imageResource);

			StringContent _bodyContent = new StringContent("");

			HttpResponseMessage _imageResp = await _client.GetAsync(_imageResourceUrl);

			var imageBytes = await _imageResp.Content.ReadAsByteArrayAsync();


			return imageBytes;
		}
	}
}
