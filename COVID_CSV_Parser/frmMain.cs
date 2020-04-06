using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileHelpers;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.IO;
using System.Globalization;
using System.Timers;

namespace COVID_CSV_Parser
{
    // Set options for FileHelpers class
    [DelimitedRecord(",")]
    [IgnoreEmptyLines()]
    [IgnoreFirst()]

    public partial class frmMain : Form
    {
        // Setup configuration
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        // Instantiate SQL connection & command
        public static SqlConnection conn = new SqlConnection(config.AppSettings.Settings["sqlConnString"].Value);
        public static SqlCommand cmd = new SqlCommand();

        // For file selector
        string defaultCSVFileDirectory = config.AppSettings.Settings["defaultCSVFileDirectory"].Value;

        // Schedule timer
        static System.Timers.Timer timer;
        static DateTime nowTime = DateTime.Now;
        static DateTime scheduledTime;
        
        public frmMain()
        {
            InitializeComponent();

            openFileDialog.InitialDirectory = defaultCSVFileDirectory;

            // Start the schedule time
            schedule_Timer();
        }

        // Method to get CSV file from specified URL - currently not used
        public string GetCSV(string url)
        {
            // These directives needed to prevent security error on HTTP request
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        // Method to use a WebClient to pull a text file from a specified URL
        public void GetCSVFileFromURL(string myURL, string myFilePath)
        {
            // These directives needed to prevent security error on HTTP request
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var client = new WebClient())
            {
                client.DownloadFile(myURL, myFilePath);
            }
        }

        public Int32 GetCubeRootColorIndex(Int32 inputValue)
        {
            Double result = 0;

            // Calculation we're using is cube root of value divided by 4
            result = Math.Round((Math.Pow(inputValue, (double)1 / 3)) / 4);

            // Clamp to max value of 10
            if (result > 10) result = 10;

            return (Int32)result;
        }

        // Method to read in the data file and get the latest results
        public void GetLatestData(Boolean downloadLatestData, string dataFilename)
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection();
                DataTable tempRawdataTable = new DataTable();
                SqlCommand cmd = new SqlCommand();

                //sqlConnection.ConnectionString = "Data Source=FNC-SQL-PRI;Initial Catalog=CoronaVirusData_Test;Persist Security Info=True;User ID=sa;Password=Engineer@1";
                sqlConnection.ConnectionString = config.AppSettings.Settings["sqlConnString"].Value;

                // Setup stopwatch
                System.Diagnostics.Stopwatch elapsed = new System.Diagnostics.Stopwatch();
                elapsed.Start();

                // Instantiate parser engine
                var engine = new FileHelperEngine<CovidDataRecord>();

