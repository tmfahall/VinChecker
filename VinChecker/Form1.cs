using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VinChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static HttpClient client = new HttpClient();

        class ReturnData
        {
            public string Data { get; set; }
        }

        public class Result
        {
            public string Value { get; set; }
            public string ValueId { get; set; }
            public string Variable { get; set; }
            public int VariableId { get; set; }
        }

        public class RootObject
        {
            public int Count { get; set; }
            public string Message { get; set; }
            public string SearchCriteria { get; set; }
            public List<Result> Results { get; set; }
        }

        string outputString;
        private async void searchButton_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(inputBox.Text, @"^[a-zA-Z0-9]+$") && inputBox.Text.Length >= 11 && inputBox.Text.Length <= 17)
            {
                string vin = inputBox.Text;
                string url = string.Format("https://vpic.nhtsa.dot.gov/api/vehicles/decodevin/", vin);

                var responseText = "";

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    HttpResponseMessage response = await client.GetAsync(vin + "?format=json");
                    response.EnsureSuccessStatusCode();
                    responseText = await response.Content.ReadAsStringAsync();

                    //JObject obj = JObject.Parse(responseText);
                    //var token = (JArray)obj.SelectToken("Results");
                    //var list = new List<RootObject>();

                    //foreach (var item in token)
                    //{
                    //    string json = JsonConvert.SerializeObject(item.SelectToken("Results"));
                    //    list.Add(JsonConvert.DeserializeObject<RootObject>(json));
                    //}

                    //responseText = "";

                    //foreach (var item in list)
                    //{
                    //    responseText += item.Variable + " : " + item.Value + "\r\n";
                    //}

                    //outputBox.Text = list.GetType().ToString();

                    string stringToExclude = "";
                    JObject jo = JObject.Parse(responseText);

                    JObject[] match = jo["Results"].Values<JObject>().Where(m => m["Value"].Value<string>() != stringToExclude).ToArray();

                    var nullList = new List<string>();

                    //left off here  ##Clean up the output #Remove VariableId and ValueId
                    foreach (JProperty prop in match.Properties())
                    {

                        if (prop.Name == "Value")
                        {
                            outputString += "\r\n";
                        }

                        outputString += prop.Name + " : " + prop.Value + "\r\n";


                        outputBox.Text = outputString;
                    }
                }
                catch (Exception error)
                {
                    responseText = error.ToString();
                    outputBox.Text = responseText;
                }
            }
            else
            {
                outputBox.Text = "Error: VIN contains illegal characters, or is less than 11 or more than 17 characters long";
            }
        }
    }
}
