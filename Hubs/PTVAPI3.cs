namespace S105.Hubs;

class PTVAPI3(string apiKey, int developerId, HttpClient client)
{
    private readonly string _apiKey = apiKey;
    private readonly int _developerId = developerId;
    private readonly HttpClient _client = client;

    public string GetURL(string endpoint)
    {
        // add developer id
        string url = string.Format($"{endpoint}{(endpoint.Contains('?') ? '&' : '?')}devid={_developerId}");
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        
        // encode key
        byte[] keyBytes = encoding.GetBytes(_apiKey);
        
        // encode url
        byte[] urlBytes = encoding.GetBytes(url);
        byte[] tokenBytes = new System.Security.Cryptography.HMACSHA1(keyBytes).ComputeHash(urlBytes);
        
        var sb = new System.Text.StringBuilder();
        // convert signature to string
        Array.ForEach<byte>(tokenBytes, x => sb.Append(x.ToString("X2")));

        // add signature to url
        // url = string.Format("{0}&signature={1}", url, sb.ToString());
        url = $"http://timetableapi.ptv.vic.gov.au{url}&signature={sb}";

        return url;
    }

    public string GetData(string endpoint)
    {
        // get url from endpoint, api key and developer id
        var url = GetURL(endpoint);
        // get data
        HttpResponseMessage response = _client.GetAsync(url).Result;
        response.EnsureSuccessStatusCode();
        string data = response.Content.ReadAsStringAsync().Result;
        return data;
    }

    public async Task<string> GetDataAsync(string endpoint)
    {
        // get url from endpoint, api key and developer id
        var url = GetURL(endpoint);

        // get data asynchronously
        HttpResponseMessage response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string data = await response.Content.ReadAsStringAsync();

        return data;
    }
}