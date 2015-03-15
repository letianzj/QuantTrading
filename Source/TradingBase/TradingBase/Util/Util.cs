/****************************** Project Header ******************************\
Project:	      QuantTrading
Author:			  Letian_zj @ Codeplex
URL:			  https://quanttrading.codeplex.com/
Copyright 2014 Letian_zj

This file is part of QuantTrading Project.

QuantTrading is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, 
either version 3 of the License, or (at your option) any later version.

QuantTrading is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with QuantTrading. 
If not, see http://www.gnu.org/licenses/.

\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.IO;
using System.Net.Mail;

namespace TradingBase
{
    public static class Util
    {
#region Time
        public static int ToIntDate(DateTime dt)
        {
            return (dt.Year * 10000) + (dt.Month * 100) + dt.Day;
        }

        public static int ToIntTime(DateTime dt)
        {
            return (dt.Hour * 10000) + (dt.Minute * 100) + (dt.Second);
        }
        public static int ToIntTime(int hour, int min, int sec)
        {
            return hour * 10000 + min * 100 + sec;
        }

        public static DateTime ToDateTime(int date, int time)
        {
            int sec = time % 100;
            int hm = time % 10000;
            int hour = (int)((time - hm) / 10000);
            int min = (time - (hour * 10000)) / 100;
            if (sec > 59) { sec -= 60; min++; }
            if (min > 59) { hour++; min -= 60; }
            int year = 1, day = 1, month = 1;
            if (date != 0)
            {
                int ym = (date % 10000);
                year = (int)((date - ym) / 10000);
                int mm = ym % 100;
                month = (int)((ym - mm) / 100);
                day = mm;
            }
            return new DateTime(year, month, day, hour, min, sec);
        }

        public static int IntTimeToIntTimeSpan(int time)
        {
            int s1 = time % 100;
            int m1 = ((time - s1) / 100) % 100;
            int h1 = (int)((time - (m1 * 100) - s1) / 10000);
            return h1 * 3600 + m1 * 60 + s1;
        }

        /// <summary>
        /// adds inttime and int timespan (in seconds).  does not rollover 24hr periods.
        /// </summary>
        public static int IntTimeAdd(int firsttime, int timespaninseconds)
        {
            int s1 = firsttime % 100;
            int m1 = ((firsttime - s1) / 100) % 100;
            int h1 = (int)((firsttime - m1 * 100 - s1) / 10000);
            s1 += timespaninseconds;
            if (s1 >= 60)
            {
                m1 += (int)(s1 / 60);
                s1 = s1 % 60;
            }
            if (m1 >= 60)
            {
                h1 += (int)(m1 / 60);
                m1 = m1 % 60;
            }
            int sum = h1 * 10000 + m1 * 100 + s1;
            return sum;
        }

        public static int IntTimeDiff(int firsttime, int latertime)
        {
            int span1 = IntTimeToIntTimeSpan(firsttime);
            int span2 = IntTimeToIntTimeSpan(latertime);
            return span2 - span1;
        }

        /// <summary>
        /// EST day time zone
        /// </summary>
        public static bool IsDaylightSavingTime(DateTime dt)
        {
            return TimeZoneInfo.Local.IsDaylightSavingTime(dt);
        }

        public static TimeSpan GetUtcOffset(DateTime dt)
        {
            return TimeZoneInfo.Local.GetUtcOffset(dt);
        }
#endregion

#region File IO
        public static bool IsFileWritetable(string path)
        {
            FileStream stream = null;

            try
            {
                if (!System.IO.File.Exists(path))
                    return true;
                System.IO.FileInfo file = new FileInfo(path);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return true;
        }

        /// <summary>
        /// Add date to the full symbol
        /// </summary>
        public static string SafeFilename(string fullsymbol, string path, int date)
        {
            return path + "\\" + SafeSymbol(fullsymbol) + " " + date.ToString() + ".TXT";
        }

        /// <summary>
        /// Remove invalid characters in security symbol
        /// </summary>
        public static string SafeSymbol(string fullsymbol)
        {
            char[] _invalid = Path.GetInvalidPathChars();
            char[] _more = "/\\*?:".ToCharArray();
            _more.CopyTo(_invalid, 0);
            //_more.CopyTo(0,_invalid,_invalid.Length,_more.Length);
            foreach (char c in _invalid)
            {
                int p = 0;
                while (p != -1)
                {
                    p = fullsymbol.IndexOf(c);
                    if (p != -1)
                        fullsymbol = fullsymbol.Remove(p, 1);
                }
            }
            return fullsymbol;
        }

        public static Security SecurityFromFileName(string filename)
        {
            try
            {
                // file name with extension from path string
                filename = Path.GetFileName(filename);
                string ds = System.Text.RegularExpressions.Regex.Match(filename, "([0-9]{8})[.]", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Result("$1");
                string sym = filename.Replace(ds, "").Replace(".TXT", "").Replace(".txt", "");
                Security s = Security.Deserialize(sym);
                //s.Date = Convert.ToInt32(ds);
                return s;
            }
            catch (Exception)
            {
                return new Security("", "");
            }
        }

        public static int DateFromFileName(string filename)
        {
            try
            {
                // file name with extension from path string
                filename = Path.GetFileName(filename);
                string ds = System.Text.RegularExpressions.Regex.Match(filename, "([0-9]{8})[.]", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Result("$1");
                int date = Int32.Parse(ds);
                return date;
            }
            catch (Exception)
            {
                throw new Exception("Tick file name doesn't contain valid date.");
            }
        }
#endregion

#region strategy loading
        public static bool isTickFile(string path)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(path, "TXT", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public static bool isStrategyFile(string path)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(path, "DLL", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public static List<string> GetStrategyListFromDLL(string dllfilepath) { return GetStrategyListFromDLL(dllfilepath, null); }
        public static List<string> GetStrategyListFromDLL(string dllfilepath, Action<string> debug)
        {
            List<string> slist = new List<string>();
            if (!File.Exists(dllfilepath)) return slist;
            Assembly a;
            try
            {
                a = Assembly.LoadFile(dllfilepath);
            }
            catch (Exception ex) { slist.Add(ex.Message); return slist; }
            return GetStrategyListFromAssembly(a, debug);
        }

        public static List<string> GetStrategyListFromAssembly(Assembly strategyassembly, Action<string> debug = null)
        {
            List<string> slist = new List<string>();
            Type[] t;
            try
            {
                t = strategyassembly.GetTypes();
                for (int i = 0; i < t.GetLength(0); i++)
                    if (IsStrategy(t[i])) slist.Add(t[i].FullName);
            }
            catch (Exception ex)
            {
                if (debug != null)
                {
                    debug(ex.Message + ex.StackTrace);
                }
            }

            return slist;
        }

        static bool IsStrategy(Type t)
        {
            return typeof(StrategyBase).IsAssignableFrom(t);
        }

        public static StrategyBase GetSingleStrategyFromDLL(string fullname, string dllname)
        {
            System.Reflection.Assembly a;

#if (DEBUG)
            a = System.Reflection.Assembly.LoadFrom(dllname);
#else
            byte[] raw = loadFile(dllname);
            a = System.Reflection.Assembly.Load(raw);
#endif
            return GetSingleStrategyFromAssembly(a, fullname);
        }

        public static byte[] loadFile(string filename)
        {
            // get file
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            // prepare buffer based on file size
            byte[] buffer = new byte[(int)fs.Length];
            // read file into buffer
            fs.Read(buffer, 0, buffer.Length);
            // close file
            fs.Close();
            // return buffer
            return buffer;
        }

        /// <summary>
        /// Create a single strategy from an Assembly containing many strategies. 
        /// </summary>
        /// <param name="a">the assembly object</param>
        /// <param name="boxname">The fully-qualified strategyName (as in strategy.FullName).</param>
        /// <returns></returns>
        public static StrategyBase GetSingleStrategyFromAssembly(System.Reflection.Assembly a, string fullname)
        {
            Type type;
            object[] args;
            StrategyBase b = null;
            // get class from assembly
            type = a.GetType(fullname, true, true);
            args = new object[] { };
            // create an instance of type and cast to strategy
            b = (StrategyBase)Activator.CreateInstance(type, args);
            // if it doesn't have a name, add one
            if (b.Name == string.Empty)
            {
                b.Name = type.Name;
            }
            if (b.FullName == string.Empty)
            {
                b.FullName = type.FullName;
            }
            return b;
        }

        public static bool IsStrategyDLL(string path)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(path, "DLL", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
#endregion

        #region Email
        public static string EmailFrom { get; set; }
        public static string EmailPass { get; set; }
        public static string EmailTo { get; set; }
        public static void Sendemail(string subject, string body, bool useHTML = false)
        {
            Sendemail(subject, body, null, useHTML);
        }

        public static void Sendemail(string subject, string body, string attachfilepathname, bool useHTML=false)
        {
            try
            {
                // Send mail
                // gmail = ConfigurationManager.AppSettings["gmailacct"];
                // pwd = ConfigurationManager.AppSettings["gmailpwd"];
                string emailfrom, pwd, emailto;
                emailfrom = EmailFrom;
                emailto = EmailTo;
                pwd = EmailPass;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailfrom);
                string[] toaddr = emailto.Split(';');
                foreach(string s in toaddr)
                    mail.To.Add(s);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = useHTML;

                if (!string.IsNullOrEmpty(attachfilepathname))
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(attachfilepathname);
                    mail.Attachments.Add(attachment);
                }

                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(emailfrom, pwd);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string[] GmailAccount(string configfile)
        {
            string[] ret = new string[2];
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(configfile))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "item")
                        {
                            if (reader["name"] == "Email")
                            {
                                reader.Read();
                                ret[0] = reader.Value.Trim();
                            }
                            else if (reader["name"] == "EmailPass")
                            {
                                reader.Read();
                                ret[1] = reader.Value.Trim();
                            }
                        }
                    }
                }
            }
            return ret;
        }
        #endregion

        #region Other
        /// <summary>
        /// Get Root Path
        /// </summary>
        /// <returns>C:\QuantTrading\</returns>
        public static string GetRootPath()
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, @"..\..");
            return Path.GetFullPath(path);
        }

        public static void SaveConfig(ConfigManager config)
        {
            ConfigManager.Serialize(config, @"c:\quanttrading\config\mainsettings.xml");
        }

        public static ConfigManager LoadConfig(string settingpath)
        {
            if (File.Exists(settingpath))
                return ConfigManager.Deserialize(settingpath);
            else
                return new ConfigManager();
        }

        static int GetProcessCount(string program)
        {
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();
            int count = 0;
            foreach (System.Diagnostics.Process p in ps)
            {
                string cps = p.ProcessName.ToLower();
                if ((cps == (program.ToLower())) || (cps == (program.ToLower() + ".vshost")))
                    count++;
            }
            return count;
        }
        #endregion
    }
}
