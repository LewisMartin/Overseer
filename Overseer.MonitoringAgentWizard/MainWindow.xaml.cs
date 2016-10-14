using Newtonsoft.Json;
using Overseer.DTOs.MonitoringConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Overseer.MonitoringAgentWizard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Configure_Click(object sender, RoutedEventArgs e)
        {
            // disable button
            ConfigureButton.IsEnabled = false;
            StatusMsg.Content = "Validating with server..";

            // form the api url
            string AppUrl = tbServerAddress.Text;
            string APIUrl = AppUrl + "/api/MonitoringConfigEndpoint/AuthenticateMonitoringAgentConfiguration";

            string APIResponse = await MakeMonitoringConfigRequest(APIUrl, tbMachineGUID.Text, tbUserName.Text, tbPassword.Password);

            MonitoringAgentConfigResponse response = JsonConvert.DeserializeObject<MonitoringAgentConfigResponse>(APIResponse);

            StatusMsg.Content = "Result: " + response.Success.ToString() + "\nSecret:" + response.MachineSecret;
            ConfigureButton.IsEnabled = true;
        }

        // asynchronous method to make request to server
        private async Task<string> MakeMonitoringConfigRequest(string apiUrl, string machineGUID, string UserName, string Password)
        {
            // wait for a bit to check UI responsiveness
            await Task.Delay(3000);

            // package the user input into our class to be serialized & posted
            MonitoringAgentConfigRequest req = new MonitoringAgentConfigRequest()
            {
                MachineGuid = new Guid(machineGUID),
                Username = UserName,
                Password = Password
            };

            // serializing our request message to json string
            string jsonData = string.Format("={0}", JsonConvert.SerializeObject(req));

            // forming the content of the POST request
            var postContent = new StringContent(
                jsonData,
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            using (var httpClient = new HttpClient())
            {
                // setting up the http client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // making the request - posting the data
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, postContent);
                string responseString = await response.Content.ReadAsStringAsync();

                // return the content of the response as a string
                return responseString;
            }
        }
    }
}
