using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmmryCoreSharp.Smmry
{
    public class SmmryClient
    {
        #region Privte Members
        private SmmryParameters defaultParameters = null;
        string defaultSmmryEndpointUrl = "https://api.smmry.com/";
        #endregion

        #region Constructors
        public SmmryClient(SmmryParameters parameters, string smmryEndpointUrl = "https://api.smmry.com/")
        {
            if(parameters == null)
            {
                throw new InvalidOperationException("SmmryParameters cannot be null.");
            }
            
            if (string.IsNullOrWhiteSpace(smmryEndpointUrl))
            {
                throw new InvalidOperationException("The Smmry endpoint URL cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(parameters.ApiKey))
            {
                throw new InvalidOperationException("The ApiKey cannot be null or empty.");
            }

            if (!string.IsNullOrWhiteSpace(parameters.Url) && !string.IsNullOrWhiteSpace(parameters.Text))
            {
                throw new InvalidOperationException("Either \"Url\" or \"Text\" must be part of your request but not both.");
            }

            if (string.IsNullOrWhiteSpace(parameters.Url) && string.IsNullOrWhiteSpace(parameters.Text))
            {
                throw new InvalidOperationException("Either \"Url\" or \"Text\" must be part of your request.");
            }

            defaultParameters = parameters;
            defaultSmmryEndpointUrl = smmryEndpointUrl;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// If "Url" - "Url" is added to the end of the main url query string like:
        /// https://api.smmry.com/?SM_API_KEY=YOURAPIHERE&SM_LENGTH=3&SM_KEYWORD_COUNT=24&SM_WITH_BREAK&SM_QUOTE_AVOID&SM_URL=www.EXAMPLEURL.com
        /// and GetAsync(string requestUri) is called from the HttpClient
        /// If "Text" - "Text" is put inside FormUrlEncodedContent with the key "sm_api_input" and PostAsync(string requestUri, HttpContent content) is called from the HttpClient.
        /// </summary>
        public Smmry Download()
        {
            string Json = GetJsonAsync(defaultParameters).GetAwaiter().GetResult();
            Smmry tempSmmry = JsonConvert.DeserializeObject<Smmry>(Json);

            if (tempSmmry.GetType().GetProperties().All(x => x.GetValue(tempSmmry) == null))
            {
                var error = JsonConvert.DeserializeObject<SmmryError>(Json);
                throw new SmmryException($"{error.Code}: {error.Message}");
            }

            return tempSmmry;
        }
        #endregion

        #region Private Methods
        private async Task<string> GetJsonAsync(Dictionary<string, object> smmryParameters)
        {
            var Client = new HttpClient();
            var url = defaultSmmryEndpointUrl + smmryParameters;

            if (smmryParameters.ContainsKey("sm_api_input"))
            {
                var formContent = new FormUrlEncodedContent
                    (
                        new[] { new KeyValuePair<string, string>("sm_api_input", smmryParameters["sm_api_input"].ToString()) }
                    );

                using var responsemessage = await Client.PostAsync(url, formContent);
                using var content = responsemessage.Content;
                return await content.ReadAsStringAsync();
            }
            else
            {
                using var responsemessage = await Client.GetAsync(url);
                using var content = responsemessage.Content;
                return await content.ReadAsStringAsync();
            }
        }
        #endregion
    }
}
