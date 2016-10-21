using Newtonsoft.Json;
using Overseer.DTOs.MonitoringConfig;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;

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
            // disable form elements
            ConfigureButton.IsEnabled = false;
            tbMachineGUID.IsEnabled = false;
            tbServerAddress.IsEnabled = false;
            tbUserName.IsEnabled = false;
            tbPassword.IsEnabled = false;

            StatusMsg.Content = "Validating with server..";

            // form the api url
            string AppUrl = tbServerAddress.Text;
            string APIUrl = AppUrl + "/api/MonitoringConfigEndpoint/AuthenticateMonitoringAgentConfiguration";

            string APIResponse = await MakeMonitoringConfigRequest(APIUrl, tbMachineGUID.Text, tbUserName.Text, tbPassword.Password);

            if (APIResponse != null)
            {
                MonitoringAgentConfigResponse response = JsonConvert.DeserializeObject<MonitoringAgentConfigResponse>(APIResponse);

                if (response.Success)
                {
                    // encrypt using DPAPI & write secret to xml config file
                    bool success = WriteToConfigFile(tbMachineGUID.Text, response.MachineSecret, tbServerAddress.Text);

                    if (success)
                    {
                        StatusMsg.Content = "Configuration completed sucessfully.";
                    }
                    else
                    {
                        StatusMsg.Content = "Error occured writing configuration to file.";
                    }
                }
                else
                {
                    // some field was wrong - output reason & highlight incorrect fields
                    StatusMsg.Content = "Configuration unsuccessful.";
                }
            }
            else
            {
                StatusMsg.Content = "Unable to connect to server.";
            }

            // re-enable form elements
            tbMachineGUID.IsEnabled = true;
            tbServerAddress.IsEnabled = true;
            tbUserName.IsEnabled = true;
            tbPassword.IsEnabled = true;
            ConfigureButton.IsEnabled = true;
        }

        // asynchronous method to make request to server
        private async Task<string> MakeMonitoringConfigRequest(string apiUrl, string machineGUID, string UserName, string Password)
        {
            // wait for a bit to check UI responsiveness
            //await Task.Delay(3000);

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
                string responseString;

                // setting up the http client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    // making the request - posting the data
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, postContent);
                    response.EnsureSuccessStatusCode(); // this will throw an exception if http response contains an unsuccessful status code

                    responseString = await response.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    responseString = null;
                }

                // return the content of the response as a string
                return responseString;
            }
        }

        // method for writing secret to monitoring agent's xml config file
        private bool WriteToConfigFile(string machineGUID, string machineSecret, string serverAddress)
        {
            string configFileLocation = @"C:\MonitoringAgentTest\MonitoringAgentConfig.xml";

            XmlDocument configXml = new XmlDocument();

            if(File.Exists(configFileLocation))
            {
                configXml.Load(configFileLocation);

                XmlNode root = configXml.DocumentElement;
                XmlNode mGUID = root.SelectSingleNode("descendant::MachineGUID");
                XmlNode mSecret = root.SelectSingleNode("descendant::MachineSecret");
                XmlNode sAddress = root.SelectSingleNode("descendant::ServerAddress");

                mGUID.InnerText = machineGUID;
                mSecret.InnerText = machineSecret;
                sAddress.InnerText = serverAddress;

                configXml.Save(configFileLocation);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
