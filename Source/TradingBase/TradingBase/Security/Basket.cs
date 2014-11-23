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

using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;


namespace TradingBase
{
    /// <summary>
    /// Holds collections of securities fullnames
    /// </summary>
    [Serializable]
    public class Basket
    {
        public Basket() { }
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="onesymbol">first symbol</param>
        public Basket(string onesymbol) : this(new string[] { onesymbol }) { }
        /// <summary>
        /// Create a basket of securities
        /// </summary>
        /// <param name="symbolist">symbols</param>
        public Basket(string[] symbolist)
        {
            _securities.AddRange(symbolist);
        }

        public Basket(List<string> symbollist)
        {
            _securities.AddRange(symbollist);
        }

        /// <summary>
        /// clone a basket
        /// </summary>
        /// <param name="copy"></param>
        public Basket(Basket copy)
        {
            foreach (string s in copy)
                Add(s);
            Name = copy.Name;
        }

        /// <summary>
        /// name of basket
        /// </summary>
        string _name = "MySecurityList";
        public string Name { get { return _name; } set { _name = value; } }
        public int Count { get { return _securities.Count; } }
        public string this [int index] { get { return _securities[index]; } set { _securities[index] = value; } }
        List<string> _securities = new List<string>();
        public List<string> Securities { get { return _securities; } set { _securities = value; } }

        public IEnumerator GetEnumerator() { foreach (string s in _securities) yield return s; }

        /// <summary>
        /// adds a security if not already present
        /// </summary>
        /// <param name="s"></param>
        public void Add(string s) 
        {
            if (!_securities.Contains(s))
            {
                _securities.Add(s);
                SendDebug(s + "added.");
            }
        }
        /// <summary>
        /// adds contents of another basket to this one.
        /// will not result in duplicate symbols
        /// </summary>
        public void Add(string[] syms)
        {
            for (int i = 0; i < syms.Length; i++)
                this.Add(syms[i]);
        }

        public void Clear()
        {
            _securities.Clear();
        }

        /// <summary>
        /// whether security is present
        /// </summary>
        /// <param name="sec">FullName</param>
        /// <returns></returns>
        public bool IsSecurityPresent(string sec)
        {
            return _securities.Contains(sec);
        }

        public int IndexOf(string obj)
        {
            return _securities.IndexOf(obj);
        }

        public event Action<string> SendDebugEvent;

        public override string ToString() { return SerializeToString(this); }

        protected virtual void SendDebug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        public static string SerializeToString(Basket b)
        {
            List<string> s = new List<string>();
            for (int i = 0; i < b.Count; i++) s.Add(b[i]);
            return string.Join(",", s.ToArray());
        }

        public static Basket DeserializeFromString(string serialBasket)
        {
            Basket mb = new Basket();
            if ((serialBasket == null) || (serialBasket == "")) return mb;
            string[] r = serialBasket.Split(',');
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] == "") continue;
                mb.Add(r[i]);
            }
            return mb;
        }

        public static void SerializeToXML(Basket b, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Basket));
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, b);
            writer.Close();
        }

        public static Basket DeserializeFromXML(string filename)
        {
            Basket b = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Basket));
            StreamReader reader = new StreamReader(filename);

            b = (Basket)serializer.Deserialize(reader);
            reader.Close();
            return b;
        }
    }
}
