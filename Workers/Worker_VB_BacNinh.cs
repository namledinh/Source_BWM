using BWM.LIB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace BWM.Workers
{
    public class Worker_VB_BacNinh : Processor
    {
        string ConnectionString = ConfigurationManager.AppSettings["DBMySQLConStringLocal"];
        //string ConnectionString = "server = 10.3.10.51; uid=root;pwd=kqldu0cGzfoulvCRZO2S;database=api_thongkevanban_bacninh";
        const string tableThongKeDonVi = "tablethongkedonvi";
        const string tableThongKeVanBan = "tablethongkevanban";

        Dictionary<string, string> DictMatching;
        public Worker_VB_BacNinh(Worker Worker) : base(Worker)
        {
            Init();

            InitDictMatching(out DictMatching);
        }
        override public void InitSpeedAdapter()
        {
            SpeedAdapter.AddMilestone(1, 30);
            SpeedAdapter.AddMilestone(1, 20);
            SpeedAdapter.AddMilestone(1, 10);
            SpeedAdapter.AddMilestone(2, 10);
            SpeedAdapter.AddMilestone(2, 8);
            SpeedAdapter.AddMilestone(3, 4);
            SpeedAdapter.AddMilestone(4, 2);
            SpeedAdapter.AddMilestone(5, 1);
        }

        override public string DoProcess()
        {
            DateTime dateNow = DateTime.Now;
            DateTime dateStart = dateNow.AddMonths(-6);
            if (dateNow.Hour > 7) dateStart = dateNow.AddDays(-15);

            int datekey = int.Parse(dateNow.ToString("yyyyMMdd"));

            string msg = GetDataUNITS(datekey, out Dictionary<string, string> List_units);
            if (msg.Length > 0) WriteErrorLog($"[API THỐNG KÊ VĂN BẢN] " + msg);

            while (dateStart <= dateNow)
            {
                datekey = int.Parse(dateNow.ToString("yyyyMMdd"));
                string fromdate = dateNow.ToString("yyyy-MM-dd");
                string todate = fromdate;
                foreach (var unit_id in List_units)
                {
                    msg = GetDataQLVBDH(fromdate, todate, unit_id.Key, unit_id.Value, datekey);
                    if (msg.Length > 0) WriteErrorLog($"[API THỐNG KÊ VĂN BẢN] " + msg);
                }
                WriteLog($"[API THỐNG KÊ VĂN BẢN] {datekey}");
                dateNow = dateNow.AddDays(-1);
            }
            return msg;
        }

        protected string GetDataUNITS(int datekey, out Dictionary<string, string> List_units)
        {
            List_units = null;

            string api_url = $"http://qlvbdh.bacninh.gov.vn/api/units.bn";
            string msg = GetDataAPIHelper.APICall(api_url, out string json);
            if (msg.Length > 0) return $"Error when call api_url {api_url}, msg {msg}";

            msg = GenerateSQLCommandUNITS(json, datekey, out string cmdText, out List_units);
            if (msg.Length > 0) return $"Error when call GenerateSQLCommand api_url {api_url}, msg: {msg}";

            msg = ExcecuteCommand(cmdText);
            return msg;
        }
        protected string GetDataQLVBDH(string tu_ngay, string den_ngay, string unitCode, string unitName, int datekey)
        {
            const string url = "http://api.qlvbdh.bacninh.gov.vn/report/reportInByUnit.json";
            const string Authorization = "ZjhkMGQxYThmMDY0N2Q1YTUzMWZlY2M4ZWNjNjg1YzAzZmUwZDYyOGVjZWU2MGJjNzM2ZjcxZDA0ZjMxNjA1ODE0MWFmMjdkZWVmZDUzNmQ5NjA3MzkxMTNlZDlhMDA1MzFmNDM5NzgxMTc0NTY0NjMzOTc0MzU2Zjk5YTkwMTU=";
            string api_url = $"{url}?apikey={Authorization}&unit_id={unitCode}&from={tu_ngay}&to={den_ngay}";

            string msg = GetDataAPIHelper.APICall(api_url, out string json);
            if (msg.Length > 0) return $"Error when call api_url {api_url}, msg {msg}";

            msg = GenerateSQLCommandQLVBDH(json, unitCode, unitName, datekey, out string cmdText);
            if (msg.Length > 0) return $"Error when call GenerateSQLCommand api_url {api_url}, unitCode: {unitCode}, msg: {msg}";

            msg = ExcecuteCommand(cmdText);
            return msg;
        }
        string ExcecuteCommand(string cmdText)
        {
            try
            {
                if (cmdText.Length > 0)
                    cmdText = "chay cau lenh ben duoi";
                //    msg = TSQL.ExecuteCommand(ConnectionString, cmdText);
            }
            catch (Exception ex)
            {
                return $"Error in ExcecuteCommand, msg: {ex.Message}";
            }
            return "";
        }
        private string GenerateSQLCommandUNITS(string json, int datekey, out string cmdText, out Dictionary<string, string> List_units)
        {
            cmdText = "";

            List_units = null;

            var responseUNITS = JsonConvert.DeserializeObject<APIUnitsResponse>(json);
            if (responseUNITS == null) return $"response is empty ({json})";
            if (responseUNITS.data.units.Length <= 0) return $"responseUNITS.data.units.Length <= 0";

            List_units = new Dictionary<string, string>();
            foreach (var item in responseUNITS.data.units) List_units.Add(item.unit_code, item.unit_name);

            var msg = GetDataAPIHelper.Object2ColumnHeader(responseUNITS.data.units, new { datekey, SynDate = "" }, out string cmdHeader);
            if (msg.Length > 0) return msg;

            msg = GetDataAPIHelper.Object2SQLCommand(responseUNITS.data.units, new { datekey, i = "NOW()" }, out string cmdValues);
            if (msg.Length > 0) return msg;

            var cmdTextNew = $"DELETE FROM {tableThongKeDonVi} WHERE datekey = {datekey} ; INSERT INTO {tableThongKeDonVi} ({cmdHeader}) VALUES {cmdValues}";

            #region old code
            //if (responseUNITS.data.units.Length > 0)
            //{
            //    List_units = new Dictionary<string, string>();
            //    foreach (var item in responseUNITS.data.units)
            //    {
            //        List_units.Add(item.unit_code, item.unit_name);
            //        cmdText += $"('{item.id}', '{item.unit_code}', '{item.unit_name}', '{item.unit_ws}', '{item.db}', '{item.group_id}', '{item.parent_id}', '{item.path}', '{item.ws_code}', '{item.unit_link}', '{item.orders}', '{item.isdomain}', '{item.hidden}', '{datekey}', 'NOW()'), ";
            //    }
            //    cmdText = cmdText.Substring(0, cmdText.Length - 2);
            //}

            //if (cmdText.Length > 0) cmdText = $"INSERT INTO {tableThongKeDonVi} ('id', 'unit_code', 'unit_name', 'unit_ws', 'db', 'group_id', 'parent_id', 'path', 'ws_code', 'unit_link', 'orders', 'isdomain', 'hidden', 'datekey', 'SynDate') VALUES { cmdText.TrimEnd(',')}";

            //cmdText = $"DELETE FROM {tableThongKeDonVi} WHERE datekey = {datekey} ; {cmdText}";

            // Test xem code moi va code cu co sinh cung ket qua khong
            //int i = 0;
            //while (i < cmdText.Length && i < cmdTextNew.Length && cmdText[i] == cmdTextNew[i]) i++;
            //if (cmdText == cmdTextNew) i = 0;
            #endregion

            cmdText = cmdTextNew;

            return "";
        }
        private string GenerateSQLCommandQLVBDH(string json, string unitCode, string unitName, int datekey, out string cmdText)
        {
            cmdText = "";

            var responseQLVBDH = JsonConvert.DeserializeObject<APIResponse>(json);
            if (responseQLVBDH == null || responseQLVBDH.data == null || responseQLVBDH.data.total == null) return $"response is empty ({json})";
            if (responseQLVBDH.data.total.Length <= 0) return $"responseQLVBDH.data.total.Length <= 0";

            foreach (var item in responseQLVBDH.data.total)
            {
                item.unit_code = unitCode;
                item.unit_name = unitName;
                item.datekey = datekey;
                item.SynDate = "NOW()";
                if (DictMatching.ContainsKey(unitCode)) item.IDDB = DictMatching[unitCode];
            }

            var msg = GetDataAPIHelper.Object2ColumnHeader(responseQLVBDH.data.total, out string cmdHeader);
            if (msg.Length > 0) return msg;

            msg = GetDataAPIHelper.Object2SQLCommand(responseQLVBDH.data.total, out string cmdValues);
            if (msg.Length > 0) return msg;

            var cmdTextNew = $"DELETE FROM {tableThongKeVanBan} WHERE unit_code = '{unitCode}' and unit_name = '{unitName}' and datekey = {datekey}; INSERT INTO {tableThongKeVanBan} ({cmdHeader}) VALUES {cmdValues}";

            #region old code
            //if (responseQLVBDH.data.total.Length > 0)
            //{
            //    foreach (var item in responseQLVBDH.data.total)
            //    {
            //        cmdText += $"('{item.id}', '{item.username}', '{item.fullname}', '{item.title_code}', '{item.InNotOpen}', '{item.InNotProcess}', '{item.InProcessed}', '{item.InSaved}', '{item.InToKnownNotOpen}', '{item.InToKnown}', '{item.InTotal}', '{item.InternalNotOpen}', '{item.InternalTotal}', '{unitCode}', '{unitName}', '{datekey}', 'NOW()', '{DictMatching[unitCode]}'), ";
            //    }
            //    cmdText = cmdText.Substring(0, cmdText.Length - 2);
            //}

            //if (cmdText.Length > 0) cmdText = $"INSERT INTO {tableThongKeVanBan} ('id', 'username', 'fullname', 'title_code', 'TongVanBanDenChuaMo', 'TongVanBanDenChuaXuLy', 'TongVanBanDenDaXuLy', 'TongVanBanDenDaCat', 'TongVanBanDenDeBietChuaMo', 'TongVanBanDenDeBiet', 'TongVanBanDen', 'VBNoiBoChuaMo', 'TongVanBanNoiBo', 'unit_code', 'unit_name', 'datekey', 'SynDate', 'IDDB') " +
            //                        $"VALUES { cmdText.TrimEnd(',')}";

            //cmdText = $"DELETE FROM {tableThongKeVanBan} WHERE unit_code = '{unitCode}' and unit_name = '{unitName}' and datekey = {datekey}; {cmdText}";

            // Test xem code moi va code cu co sinh cung ket qua khong
            //int i = 0;
            //while (i < cmdText.Length && i < cmdTextNew.Length && cmdText[i] == cmdTextNew[i]) i++;
            //if (cmdText == cmdTextNew) i = 0;
            #endregion

            cmdText = cmdTextNew;

            return "";
        }

        private void InitDictMatching(out Dictionary<string, string> DictMatching)
        {
            DictMatching = new Dictionary<string, string>();
            DictMatching.Add("000.00.14.H05", "22");
            DictMatching.Add("000.01.14.H05", "19");
            DictMatching.Add("000.00.01.H05", "19");
            DictMatching.Add("000.00.01.K05", "4");
            DictMatching.Add("000.00.09.H05", "6");
            DictMatching.Add("000.00.05.H05", "21");
            DictMatching.Add("000.00.15.H05", "18");
            DictMatching.Add("000.00.06.H05", "17");
            DictMatching.Add("000.00.13.H05", "14");
            DictMatching.Add("000.00.04.H05", "13");
            DictMatching.Add("000.00.17.H05", "3");
            DictMatching.Add("000.00.10.H05", "2");
            DictMatching.Add("000.00.12.H05", "10");
            DictMatching.Add("000.00.02.H05", "5");
            DictMatching.Add("000.00.03.H05", "9");
            DictMatching.Add("000.00.07.H05", "25");
            DictMatching.Add("000.00.16.H05", "26");
            DictMatching.Add("000.00.18.H05", "7");
            DictMatching.Add("000.00.20.H05", "15");
            DictMatching.Add("000.00.21.H05", "1");
            DictMatching.Add("000.00.31.H05", "37");
            DictMatching.Add("000.00.37.H05", "28");
            DictMatching.Add("000.00.38.H05", "32");
            DictMatching.Add("000.00.36.H05", "36");
            DictMatching.Add("000.00.35.H05", "40");
            DictMatching.Add("000.00.33.H05", "44");
            DictMatching.Add("000.00.32.H05", "29");
            DictMatching.Add("000.00.34.H05", "33");
            DictMatching.Add("998.99.99.H05", "23");
            DictMatching.Add("000.00.22.H05", "27");
            DictMatching.Add("999.99.99.H05", "11");
            DictMatching.Add("997.99.99.H05", "45");
            DictMatching.Add("996.99.99.H05", "30");
            DictMatching.Add("994.99.99.H05", "34");
            DictMatching.Add("995.99.99.H05", "46");
            DictMatching.Add("992.99.99.H05", "42");
            DictMatching.Add("993.99.99.H05", "38");
            DictMatching.Add("991.99.99.H05", "31");
            DictMatching.Add("000.01.31.H05", "37");
            DictMatching.Add("000.02.31.H05", "37");
            DictMatching.Add("000.03.31.H05", "37");
            DictMatching.Add("000.04.31.H05", "37");
            DictMatching.Add("000.05.31.H05", "37");
            DictMatching.Add("000.06.31.H05", "37");
            DictMatching.Add("000.07.31.H05", "37");
            DictMatching.Add("000.08.31.H05", "37");
            DictMatching.Add("000.09.31.H05", "37");
            DictMatching.Add("000.10.31.H05", "37");
            DictMatching.Add("000.11.31.H05", "37");
            DictMatching.Add("000.12.31.H05", "37");
            DictMatching.Add("000.13.31.H05", "37");
            DictMatching.Add("000.14.31.H05", "37");
            DictMatching.Add("000.15.31.H05", "37");
            DictMatching.Add("000.16.31.H05", "37");
            DictMatching.Add("000.17.31.H05", "37");
            DictMatching.Add("000.18.31.H05", "37");
            DictMatching.Add("000.19.31.H05", "37");
            DictMatching.Add("000.01.32.H05", "29");
            DictMatching.Add("000.02.32.H05", "29");
            DictMatching.Add("000.03.32.H05", "29");
            DictMatching.Add("000.04.32.H05", "29");
            DictMatching.Add("000.05.32.H05", "29");
            DictMatching.Add("000.06.32.H05", "29");
            DictMatching.Add("000.07.32.H05", "29");
            DictMatching.Add("000.08.32.H05", "29");
            DictMatching.Add("000.09.32.H05", "29");
            DictMatching.Add("000.10.32.H05", "29");
            DictMatching.Add("000.11.32.H05", "29");
            DictMatching.Add("000.12.32.H05", "29");
            DictMatching.Add("000.01.33.H05", "44");
            DictMatching.Add("000.02.33.H05", "44");
            DictMatching.Add("000.03.33.H05", "44");
            DictMatching.Add("000.04.33.H05", "44");
            DictMatching.Add("000.05.33.H05", "44");
            DictMatching.Add("000.06.33.H05", "44");
            DictMatching.Add("000.07.33.H05", "44");
            DictMatching.Add("000.08.33.H05", "44");
            DictMatching.Add("000.09.33.H05", "44");
            DictMatching.Add("000.10.33.H05", "44");
            DictMatching.Add("000.11.33.H05", "44");
            DictMatching.Add("000.12.33.H05", "44");
            DictMatching.Add("000.13.33.H05", "44");
            DictMatching.Add("000.14.33.H05", "44");
            DictMatching.Add("000.01.34.H05", "33");
            DictMatching.Add("000.02.34.H05", "33");
            DictMatching.Add("000.03.34.H05", "33");
            DictMatching.Add("000.04.34.H05", "33");
            DictMatching.Add("000.05.34.H05", "33");
            DictMatching.Add("000.06.34.H05", "33");
            DictMatching.Add("000.07.34.H05", "33");
            DictMatching.Add("000.08.34.H05", "33");
            DictMatching.Add("000.09.34.H05", "33");
            DictMatching.Add("000.10.34.H05", "33");
            DictMatching.Add("000.11.34.H05", "33");
            DictMatching.Add("000.12.34.H05", "33");
            DictMatching.Add("000.13.34.H05", "33");
            DictMatching.Add("000.14.34.H05", "33");
            DictMatching.Add("000.01.35.H05", "40");
            DictMatching.Add("000.02.35.H05", "40");
            DictMatching.Add("000.03.35.H05", "40");
            DictMatching.Add("000.04.35.H05", "40");
            DictMatching.Add("000.05.35.H05", "40");
            DictMatching.Add("000.06.35.H05", "40");
            DictMatching.Add("000.07.35.H05", "40");
            DictMatching.Add("000.08.35.H05", "40");
            DictMatching.Add("000.09.35.H05", "40");
            DictMatching.Add("000.10.35.H05", "40");
            DictMatching.Add("000.11.35.H05", "40");
            DictMatching.Add("000.12.35.H05", "40");
            DictMatching.Add("000.13.35.H05", "40");
            DictMatching.Add("000.14.35.H05", "40");
            DictMatching.Add("000.15.35.H05", "40");
            DictMatching.Add("000.16.35.H05", "40");
            DictMatching.Add("000.17.35.H05", "40");
            DictMatching.Add("000.18.35.H05", "40");
            DictMatching.Add("000.30.36.H05", "36");
            DictMatching.Add("000.31.36.H05", "36");
            DictMatching.Add("000.32.36.H05", "36");
            DictMatching.Add("000.33.36.H05", "36");
            DictMatching.Add("000.34.36.H05", "36");
            DictMatching.Add("000.35.36.H05", "36");
            DictMatching.Add("000.36.36.H05", "36");
            DictMatching.Add("000.37.36.H05", "36");
            DictMatching.Add("000.38.36.H05", "36");
            DictMatching.Add("000.39.36.H05", "36");
            DictMatching.Add("000.40.36.H05", "36");
            DictMatching.Add("000.41.36.H05", "36");
            DictMatching.Add("000.42.36.H05", "36");
            DictMatching.Add("000.43.36.H05", "36");
            DictMatching.Add("000.44.36.H05", "36");
            DictMatching.Add("000.45.36.H05", "36");
            DictMatching.Add("000.46.36.H05", "36");
            DictMatching.Add("000.47.36.H05", "36");
            DictMatching.Add("000.48.36.H05", "36");
            DictMatching.Add("000.49.36.H05", "36");
            DictMatching.Add("000.50.36.H05", "36");
            DictMatching.Add("000.01.37.H05", "28");
            DictMatching.Add("000.02.37.H05", "28");
            DictMatching.Add("000.03.37.H05", "28");
            DictMatching.Add("000.04.37.H05", "28");
            DictMatching.Add("000.05.37.H05", "28");
            DictMatching.Add("000.06.37.H05", "28");
            DictMatching.Add("000.07.37.H05", "28");
            DictMatching.Add("000.08.37.H05", "28");
            DictMatching.Add("000.09.37.H05", "28");
            DictMatching.Add("000.10.37.H05", "28");
            DictMatching.Add("000.11.37.H05", "28");
            DictMatching.Add("000.12.37.H05", "28");
            DictMatching.Add("000.13.37.H05", "28");
            DictMatching.Add("000.14.37.H05", "28");
            DictMatching.Add("000.01.38.H05", "32");
            DictMatching.Add("000.02.38.H05", "32");
            DictMatching.Add("000.03.38.H05", "32");
            DictMatching.Add("000.04.38.H05", "32");
            DictMatching.Add("000.05.38.H05", "32");
            DictMatching.Add("000.06.38.H05", "32");
            DictMatching.Add("000.07.38.H05", "32");
            DictMatching.Add("000.08.38.H05", "32");
            DictMatching.Add("000.10.38.H05", "32");
            DictMatching.Add("000.11.38.H05", "32");
            DictMatching.Add("000.12.38.H05", "32");
            DictMatching.Add("000.13.38.H05", "32");
            DictMatching.Add("000.14.38.H05", "32");
            DictMatching.Add("000.09.38.H05", "32");
            DictMatching.Add("990.99.99.H05", "41");
            DictMatching.Add("000.00.23.H05", "20");
            DictMatching.Add("000.00.24.H05", "8");
            DictMatching.Add("000.01.16.H05", "19");
            DictMatching.Add("000.02.16.H05", "19");
            DictMatching.Add("000.00.39.H05", "12");
            DictMatching.Add("899.99.99.H05", "35");
            DictMatching.Add("899.99.98.H05", "39");
            DictMatching.Add("000.00.101.H05", "16");
            DictMatching.Add("989.99.99.H05", "24");
            DictMatching.Add("979.99.99.H05", "43");
            DictMatching.Add("969.99.99.H05", "19");
            DictMatching.Add("000.02.01.H05", "19");
            DictMatching.Add("000.01.05.H05", "19");
            DictMatching.Add("000.16.37.H05", "28");
            DictMatching.Add("000.17.37.H05", "28");
            DictMatching.Add("000.18.37.H05", "28");
            DictMatching.Add("000.19.37.H05", "28");
            DictMatching.Add("000.20.37.H05", "28");
            DictMatching.Add("000.21.37.H05", "28");
            DictMatching.Add("000.22.37.H05", "28");
            DictMatching.Add("000.23.37.H05", "28");
            DictMatching.Add("000.24.37.H05", "28");
            DictMatching.Add("000.25.37.H05", "28");
            DictMatching.Add("000.26.37.H05", "28");
            DictMatching.Add("000.27.37.H05", "28");
            DictMatching.Add("000.28.37.H05", "28");
            DictMatching.Add("000.29.37.H05", "28");
            DictMatching.Add("000.30.37.H05", "28");
            DictMatching.Add("000.31.37.H05", "28");
            DictMatching.Add("000.32.37.H05", "28");
            DictMatching.Add("000.33.37.H05", "28");
            DictMatching.Add("000.34.37.H05", "28");
            DictMatching.Add("000.35.37.H05", "28");
            DictMatching.Add("000.36.37.H05", "28");
            DictMatching.Add("000.37.37.H05", "28");
            DictMatching.Add("000.38.37.H05", "28");
            DictMatching.Add("000.39.37.H05", "28");
            DictMatching.Add("000.15.38.H05", "32");
            DictMatching.Add("000.16.38.H05", "32");
            DictMatching.Add("000.17.38.H05", "32");
            DictMatching.Add("000.18.38.H05", "32");
            DictMatching.Add("000.19.38.H05", "32");
            DictMatching.Add("000.20.38.H05", "32");
            DictMatching.Add("000.21.38.H05", "32");
            DictMatching.Add("000.22.38.H05", "32");
            DictMatching.Add("000.23.38.H05", "32");
            DictMatching.Add("000.24.38.H05", "32");
            DictMatching.Add("000.25.38.H05", "32");
            DictMatching.Add("000.27.38.H05", "32");
            DictMatching.Add("000.29.38.H05", "32");
            DictMatching.Add("000.31.38.H05", "32");
            DictMatching.Add("000.33.38.H05", "32");
            DictMatching.Add("000.34.38.H05", "32");
            DictMatching.Add("000.35.38.H05", "32");
            DictMatching.Add("000.02.36.H05", "36");
            DictMatching.Add("000.09.36.H05", "36");
            DictMatching.Add("000.11.36.H05", "36");
            DictMatching.Add("000.14.36.H05", "36");
            DictMatching.Add("000.10.36.H05", "36");
            DictMatching.Add("000.07.36.H05", "36");
            DictMatching.Add("000.13.36.H05", "36");
            DictMatching.Add("000.15.36.H05", "36");
            DictMatching.Add("000.08.36.H05", "36");
            DictMatching.Add("000.12.36.H05", "36");
            DictMatching.Add("000.16.36.H05", "36");
            DictMatching.Add("000.04.36.H05", "36");
            DictMatching.Add("000.05.36.H05", "36");
            DictMatching.Add("000.03.36.H05", "36");
            DictMatching.Add("000.06.36.H05", "36");
            DictMatching.Add("000.23.36.H05", "36");
            DictMatching.Add("000.24.36.H05", "36");
            DictMatching.Add("000.25.36.H05", "36");
            DictMatching.Add("000.26.36.H05", "36");
            DictMatching.Add("000.27.36.H05", "36");
            DictMatching.Add("000.28.36.H05", "36");
            DictMatching.Add("000.29.36.H05", "36");
            DictMatching.Add("000.18.36.H05", "36");
            DictMatching.Add("000.19.36.H05", "36");
            DictMatching.Add("000.20.36.H05", "36");
            DictMatching.Add("000.21.36.H05", "36");
            DictMatching.Add("000.22.36.H05", "36");
            DictMatching.Add("000.99.36.H05", "36");
            DictMatching.Add("000.97.36.H05", "36");
            DictMatching.Add("000.99.36.H058", "36");
            DictMatching.Add("000.302.36.H05", "36");
            DictMatching.Add("000.318.36.H05", "36");
            DictMatching.Add("000.306.36.H05", "36");
            DictMatching.Add("000.327.36.H05", "36");
            DictMatching.Add("000.328.36.H06", "36");
            DictMatching.Add("000.15.33.H05", "44");
            DictMatching.Add("000.16.33.H05", "44");
            DictMatching.Add("000.17.33.H05", "44");
            DictMatching.Add("000.18.33.H05", "44");
            DictMatching.Add("000.19.33.H05", "44");
            DictMatching.Add("000.20.33.H05", "44");
            DictMatching.Add("000.21.33.H05", "44");
            DictMatching.Add("000.24.33.H05", "44");
            DictMatching.Add("000.25.33.H05", "44");
            DictMatching.Add("000.26.33.H05", "44");
            DictMatching.Add("000.27.33.H05", "44");
            DictMatching.Add("000.28.33.H05", "44");
            DictMatching.Add("000.29.33.H05", "44");
            DictMatching.Add("000.30.33.H05", "44");
            DictMatching.Add("000.31.33.H05", "44");
            DictMatching.Add("000.32.33.H05", "44");
            DictMatching.Add("000.33.33.H05", "44");
            DictMatching.Add("000.34.33.H05", "44");
            DictMatching.Add("000.22.33.H05", "44");
            DictMatching.Add("000.19.35.H05", "40");
            DictMatching.Add("000.20.35.H05", "40");
            DictMatching.Add("000.21.35.H05", "40");
            DictMatching.Add("000.22.35.H05", "40");
            DictMatching.Add("000.23.35.H05", "40");
            DictMatching.Add("000.24.35.H05", "40");
            DictMatching.Add("000.25.35.H05", "40");
            DictMatching.Add("000.26.35.H05", "40");
            DictMatching.Add("000.27.35.H05", "40");
            DictMatching.Add("000.28.35.H05", "40");
            DictMatching.Add("000.29.35.H05", "40");
            DictMatching.Add("000.30.35.H05", "40");
            DictMatching.Add("000.31.35.H05", "40");
            DictMatching.Add("000.32.35.H05", "40");
            DictMatching.Add("000.33.35.H05", "40");
            DictMatching.Add("000.34.35.H05", "40");
            DictMatching.Add("000.35.35.H05", "40");
            DictMatching.Add("000.38.35.H05", "40");
            DictMatching.Add("000.20.31.H05", "37");
            DictMatching.Add("000.21.31.H05", "37");
            DictMatching.Add("000.22.31.H05", "37");
            DictMatching.Add("000.23.31.H05", "37");
            DictMatching.Add("000.24.31.H05", "37");
            DictMatching.Add("000.25.31.H05", "37");
            DictMatching.Add("000.26.31.H05", "37");
            DictMatching.Add("000.27.31.H05", "37");
            DictMatching.Add("000.28.31.H05", "37");
            DictMatching.Add("000.30.31.H05", "37");
            DictMatching.Add("000.31.31.H05", "37");
            DictMatching.Add("000.32.31.H05", "37");
            DictMatching.Add("000.33.31.H05", "37");
            DictMatching.Add("000.34.31.H05", "37");
            DictMatching.Add("000.39.31.H05", "37");
            DictMatching.Add("000.41.31.H05", "37");
            DictMatching.Add("000.46.31.H05", "37");
            DictMatching.Add("000.48.31.H05", "37");
            DictMatching.Add("000.38.31.H05", "37");
            DictMatching.Add("000.29.31.H05", "37");
            DictMatching.Add("000.13.32.H05", "29");
            DictMatching.Add("000.14.32.H05", "29");
            DictMatching.Add("000.15.32.H05", "29");
            DictMatching.Add("000.16.32.H05", "29");
            DictMatching.Add("000.17.32.H05", "29");
            DictMatching.Add("000.18.32.H05", "29");
            DictMatching.Add("000.19.32.H05", "29");
            DictMatching.Add("000.20.32.H05", "29");
            DictMatching.Add("000.21.32.H05", "29");
            DictMatching.Add("000.22.32.H05", "29");
            DictMatching.Add("000.23.32.H05", "29");
            DictMatching.Add("000.25.32.H05", "29");
            DictMatching.Add("000.26.32.H05", "29");
            DictMatching.Add("000.27.32.H05", "29");
            DictMatching.Add("000.28.32.H05", "29");
            DictMatching.Add("000.29.32.H05", "29");
            DictMatching.Add("000.30.32.H05", "29");
            DictMatching.Add("000.31.32.H05", "29");
            DictMatching.Add("000.32.32.H05", "29");
            DictMatching.Add("000.33.32.H05", "29");
            DictMatching.Add("000.34.32.H05", "29");
            DictMatching.Add("000.35.32.H05", "29");
            DictMatching.Add("000.36.32.H05", "29");
            DictMatching.Add("000.37.32.H05", "29");
            DictMatching.Add("000.38.32.H05", "29");
            DictMatching.Add("000.39.32.H05", "29");
            DictMatching.Add("000.40.32.H05", "29");
            DictMatching.Add("000.41.32.H05", "29");
            DictMatching.Add("000.43.32.H05", "29");
            DictMatching.Add("000.44.32.H05", "29");
            DictMatching.Add("000.45.32.H05", "29");
            DictMatching.Add("000.15.34.H05", "33");
            DictMatching.Add("000.16.34.H05", "33");
            DictMatching.Add("000.17.34.H05", "33");
            DictMatching.Add("000.18.34.H05", "33");
            DictMatching.Add("000.19.34.H05", "33");
            DictMatching.Add("000.20.34.H05", "33");
            DictMatching.Add("000.21.34.H05", "33");
            DictMatching.Add("000.22.34.H05", "33");
            DictMatching.Add("000.23.34.H05", "33");
            DictMatching.Add("000.24.34.H05", "33");
            DictMatching.Add("000.25.34.H05", "33");
            DictMatching.Add("000.26.34.H05", "33");
            DictMatching.Add("000.28.34.H05", "33");
            DictMatching.Add("000.29.34.H05", "33");
            DictMatching.Add("000.30.34.H05", "33");
            DictMatching.Add("000.31.34.H05", "33");
            DictMatching.Add("000.32.34.H05", "33");
            DictMatching.Add("000.33.34.H05", "33");
            DictMatching.Add("000.34.34.H05", "33");
            DictMatching.Add("000.36.34.H05", "33");
            DictMatching.Add("000.37.34.H05", "33");
            DictMatching.Add("000.02.39.H05", "19");
            DictMatching.Add("000.01.39.H05", "19");
            DictMatching.Add("000.03.39.H05", "19");
            DictMatching.Add("000.04.39.H05", "19");
            DictMatching.Add("000.05.39.H05", "19");
        }

        #region Data Class
        public class APIResponse
        {
            public int code { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public Total[] total { get; set; }
        }

        public class Total
        {
            public string id { get; set; }
            public string username { get; set; }
            public string fullname { get; set; }
            public string title_code { get; set; }

            [AlterHeaderTextAttribute("TongVanBanDenChuaMo")]
            public int InNotOpen { get; set; }

            [AlterHeaderTextAttribute("TongVanBanDenChuaXuLy")]
            public int InNotProcess { get; set; }

            [AlterHeaderTextAttribute("TongVanBanDenDaXuLy")]
            public int InProcessed { get; set; }

            [AlterHeaderTextAttribute("TongVanBanDenDaCat")]
            public int InSaved { get; set; }

            [AlterHeaderTextAttribute("TongVanBanDenDeBietChuaMo")]
            public int InToKnownNotOpen { get; set; }

            [AlterHeaderTextAttribute("TongVanBanDenDeBiet")]
            public int InToKnown { get; set; }

            [AlterHeaderTextAttribute("TongVanBanDen")]
            public int InTotal { get; set; }

            [AlterHeaderTextAttribute("VBNoiBoChuaMo")]
            public int InternalNotOpen { get; set; }

            [AlterHeaderTextAttribute("TongVanBanNoiBo")]
            public int InternalTotal { get; set; }

            // Cac header rieng cho DS
            public string unit_code { get; set; }
            public string unit_name { get; set; }
            public int datekey { get; set; }
            public string SynDate { get; set; }
            public string IDDB { get; set; }
        }
        //

        public class APIUnitsResponse
        {
            public int code { get; set; }
            public Data_Units data { get; set; }
        }

        public class Data_Units
        {
            public Unit[] units { get; set; }
        }

        public class Unit
        {
            public string id { get; set; }
            public string unit_code { get; set; }
            public string unit_name { get; set; }
            public string unit_ws { get; set; }
            public string db { get; set; }
            public string group_id { get; set; }
            public string parent_id { get; set; }
            public string path { get; set; }
            public string ws_code { get; set; }
            public string unit_link { get; set; }
            public string orders { get; set; }
            public string isdomain { get; set; }
            public object hidden { get; set; }
        }

        #endregion
    }
}