                if (downloadLatestData)
                {
                    // Download the file first and store to c:\temp
                    // Build the URL with the current date string in the filename
                    // NOTE: Temporarily hard-wired to use yesterday's date
                    string csvURL = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/" +
                        DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy") + ".csv";

                    GetCSVFileFromURL(csvURL, defaultCSVFileDirectory + "\\LatestCovidData.csv");

                    // Parse the downloaded file
                    var records = engine.ReadFile(defaultCSVFileDirectory + "\\LatestCovidData.csv");

                    Int32 rowCount = 0;

                    logTxt.Clear();
                    foreach (var record in records)
                    {
                        if ((record.FIPS.ToString().Trim() != string.Empty) && (record.FIPS >= 1000) && (record.FIPS < 60000))
                        {
                            string logText = "FIPS: " + record.FIPS.ToString() + " | ";
                            logText += "State: " + record.Province_State + " | ";
                            logText += "County: " + record.Admin2 + " | ";
                            logText += "Confirmed: " + record.Confirmed.ToString() + " | ";
                            logText += "Deaths: " + record.Deaths.ToString() + " | ";
                            logText += "Recovered: " + record.Recovered.ToString() + Environment.NewLine;

                            if (chkShowLogData.Checked)
                            {
                                logTxt.AppendText(logText);
                            }
                            rowCount++;

                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", record.FIPS));
                                cmd.Parameters.Add(new SqlParameter("@County", record.Admin2));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", record.Lat ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", record.Long_ ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {

                                txtStatus.Text = "Error occurred during database posting: " + e.Message;
                            }
                            sqlConnection.Close();
                        }
                    }
                    // Display stats for processing
                    elapsed.Stop();
                    logTxt.AppendText(Environment.NewLine);
                    logTxt.AppendText("Last update processed: " + DateTime.Now.ToString() + Environment.NewLine);
                    logTxt.AppendText("Data rows processed: " + rowCount.ToString() + Environment.NewLine);
                    logTxt.AppendText("Total elapsed time (seconds): " + elapsed.Elapsed.TotalSeconds + Environment.NewLine);
                    logTxt.AppendText("Current Data File Date: " + DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy"));
                    txtStatus.Text = "Data update completed successfully at: " + DateTime.Now.ToString();
                }
                else
                {
                    // Parse the specified/selected file
                    var records = engine.ReadFile(dataFilename);

                    Int32 rowCount = 0;

                    logTxt.Clear();
                    foreach (var record in records)
                    {
                        if ((record.FIPS.ToString().Trim() != string.Empty) && (record.FIPS >= 1000) && (record.FIPS < 60000))
                        {
                            string logText = "FIPS: " + record.FIPS.ToString() + " | ";
                            logText += "State: " + record.Province_State + " | ";
                            logText += "County: " + record.Admin2 + " | ";
                            logText += "Confirmed: " + record.Confirmed.ToString() + " | ";
                            logText += "Deaths: " + record.Deaths.ToString() + " | ";
                            logText += "Recovered: " + record.Recovered.ToString() + Environment.NewLine;

                            if (chkShowLogData.Checked)
                            {
                                logTxt.AppendText(logText);
                            }
                            rowCount++;

                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", record.FIPS));
                                cmd.Parameters.Add(new SqlParameter("@County", record.Admin2));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update?? DateTime.Now.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", record.Lat ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", record.Long_ ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {

                                txtStatus.Text = "Error occurred during database posting: " + e.Message;
                            }
                            sqlConnection.Close();
                        }
                    }
                    // Display stats for processing
                    elapsed.Stop();
                    logTxt.AppendText(Environment.NewLine);
                    logTxt.AppendText("Last update processed: " + DateTime.Now.ToString() + Environment.NewLine);
                    logTxt.AppendText("Data rows processed: " + rowCount.ToString() + Environment.NewLine);
                    logTxt.AppendText("Total elapsed time (seconds): " + elapsed.Elapsed.TotalSeconds + Environment.NewLine);
                    logTxt.AppendText("Current Data File Date: " + DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy"));
                    txtStatus.Text = "Data update completed successfully at: " + DateTime.Now.ToString();
                }
            }
            catch (Exception e)
            {
                txtStatus.Text = "Error occurred during data retrieval and database posting: " + e.Message;
            }
        }

        // Handler for button to force getting latest data
        private void btnGetData_Click(object sender, EventArgs e)
        {
            // Call method with flag set to download the latest data; if false, filename is specified as 2nd parameter
            GetLatestData(true, String.Empty);
        }

        // Method to launch a file picker and process the selected file from the default data file folder
        private void btnProcessSelectedFile_Click(object sender, EventArgs e)
        {
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.FileName = "LatestCovidData.csv";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Set the filename
                txtSelectedFilename.Text = openFileDialog.FileName;

                // Call method to process the file
                GetLatestData(false, openFileDialog.FileName);
            }
        }

        // Handler to download data for the specified date
        private void btnDownloadFile_Click(object sender, EventArgs e)
        {
            // Get the selected date
            DateTime selectedDate = monthCalendar.SelectionRange.Start;

            // Check to see if today's date was selected and it's not yet 8:00 PM EDT
            TimeSpan start = new TimeSpan(0, 0, 0); //midnight
            TimeSpan end = new TimeSpan(21, 0, 0); //9:00 PM - 1 hour buffer after 8:00 PM 
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (((now > start) && (now < end)) && (selectedDate == DateTime.Now.Date))
            {
                // Invalid data
                string message = "You cannot request today's data until after 9:00 PM EDT";
                string title = "Error";
                MessageBox.Show(message, title);
                return;
            }

            string csvURL = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/" +
                selectedDate.ToString("MM-dd-yyyy") + ".csv";

            GetCSVFileFromURL(csvURL, defaultCSVFileDirectory + "\\CovidData_" + selectedDate.ToString("MM-dd-yyyy") + ".csv");
            lblFileProcessed.Text = "CovidData_" + selectedDate.ToString("MM-dd-yyyy") + ".csv";
        }

        // Here's the scheduling timer - fire at 9:00 PM
        //static void schedule_Timer()
        void schedule_Timer()
        {
            Console.WriteLine("### Timer Started ###");

            //DateTime nowTime = DateTime.Now;
            //DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 20, 14, 0, 0); // Start at 8:10 PM
            nowTime = DateTime.Now;
            scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 20, 32, 0, 0); // Start at 8:10 PM

            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            timer = new System.Timers.Timer(tickTime);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        //static void timer_Elapsed(object sender, ElapsedEventArgs e)
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Call method with flag set to download the latest data; if false, filename is specified as 2nd parameter
            GetLatestData(true, String.Empty);

            // Stop the timer
            timer.Stop();

            // Restart
            schedule_Timer();
        }

    }
}
