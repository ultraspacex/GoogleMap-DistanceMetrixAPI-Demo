// Yutthana L. 
// ultraspacex@gmail.com
// 14-9-2018

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using RestSharp;

namespace GoogleMapAPI_Distance
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Mouse drag form
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // Form Drop shadow
        protected override CreateParams CreateParams
        {
            get
            {
                const int csDropshadow = 0x00020000;
                var cp = base.CreateParams;
                cp.ClassStyle |= csDropshadow;
                return cp;
            }
        }

        public async void GetSingleDistanceFromGoogleMapApi(string origins, string destination)
        {
            const string googleMapApiKey = "AIzaSyB9A_Z9HMQNbqHLUmSNyhR4bIDOEg2IiRY";
            const string units = "metric";
            const string avoids = "tolls,highways,indoor";
            const string language = "en-US";

            var client = new RestClient(
                "https://maps.googleapis.com/maps/api/distancematrix/json?mode=driving&origins=" + origins +
                "&destinations=" + destination +
                "&language=" + language +
                "&key=" + googleMapApiKey +
                "&units=" + units +
                "&avoid=" + avoids);
            var request = new RestRequest(Method.GET);
            var response = await client.ExecuteTaskAsync(request);

            var json = response.Content;

            try
            {
                var responseResult = JsonConvert.DeserializeObject<ResponseModel>(json);

                lnOrigins.Text = responseResult.OriginAddresses[0];
                lbDestination.Text = responseResult.DestinationAddresses[0];
                lbDistance.Text = responseResult.Rows[0].Elements[0].Distance.Text;
                lbTime.Text = responseResult.Rows[0].Elements[0].Duration.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show("msg :: " + ex.InnerException, "Error");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
            {
                GetSingleDistanceFromGoogleMapApi(textBox1.Text, textBox2.Text);
            }
            else
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    textBox1.Focus();
                }

                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    textBox2.Focus();
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class ResponseModel
    {
        public string Status { get; set; }

        [JsonProperty(PropertyName = "destination_addresses")]
        public string[] DestinationAddresses { get; set; }

        [JsonProperty(PropertyName = "origin_addresses")]
        public string[] OriginAddresses { get; set; }

        public Row[] Rows { get; set; }

        public class Data
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        public class Element
        {
            public string Status { get; set; }
            public Data Duration { get; set; }
            public Data Distance { get; set; }
        }

        public class Row
        {
            public Element[] Elements { get; set; }
        }
    }
}
