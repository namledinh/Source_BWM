using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BWM
{
    public partial class FormGenCode : Form
    {
        public FormGenCode()
        {
            InitializeComponent();
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            string msg = DoWork(out string table, out string query, out string store);
            if (msg.Length > 0) tbResultClass.Text = msg;
            else tbResultClass.Text = table;

            if (msg.Length > 0) tbResultSQL.Text = msg;
            else tbResultSQL.Text = query;

            if (msg.Length > 0) tbResultStore.Text = msg;
            else tbResultStore.Text = store;
        }

        string sTable = "table ";
        string DoWork(out string table, out string query, out string store)
        {
            table = query = store = null;

            int currIndex = 0;

            StringBuilder sbClass = new StringBuilder();
            StringBuilder sbSQLTable = new StringBuilder();
            StringBuilder sbSQLStore = new StringBuilder();
            while (currIndex < tbSource.Lines.Length)
            {
                var msg = GetABlock(ref currIndex, out string table_temp, out string query_temp, out string store_temp);
                if (msg.Length > 0) return msg;

                sbClass.Append(table_temp);
                sbSQLTable.Append(query_temp);
                sbSQLStore.Append(store_temp);
            }
            table = sbClass.ToString();
            query = sbSQLTable.ToString();
            store = sbSQLStore.ToString();

            return "";
        }

        string GetABlock(ref int currIndex, out string table, out string query, out string store)
        {
            table = query = store = null;

            int iTableName = GetIndex(tbSource.Lines, sTable, currIndex);
            if (iTableName == -1) return $"Cannot GetIndex of {sTable}";

            StringBuilder sbClass = new StringBuilder();
            sbClass.Append($"public class {GetTableName(tbSource.Lines[iTableName])}\r\n").Append("{\r\n");

            StringBuilder sbSQLTable = new StringBuilder();
            sbSQLTable.Append($"DROP TABLE IF EXISTS {GetTableName(tbSource.Lines[iTableName])};\r\n");
            sbSQLTable.Append($"CREATE TABLE `{GetTableName(tbSource.Lines[iTableName])}` (\r\n");

            StringBuilder sbSQLStore = new StringBuilder();
            sbSQLStore.Append($"DROP PROCEDURE IF EXISTS usp_{GetTableName(tbSource.Lines[iTableName])}_insert;\r\n");
            sbSQLStore.Append($"CREATE PROCEDURE `usp_{GetTableName(tbSource.Lines[iTableName])}_insert` ()\r\n");
            sbSQLStore.Append($"BEGIN\r\n");
            sbSQLStore.Append($"TRUNCATE {GetTableName(tbSource.Lines[iTableName])};\r\n\r\n");
            sbSQLStore.Append($"INSERT INTO {GetTableName(tbSource.Lines[iTableName])}\r\n");
            sbSQLStore.Append($"SELECT ");

            int i = iTableName + 1;
            while (i < tbSource.Lines.Length)
            {
                if (tbSource.Lines[i].Trim() == "") { i++; continue; }
                if (tbSource.Lines[i].ToString().ToLower().StartsWith(sTable)) break;

                var items = tbSource.Lines[i].Split(' ', '\t');
                if (items.Length != 2) return $"{tbSource.Lines[i]} has not exactly 2 items";

                string type = GetType(items[1]);
                sbClass.Append($"public {type} {items[0]} " + "{ get; set; }\r\n");

                type = GetTypeSQL(items[1]);
                sbSQLTable.Append($"`{items[0]}` {type} COLLATE utf8_unicode_ci DEFAULT NULL,\r\n");

                sbSQLStore.Append($"{items[0]}, ");

                i++;
            }
            currIndex = i;

            sbClass.Append("}\r\n\r\n");
            table = sbClass.ToString();

            sbSQLTable.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;\r\n\r\n");
            query = sbSQLTable.ToString();

            sbSQLStore.Length--; // delete last " "
            sbSQLStore.Length--; // delete last ","
            sbSQLStore.Append($" FROM xxxxxx;" + "\r\n");
            sbSQLStore.Append($"END;" + "\r\n\r\n");
            store = sbSQLStore.ToString();

            return "";
        }

        int GetIndex(string[] lines, string s, int currIndex)
        {
            if (currIndex > lines.Length) return -1;

            while (currIndex < lines.Length)
            {
                if (lines[currIndex].ToString().ToLower().StartsWith(s)) return currIndex;
                currIndex++;
            }

            return -1;
        }
        string GetTableName(string line)
        {
            return line.Substring(sTable.Length).Trim();
        }
        string GetType(string s)
        {
            s = s.ToLower();
            if (s.StartsWith("varchar")) return "string";
            if (s == "int") return "int";
            if (s == "datetime") return "DateTime";

            return s;
        }



        string GetTypeSQL(string s)
        {
            s = s.ToLower();
            if (s.StartsWith("varchar"))
            {
                var len = s.Substring("varchar".Length);
                if (len.Trim() == "") len = "255";
                return $"varchar({len})";
            }
            if (s == "int") return "int";
            if (s == "datetime") return "timestamp";

            return s;
        }

        private void FormGenCode_Load(object sender, EventArgs e)
        {
            this.ActiveControl = tbSource;
        }
    }
    //public class DomainCCTV
    //{
    //    public string MaDuAn { get; set; }
    //    public string TenDuAn { get; set; }
    //    public string Domain { get; set; }
    //    public string BI { get; set; }
    //}

    //    DROP TABLE IF EXISTS qtmt_stations;
    //CREATE TABLE `qtmt_stations` (
    //  `ma_tram` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `station_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `station_name` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `station_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `last_time` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `qi_adjust` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `qi_adjust_time` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `latitude` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `longitude` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `address` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `qi` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `is_public_data_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `order_no` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
    //  `datekey` int DEFAULT NULL,
    //  `syncdate` timestamp NULL DEFAULT NULL
    //) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE = utf8_unicode_ci;

    //    DROP PROCEDURE IF EXISTS usp_noitru_pttt_ioc_insert;
    //CREATE PROCEDURE `usp_noitru_pttt_ioc_insert`()
    //BEGIN
    //    TRUNCATE noitru_pttt_ioc;

    //    INSERT INTO noitru_pttt_ioc
    //            SELECT    MaBA,
    //                            STT,
    //                            STTPTTT,
    //                            MaPTTT,
    //                            TenPTTT,
    //                            LoaiPTTT,
    //                            DonGiaBH,
    //                            DonGiaDV,
    //                            GhiChu,
    //                            NgayThucHIen,
    //                            TongTienDV,
    //                            TongTienBH,
    //                            ThanhTien,
    //                            TrangThai,
    //                            NgayHenTH,
    //                            DATE_FORMAT(NgayThucHIen, '%Y%m%d')
    //                FROM noitru_phauthuatthuthuat;
    //    END;
}
