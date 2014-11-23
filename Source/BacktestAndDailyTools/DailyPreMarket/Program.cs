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

using Excel = Microsoft.Office.Interop.Excel;
using RDotNet;
using TradingBase;

namespace DailyPreMarket
{
    class Program
    {
        static void Main(string[] args)
        {
            bool matchpairs = false;
            if (args.Length > 0)
            {
                if (args[0] == "1")
                    matchpairs = true;
            }

            string _rootpath = Util.GetRootPath();
            ConfigManager _configmanager = Util.LoadConfig(_rootpath + @"\Config\mainsettings.xml");
            List<List<string>> results_match = new List<List<string>>();
            List<List<string>> results_sn = new List<List<string>>();
            List<List<string>> results_pair = new List<List<string>>();

            // ******************** Read Tickers ****************************** //
            int counter = 0;
            string line;
            System.IO.StreamReader tickerfile = new System.IO.StreamReader(_rootpath + _configmanager.DailyBucket);
            while ((line = tickerfile.ReadLine()) != null)
            {
                List<string> sublist = new List<string>();
                string[] entry = line.Split(',');
                for (int j = 0; j < entry.Length; j++)
                    sublist.Add(entry[j]);

                results_sn.Add(sublist);
                counter++;
            }
            tickerfile.Close();

            // ******************** Read pairs ****************************** //
            counter = 0;
            System.IO.StreamReader pairsfile = new System.IO.StreamReader(_rootpath + _configmanager.DailyPairs);
            while ((line = pairsfile.ReadLine()) != null)
            {
                List<string> sublist = new List<string>();
                string[] entry = line.Split(',');
                for (int j = 0; j < entry.Length; j++)
                    sublist.Add(entry[j]);

                results_pair.Add(sublist);
                counter++;
            }
            pairsfile.Close();


            // ******************** REngine Calculation ****************************** //
            REngine _rengine;
            REngine.SetEnvironmentVariables();
            CharacterVector _rresponse;
            // There are several options to initialize the engine, but by default the following suffice:
            _rengine = REngine.GetInstance();

            try
            {
                _rresponse = _rengine.Evaluate("rm(list=ls())").AsCharacter();
                _rresponse = _rengine.Evaluate("library('quantmod')").AsCharacter();

                _rresponse = _rengine.CreateCharacter(_rootpath + _configmanager.RWorkspacePath);
                _rengine.SetSymbol("workpath", _rresponse);
                _rresponse = _rengine.Evaluate("setwd(workpath)").AsCharacter();
                _rresponse = _rengine.Evaluate("getwd()").AsCharacter();
                _rresponse = _rengine.CreateCharacter(_rootpath + _configmanager.RWorkspacePath + "DailyScan.R");
                _rengine.SetSymbol("workpath", _rresponse);
                _rresponse = _rengine.Evaluate("source(workpath)").AsCharacter();
                _rresponse = _rengine.CreateCharacter(_rootpath + _configmanager.RWorkspacePath + "DailyPairs.R");
                _rengine.SetSymbol("workpath", _rresponse);
                _rresponse = _rengine.Evaluate("source(workpath)").AsCharacter();


                //************************* 0. Match Pairs  *********************************
                if (matchpairs)
                {
                    List<string> sublist0 = new List<string>();
                    sublist0.AddRange(results_sn[0]);
                    sublist0.AddRange(results_sn[0]);
                    bool gettitle = true;

                    System.IO.StreamWriter error = new System.IO.StreamWriter(_rootpath + _configmanager.DailyResultPath + "error.txt");

                    for (int i = 1; i < results_sn.Count; i++)
                    {
                        for (int j = i + 1; j < results_sn.Count; j++)
                        {
                            List<string> sublist = new List<string>();

                            CharacterVector symbolA = _rengine.CreateCharacter(results_sn[i][0]);          // create symbol string
                            _rengine.SetSymbol("A", symbolA);                                                // assign a name
                            CharacterVector symbolB = _rengine.CreateCharacter(results_sn[j][0]);                          // create symbol string
                            _rengine.SetSymbol("B", symbolB);                                                // assign a name
                            NumericVector hratio = _rengine.CreateNumeric(0.0);
                            _rengine.SetSymbol("hratio", hratio);                                                // assign a name

                            try
                            {
                                _rresponse = _rengine.Evaluate("results <- DailyPairs(A,B,hratio)").AsCharacter();       // call DailyScan
                                
                                sublist.AddRange(results_sn[i]);
                                sublist.AddRange(results_sn[j]);
                                sublist.AddRange(_rresponse);

                                if (gettitle)
                                {
                                    _rresponse = _rengine.Evaluate("names(results)").AsCharacter();       // get results names
                                    sublist0.AddRange(_rresponse);
                                    results_match.Add(sublist0);
                                    gettitle = false;
                                }

                                results_match.Add(sublist);
                            }
                            catch 
                            {
                                error.WriteLine("Pairs(" + i + "," + j + ") (" + results_sn[i][0] + "," + results_sn[j][0] + ") has error.");
                            }

                            Console.WriteLine("Pairs(" + i + "," + j + ") (" + results_sn[i][0] + "," + results_sn[j][0] + ") is done.");
                        }
                    }

                    error.Close();
                }


                //************************* 1. process single names *********************************
                for (int j = 1; j < results_sn.Count; j++)
                {
                    CharacterVector symbol = _rengine.CreateCharacter(results_sn[j][0]);       // create symbol string
                    _rengine.SetSymbol("sym", symbol);                                      // assign a name
                    _rresponse = _rengine.Evaluate("results <- DailyScan(sym)").AsCharacter();       // call DailyScan
                    results_sn[j].AddRange(_rresponse);

                    if (j == 1)
                    {
                        _rresponse = _rengine.Evaluate("names(results)").AsCharacter();       // get results names
                        results_sn[0].AddRange(_rresponse);
                    }

                    Console.WriteLine("symbol " + j + " is done.");
                }

                //************************* 2. process pairs *********************************
                for (int j = 1; j < results_pair.Count; j++)
                {
                    CharacterVector symbolA = _rengine.CreateCharacter(results_pair[j][0]);          // create symbol string
                    _rengine.SetSymbol("A", symbolA);                                                // assign a name
                    CharacterVector symbolB = _rengine.CreateCharacter(results_pair[j][2]);                          // create symbol string
                    _rengine.SetSymbol("B", symbolB);                                                // assign a name
                    NumericVector hratio  =  _rengine.CreateNumeric(Convert.ToDouble(results_pair[j][5]));
                    _rengine.SetSymbol("hratio", hratio);                                                // assign a name

                    _rresponse = _rengine.Evaluate("results <- DailyPairs(A,B,hratio)").AsCharacter();       // call DailyScan
                    results_pair[j].AddRange(_rresponse);

                    if (j == 1)
                    {
                        _rresponse = _rengine.Evaluate("names(results)").AsCharacter();       // get results names
                        results_pair[0].AddRange(_rresponse);
                    }

                    Console.WriteLine("Pairs " + j + " is done.");
                }
            }
            catch { }
            finally
            {
                if (_rengine != null)
                {
                    _rengine.Close();
                    _rengine.Dispose();
                }
            }

            // ******************** Write to Excel ****************************** //
            Excel.Application excel = new Excel.Application();
            excel.Visible = false;
            excel.DisplayAlerts = false;

            Excel.Workbook wb = excel.Workbooks.Add();
            Excel.Worksheet sh3 = wb.Sheets.Add();
            sh3.Name = "MatchPairs";
            Excel.Worksheet sh2 = wb.Sheets.Add();
            sh2.Name = "Pairs";
            Excel.Worksheet sh = wb.Sheets.Add();
            sh.Name = "SingleName";

            // sh.Cells[2, "B"].Value2 = "A";
            // sh.Cells[2, "C"].Value2 = "1122";
            object[,] arr = new object[results_sn.Count, results_sn[0].Count];
            for (int i = 0; i < results_sn.Count; i++)
            {
                for (int j = 0; j < results_sn[0].Count; j++)
                {
                    arr[i, j] = (object)results_sn[i][j];
                }
            }

            object[,] arr2 = new object[results_pair.Count, results_pair[0].Count];
            for (int i = 0; i < results_pair.Count; i++)
            {
                for (int j = 0; j < results_pair[0].Count; j++)
                {
                    arr2[i, j] = (object)results_pair[i][j];
                }
            }

            object[,] arr3 = null;
            if (matchpairs)
            {
                arr3 = new object[results_match.Count, results_match[0].Count];
                for (int i = 0; i < results_match.Count; i++)
                {
                    for (int j = 0; j < results_match[0].Count; j++)
                    {
                        arr3[i, j] = (object)results_match[i][j];
                    }
                }
            }
            
            try
            {
                Excel.Range c1 = (Excel.Range)sh.Cells[1, 1];
                Excel.Range c2 = (Excel.Range)sh.Cells[results_sn.Count, results_sn[0].Count];
                Excel.Range range = sh.get_Range(c1, c2);
                range.Value = arr;

                c1 = (Excel.Range)sh2.Cells[1, 1];
                c2 = (Excel.Range)sh2.Cells[results_pair.Count, results_pair[0].Count];
                range = sh2.get_Range(c1, c2);
                range.Value = arr2;

                if (matchpairs)
                {
                    c1 = (Excel.Range)sh3.Cells[1, 1];
                    c2 = (Excel.Range)sh3.Cells[results_match.Count, results_match[0].Count];
                    range = sh3.get_Range(c1, c2);
                    range.Value = arr3;
                }
                
                wb.Worksheets["Sheet1"].Delete();
                wb.Worksheets["Sheet2"].Delete();
                wb.Worksheets["Sheet3"].Delete();
            }
            catch { }
            finally
            {
                wb.SaveAs(_rootpath + _configmanager.DailyResultPath + "DailyResults" + Util.ToIntDate(DateTime.Today) + ".xlsx");
                wb.Close();
                excel.Quit();
            }        
    
            // send email
            Util.EmailFrom = _configmanager.GmailFrom;
            Util.EmailPass = _configmanager.GmailPass;
            Util.EmailTo = _configmanager.EmailTo;
            Util.Sendemail("DailyPreMarket", "", _rootpath + _configmanager.DailyResultPath + "DailyResults" + Util.ToIntDate(DateTime.Today) + ".xlsx");
        }
    }
}
