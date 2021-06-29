using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public abstract class JSONRequest_Base
{
	private string m_strEndpointPath = String.Empty;

	public JSONRequest_Base(string strPath)
	{
		m_strEndpointPath = strPath;
	}

	public async Task<string> SendRequest(HttpClient client, string strToken, string strEndpoint)
	{
		string strBody = Serialize();

		StringContent content = new StringContent(strBody, Encoding.UTF8, "application/json");
		content.Headers.Add("Token", strToken);

		var response = await client.PostAsync(new Uri(Helpers.FormatString("{0}/{1}", strEndpoint, m_strEndpointPath)), content).ConfigureAwait(true);
		var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true); ;
		return responseString;
	}

	private string Serialize()
	{
		return Newtonsoft.Json.JsonConvert.SerializeObject(this);
	}
}