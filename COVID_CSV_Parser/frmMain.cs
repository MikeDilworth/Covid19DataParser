using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        // Declare background worker threads for state & county-level data fetches
        private BackgroundWorker backgroundWorkerStateLevel;
        private BackgroundWorker backgroundWorkerCountyLevel;
        // Added for vaccination data
        private BackgroundWorker backgroundWorkerVaccinationStateLevel;

        public frmMain()
        {
            InitializeComponent();

            openFileDialog.InitialDirectory = defaultCSVFileDirectory;

            // Start the schedule timer
            schedule_Timer();

            // Setup the background worker threads
            backgroundWorkerStateLevel = new BackgroundWorker();
            backgroundWorkerStateLevel.DoWork += new DoWorkEventHandler(BackgroundWorkerStateLevel_DoWork);
            backgroundWorkerStateLevel.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorkerStateLevel_RunWorkerCompleted);
            backgroundWorkerCountyLevel = new BackgroundWorker();
            backgroundWorkerCountyLevel.DoWork += new DoWorkEventHandler(BackgroundWorkerCountyLevel_DoWork);
            backgroundWorkerCountyLevel.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorkerCountyLevel_RunWorkerCompleted);
            //Added for vaccination data
            backgroundWorkerVaccinationStateLevel = new BackgroundWorker();
            backgroundWorkerVaccinationStateLevel.DoWork += new DoWorkEventHandler(BackgroundWorkerVaccinationStateLevel_DoWork);
            backgroundWorkerVaccinationStateLevel.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorkerVaccinationStateLevel_RunWorkerCompleted);
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

        // Method to create datetime string from encoded date only string
        public string GetDateTimeString(string dateStringIn)
        {
            string dateTimeString = string.Empty;

            if (dateStringIn.Length == 8)
            {
                dateTimeString = dateStringIn.Substring(0, 4) + "-" + dateStringIn.Substring(4, 2) + "-" + dateStringIn.Substring(6, 2) + " 00:00:00";
            }

            return dateTimeString;
        }

        /// <summary>
        /// Event handlers for the state-level data worker thread
        /// </summary>
        // Handler for state level data background thread work completed
        private void BackgroundWorkerStateLevel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logTxt.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                logTxt.AppendText(e.Result.ToString());
            });
        }

        // Handler for getting the latest state-level data (historical)
        private void btnGetLatestStateData_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerStateLevel.IsBusy != true)
            {
                backgroundWorkerStateLevel.RunWorkerAsync();
            }
        }

        // Worker thread method to read in the latest state-level data file and post the results to the SQL DB
        private void BackgroundWorkerStateLevel_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Setup stopwatch
                System.Diagnostics.Stopwatch elapsed = new System.Diagnostics.Stopwatch();
                elapsed.Start();

                // These directives needed to prevent security error on HTTP request
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new RestClient("http://covidtracking.com/api/v1/states/daily.json");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                // Create list for data records
                var DailyDataList = new List<DailyStateTotals>();

                var StateDataAr = JArray.Parse(response.Content);

                Int32 rowCount = 0;
                //logTxt.Clear();

                foreach (JObject a in StateDataAr)
                {
                    var StateData = JObject.Parse(a.ToString());

                    DailyStateTotals foo = new DailyStateTotals
                    {
                        date = (string)StateData.SelectToken("date"),
                        state = (string)StateData.SelectToken("state"),
                        positive = (string)StateData.SelectToken("positive"),
                        negative = (string)StateData.SelectToken("negative"),
                        deaths = (string)StateData.SelectToken("death"),
                        hospitalized = (string)StateData.SelectToken("hospitalized"),
                        total = (string)StateData.SelectToken("total"),
                        totalResults = (string)StateData.SelectToken("totalTestResults"),
                        fips = (string)StateData.SelectToken("fips"),
                        deathInc = (string)StateData.SelectToken("deathIncrease"),
                        hospInc = (string)StateData.SelectToken("hospitalizedIncrease"),
                        negativeInc = (string)StateData.SelectToken("negativeIncrease"),
                        positiveInc = (string)StateData.SelectToken("positiveIncrease"),
                        totalTestResultsInc = (string)StateData.SelectToken("totalTestResultsIncrease")
                    };

                    DailyDataList.Add(foo);
                    rowCount++;
                }

                SqlConnection conn = new SqlConnection(config.AppSettings.Settings["sqlConnString"].Value);
                conn.Open();
                SqlCommand cmd;

                foreach (DailyStateTotals b in DailyDataList)
                {
                    cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_State"].Value, conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                    };

                    //ADD PARAMETER NAMES IN FIRST ARGUMENT VALUES
                    cmd.Parameters.AddWithValue("@date", GetDateTimeString(b.date));
                    cmd.Parameters.AddWithValue("@state", b.state ?? "");
                    cmd.Parameters.AddWithValue("@positive", b.positive ?? "");
                    cmd.Parameters.AddWithValue("@negative", b.negative ?? "");
                    cmd.Parameters.AddWithValue("@deaths", b.deaths ?? "");
                    cmd.Parameters.AddWithValue("@hospitalized", b.hospitalized ?? "");
                    cmd.Parameters.AddWithValue("@total", b.total ?? "");
                    cmd.Parameters.AddWithValue("@totalResults", b.totalResults ?? "");
                    cmd.Parameters.AddWithValue("@fips", b.fips ?? "0");
                    cmd.Parameters.AddWithValue("@deathInc", b.deathInc ?? "");
                    cmd.Parameters.AddWithValue("@hospInc", b.hospInc ?? "");
                    cmd.Parameters.AddWithValue("@negativeInc", b.negativeInc ?? "");
                    cmd.Parameters.AddWithValue("@positiveInc", b.positiveInc ?? "");
                    cmd.Parameters.AddWithValue("@totalTestResultsInc", b.totalTestResultsInc ?? "");

                    cmd.ExecuteNonQuery();
                }

                conn.Close();

                // Display stats for processing
                elapsed.Stop();

                string resultString = string.Empty;
                resultString += "STATE LEVEL DATA - LATEST DATA" + Environment.NewLine;
                resultString += "Last state data update processed: " + DateTime.Now.ToString() + Environment.NewLine;
                resultString += "Data rows processed: " + rowCount.ToString() + Environment.NewLine;
                resultString += "Total elapsed time (seconds): " + elapsed.Elapsed.TotalSeconds + Environment.NewLine;
                resultString += "State-Level data update completed successfully at: " + DateTime.Now.ToString();
                resultString += Environment.NewLine + Environment.NewLine;
                
                /*
                // Write out a basic JSON data file
                string json = JsonConvert.SerializeObject(response.Content, Formatting.Indented);
                string filePath = @"c:\temp\LatestStateLevelData.json";
                //File.WriteAllText(filePath, json);
                */

                e.Result = resultString;
            }
            catch (Exception ex)
            {
                txtStatus.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    txtStatus.Text = "Error occurred during state-level data retrieval and database posting: " + ex.Message;
                });
            }
        }

        /// <summary>
        /// Event handlers for the state-level vaccination data worker thread
        /// </summary>
        // Handler for state level vaccination data background thread work completed
        private void BackgroundWorkerVaccinationStateLevel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logTxt.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                logTxt.AppendText(e.Result.ToString());
            });
        }

        // Handler for manual update button for vaccination data
        private void btnGetLatestStateVaccinationData_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerVaccinationStateLevel.IsBusy != true)
            {
                backgroundWorkerVaccinationStateLevel.RunWorkerAsync();
            }

        }

        // Worker thread method to read in the latest state-level vaccination data file and post the results to the SQL DB
        private void BackgroundWorkerVaccinationStateLevel_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Setup stopwatch
                System.Diagnostics.Stopwatch elapsed = new System.Diagnostics.Stopwatch();
                elapsed.Start();

                // These directives needed to prevent security error on HTTP request
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new RestClient("https://covid.cdc.gov/covid-data-tracker/COVIDData/getAjaxData?id=vaccination_data");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                // Create list for data records
                var DailyVaccinationDataList = new List<DailyStateVaccinationTotals>();

                //var StateVaccinationDataAr = JArray.Parse(response.Content);
                var tempData = JObject.Parse(response.Content);

                JArray StateVaccinationDataAr = (JArray)tempData["vaccination_data"];

                //var StateVaccinationDataAr = tempData.SelectTokens("vaccination_data");

                Int32 rowCount = 0;
                //logTxt.Clear();

                foreach (JObject a in StateVaccinationDataAr)
                {
                    var StateVaccinationData = JObject.Parse(a.ToString());

                    DailyStateVaccinationTotals dsvt = new DailyStateVaccinationTotals
                    {
                        Date = (string)StateVaccinationData.SelectToken("Date"),
                        MMWR_week = (string)StateVaccinationData.SelectToken("MMWR_week"),
                        Location = (string)StateVaccinationData.SelectToken("Location"),
                        ShortName= (string)StateVaccinationData.SelectToken("ShortName"),
                        LongName = (string)StateVaccinationData.SelectToken("LongName"),
                        Doses_Distributed = (string)StateVaccinationData.SelectToken("Doses_Distributed"),
                        Doses_Administered = (string)StateVaccinationData.SelectToken("Doses_Administered"),
                        Dist_Per_100K = (string)StateVaccinationData.SelectToken("Dist_Per_100K"),
                        Admin_Per_100K = (string)StateVaccinationData.SelectToken("Admin_Per_100K"),
                        Census2019 = (string)StateVaccinationData.SelectToken("Census2019"),
                        Administered_Moderna = (string)StateVaccinationData.SelectToken("Administered_Moderna"),
                        Administered_Pfizer = (string)StateVaccinationData.SelectToken("Administered_Pfizer"),
                        Administered_Unk_Manuf = (string)StateVaccinationData.SelectToken("Administered_Unk_Manuf"),
                        Ratio_Admin_Dist = (string)StateVaccinationData.SelectToken("Ratio_Admin_Dist")
                    };

                    DailyVaccinationDataList.Add(dsvt);
                    rowCount++;
                }

                SqlConnection conn = new SqlConnection(config.AppSettings.Settings["sqlConnString"].Value);
                conn.Open();
                SqlCommand cmd;

                foreach (DailyStateVaccinationTotals d in DailyVaccinationDataList)
                {
                    //cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_Vaccination_State"].Value, conn)
                    cmd = new SqlCommand("insertParsedJSONStateVaccinationData", conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                    };

                    //ADD PARAMETER NAMES IN FIRST ARGUMENT VALUES
                    //cmd.Parameters.AddWithValue("@Date", GetDateTimeString(d.Date));
                    cmd.Parameters.AddWithValue("@Date", d.Date ?? "");
                    cmd.Parameters.AddWithValue("@MMWR_week", d.MMWR_week ?? "");
                    cmd.Parameters.AddWithValue("@Location", d.Location ?? "");
                    cmd.Parameters.AddWithValue("@ShortName", d.ShortName ?? "");
                    cmd.Parameters.AddWithValue("@LongName", d.LongName ?? "");
                    cmd.Parameters.AddWithValue("@Doses_Distributed", d.Doses_Distributed ?? "");
                    cmd.Parameters.AddWithValue("@Doses_Administered", d.Doses_Administered ?? "");
                    cmd.Parameters.AddWithValue("@Dist_Per_100K", d.Dist_Per_100K ?? "");
                    cmd.Parameters.AddWithValue("@Admin_Per_100K", d.Admin_Per_100K ?? "");
                    cmd.Parameters.AddWithValue("@Census2019", d.Census2019 ?? "");
                    cmd.Parameters.AddWithValue("@Administered_Moderna ", d.Administered_Moderna ?? "");
                    cmd.Parameters.AddWithValue("@Administered_Pfizer ", d.Administered_Pfizer ?? "");
                    cmd.Parameters.AddWithValue("@Administered_Unk_Manuf", d.Administered_Unk_Manuf ?? "");
                    cmd.Parameters.AddWithValue("@Ratio_Admin_Dist ", d.Ratio_Admin_Dist ?? "");

                    cmd.ExecuteNonQuery();
                }

                conn.Close();

                // Display stats for processing
                elapsed.Stop();

                string resultString = string.Empty;
                resultString += "STATE LEVEL VACCINTAION DATA - LATEST DATA" + Environment.NewLine;
                resultString += "Last state vaccination data update processed: " + DateTime.Now.ToString() + Environment.NewLine;
                resultString += "Data rows processed: " + rowCount.ToString() + Environment.NewLine;
                resultString += "Total elapsed time (seconds): " + elapsed.Elapsed.TotalSeconds + Environment.NewLine;
                resultString += "State-Level vaccination data update completed successfully at: " + DateTime.Now.ToString();
                resultString += Environment.NewLine + Environment.NewLine;

                /*
                // Write out a basic JSON data file
                string json = JsonConvert.SerializeObject(response.Content, Formatting.Indented);
                string filePath = @"c:\temp\LatestStateLevelData.json";
                //File.WriteAllText(filePath, json);
                */

                e.Result = resultString;
            }
            catch (Exception ex)
            {
                txtStatus.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    txtStatus.Text = "Error occurred during state-level vaccination data retrieval and database posting: " + ex.Message;
                });
            }
        }

        /// <summary>
        /// Event handlers for the county-level data worker thread
        /// </summary>
        // Handler for state level data background thread work completed
        private void BackgroundWorkerCountyLevel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logTxt.Invoke((MethodInvoker)delegate {
                // Running on the UI thread
                logTxt.AppendText(e.Result.ToString());
            });
        }

        // Handler for button to force getting latest county-level data
        private void btnGetData_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerCountyLevel.IsBusy != true)
            {
                backgroundWorkerCountyLevel.RunWorkerAsync();
            }
        }

        // Method to read in the latest county-level data file and post the results to the SQL DB
        //private void GetLatestData_County(Boolean downloadLatestData, string dataFilename)
        private void BackgroundWorkerCountyLevel_DoWork(object sender, DoWorkEventArgs e)
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

                // Download the file first and store to c:\temp
                // Build the URL with the current date string in the filename
                // NOTE: Hard-wired to use yesterday's date
                string csvURL = @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_daily_reports/" +
                    DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy") + ".csv";

                GetCSVFileFromURL(csvURL, defaultCSVFileDirectory + "\\LatestCovidData.csv");

                // Parse the downloaded file
                var records = engine.ReadFile(defaultCSVFileDirectory + "\\LatestCovidData.csv");

                // Set fixes timestamp
                string timestamp = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd HH:mm:ss");

                Int32 rowCount = 0;

                //logTxt.Clear();
                foreach (var record in records)
                {
                    if ((record.FIPS.ToString().Trim() != string.Empty) && (record.FIPS >= 1000) && (record.FIPS < 60000))
                    {
                        /*
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
                        */
                        rowCount++;

                        sqlConnection.Open();
                        // Call stored procedure for each record to append data to database
                        try
                        {
                            cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("@FIPS", record.FIPS));
                            if (record.FIPS == 36061)
                                cmd.Parameters.Add(new SqlParameter("@County", "Manhattan"));
                            else
                                cmd.Parameters.Add(new SqlParameter("@County", record.Admin2));
                            cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                            cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                            //cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                            cmd.Parameters.Add(new SqlParameter("@Update_Time", timestamp));
                            cmd.Parameters.Add(new SqlParameter("@Latitude", record.Lat ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Longitude", record.Long_ ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                            //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            txtStatus.Invoke((MethodInvoker)delegate {
                                // Running on the UI thread
                                txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                            });
                        }
                        sqlConnection.Close();

                        // If the record is for NYC, append 4 additional records for the 4 boroughs not reported
                        if (record.FIPS == 36061)
                        {
                            // Bronx County (FIPS = 36005)
                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", 36005));
                                cmd.Parameters.Add(new SqlParameter("@County", "The Bronx"));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                //cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", timestamp));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", 40.8448));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", -73.8648));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                txtStatus.Invoke((MethodInvoker)delegate {
                                    // Running on the UI thread
                                    txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                                });
                            }
                            sqlConnection.Close();

                            // Kings County (Brooklyn)  (FIPS = 36047)
                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", 36047));
                                cmd.Parameters.Add(new SqlParameter("@County", "Brooklyn"));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                //cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", timestamp));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", 40.6782));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", -73.9442));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                txtStatus.Invoke((MethodInvoker)delegate {
                                    // Running on the UI thread
                                    txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                                });
                            }
                            sqlConnection.Close();

                            // Queens County  (FIPS = 36081)
                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", 36081));
                                cmd.Parameters.Add(new SqlParameter("@County", "Queens"));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                //cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", timestamp));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", 40.7282));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", -73.7949));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                txtStatus.Invoke((MethodInvoker)delegate {
                                    // Running on the UI thread
                                    txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                                });
                            }
                            sqlConnection.Close();

                            // Richmond County (Staten Island)  (FIPS = 36085)
                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", 36085));
                                cmd.Parameters.Add(new SqlParameter("@County", "Staten Island"));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                //cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", timestamp));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", 40.5795));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", -74.1502));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                txtStatus.Invoke((MethodInvoker)delegate {
                                    // Running on the UI thread
                                    txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                                });
                            }
                            sqlConnection.Close();
                        }
                    }
                }
                // Display stats for processing
                elapsed.Stop();

                string resultString = string.Empty;
                resultString += "COUNTY LEVEL DATA - LATEST DATA" + Environment.NewLine;
                resultString += "Last county data update processed: " + DateTime.Now.ToString() + Environment.NewLine;
                resultString += "Data rows processed: " + rowCount.ToString() + Environment.NewLine;
                resultString += "Total elapsed time (seconds): " + elapsed.Elapsed.TotalSeconds + Environment.NewLine;
                resultString += "County-Level data update completed successfully at: " + DateTime.Now.ToString();
                resultString += Environment.NewLine + Environment.NewLine;

                e.Result = resultString;
            }
            catch (Exception ex)
            {
                txtStatus.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    txtStatus.Text = "Error occurred during county-level data retrieval and database posting: " + ex.Message;
                });
            }
        }

        // Method to get the county-level data for the specified date
        private void GetDataForSpecifiedDateCountyLevel(string dataFilename)
        {
            Int32 rowCount = 0;

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
                //engine.ErrorManager.ErrorMode

                // Parse the specified/selected file
                var records = engine.ReadFile(dataFilename);

                string baseFilename = Path.GetFileNameWithoutExtension(dataFilename);
                string year = baseFilename.Substring(16, 4);
                string month = baseFilename.Substring(10, 2);
                string day = baseFilename.Substring(13, 2);
                string timeStamp = year + "-" + month + "-" + day + " 23:59:59";

                //logTxt.Clear();
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
                            cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add(new SqlParameter("@FIPS", record.FIPS));
                            if (record.FIPS == 36061)
                                cmd.Parameters.Add(new SqlParameter("@County", "Manhattan"));
                            else
                                cmd.Parameters.Add(new SqlParameter("@County", record.Admin2));
                            cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                            cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                            //cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                            cmd.Parameters.Add(new SqlParameter("@Update_Time", timeStamp));
                            cmd.Parameters.Add(new SqlParameter("@Latitude", record.Lat ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Longitude", record.Long_ ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                            cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                            //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {

                            txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                        }
                        sqlConnection.Close();

                        // If the record is for NYC, append 4 additional records for the 4 boroughs not reported
                        if (record.FIPS == 36061)
                        {
                            // Bronx County (FIPS = 36005)
                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", 36005));
                                cmd.Parameters.Add(new SqlParameter("@County", "The Bronx"));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", 40.8448));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", -73.8648));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {

                                txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                            }
                            sqlConnection.Close();

                            // Kings County (Brooklyn)  (FIPS = 36047)
                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", 36047));
                                cmd.Parameters.Add(new SqlParameter("@County", "Brooklyn"));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", 40.6782));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", -73.9442));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {

                                txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                            }
                            sqlConnection.Close();

                            // Queens County  (FIPS = 36081)
                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", 36081));
                                cmd.Parameters.Add(new SqlParameter("@County", "Queens"));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", 40.7282));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", -73.7949));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {

                                txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                            }
                            sqlConnection.Close();

                            // Richmond County (Staten Island)  (FIPS = 36085)
                            sqlConnection.Open();
                            // Call stored procedure for each record to append data to database
                            try
                            {
                                cmd = new SqlCommand(config.AppSettings.Settings["storedProcedure_County"].Value, sqlConnection);
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@FIPS", 36085));
                                cmd.Parameters.Add(new SqlParameter("@County", "Staten Island"));
                                cmd.Parameters.Add(new SqlParameter("@Province_State", record.Province_State));
                                cmd.Parameters.Add(new SqlParameter("@Country_Region", record.Country_Region));
                                cmd.Parameters.Add(new SqlParameter("@Update_Time", record.Last_Update));
                                cmd.Parameters.Add(new SqlParameter("@Latitude", 40.5795));
                                cmd.Parameters.Add(new SqlParameter("@Longitude", -74.1502));
                                cmd.Parameters.Add(new SqlParameter("@Confirmed", record.Confirmed ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Deaths", record.Deaths ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Recovered", record.Recovered ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Active", record.Active ?? 0));
                                cmd.Parameters.Add(new SqlParameter("@Combined_Key", record.Combined_Key));
                                //cmd.Parameters.Add(new SqlParameter("@ColorIndex_HeatMap", GetCubeRootColorIndex(record.Confirmed ?? 0)));
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {

                                txtStatus.Text = "Error occurred during database posting: " + ex.Message;
                            }
                            sqlConnection.Close();
                        }
                    }
                }
                // Display stats for processing
                elapsed.Stop();

                string resultString = string.Empty;
                resultString += "COUNTY LEVEL DATA - SELECTED DATA FILE" + Environment.NewLine;
                resultString += "Last county data update processed: " + DateTime.Now.ToString() + Environment.NewLine;
                resultString += "Data file name: " + dataFilename + Environment.NewLine;
                resultString += "Data rows processed: " + rowCount.ToString() + Environment.NewLine;
                resultString += "Total elapsed time (seconds): " + elapsed.Elapsed.TotalSeconds + Environment.NewLine;
                resultString += "County-Level data update completed successfully at: " + DateTime.Now.ToString();
                resultString += Environment.NewLine + Environment.NewLine;

                logTxt.AppendText(resultString);
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Error occurred during county-level data retrieval and database posting at row #" + rowCount.ToString() + ": " + ex.Message;
            }
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
                txtSelectedCountyFilename.Text = openFileDialog.FileName;

                // Call method to process the file
                GetDataForSpecifiedDateCountyLevel(openFileDialog.FileName);
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
            lblCountyFileProcessed.Text = "CovidData_" + selectedDate.ToString("MM-dd-yyyy") + ".csv";
        }

        // Here's the scheduling timer - fire at 9:00 PM
        //static void schedule_Timer()
        void schedule_Timer()
        {
            Console.WriteLine("### Timer Started ###");

            nowTime = DateTime.Now;
            scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 06, 00, 0, 0).AddDays(0); // Start at 6:00 AM tomorrow 

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
            // Start worker thread to get latest state-level data
            if (backgroundWorkerStateLevel.IsBusy != true)
            {
                backgroundWorkerStateLevel.RunWorkerAsync();
            }

            // Start worker thread to get latest state-level vaccinationdata
            if (backgroundWorkerVaccinationStateLevel.IsBusy != true)
            {
                backgroundWorkerVaccinationStateLevel.RunWorkerAsync();
            }

            // Start worker thread to get latest county-level data
            if (backgroundWorkerCountyLevel.IsBusy != true)
            {
                backgroundWorkerCountyLevel.RunWorkerAsync();
            }

            // Stop the timer
            timer.Stop();

            // Restart the schedule timer
            schedule_Timer();
        }
    }
}
