using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COVID_CSV_Parser
{
    public class DailyStateVaccinationTotals
    {
        public string Date;
        public string MMWR_week;
        public string Location;
        public string ShortName;
        public string LongName;
        public string Doses_Distributed;
        public string Doses_Administered;
        public string Dist_Per_100K;
        public string Admin_Per_100K;
        public string Census2019;
        public string Administered_Moderna;
        public string Administered_Pfizer;
        public string Administered_Unk_Manuf;
        public string Ratio_Admin_Dist;
    }
}
