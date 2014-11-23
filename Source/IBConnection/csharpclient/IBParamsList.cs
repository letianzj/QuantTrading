using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBApi
{
    public static class IBParamsList
    {
      public static void AddParameter(this List<byte> source, int value)
      {
         AddParameter(source, value.ToString());
      }

      public static void AddParameter(this List<byte> source, double value)
      {
          AddParameter(source, value.ToString());
      }

      public static void AddParameter(this List<byte> source, bool value)
      {
          if (value)
              AddParameter(source, "1");
          else
              AddParameter(source, "0");
         
      }

      public static void AddParameter(this List<byte> source, string value)
      {
          if(value != null)
            source.AddRange(UTF8Encoding.UTF8.GetBytes(value));
          source.Add(Constants.EOL);
      }

      public static void AddParameterMax(this List<byte> source, double value)
      {
          if (value == Double.MaxValue)
              source.Add(Constants.EOL);
          else
              source.AddParameter(value);
          
      }

      public static void AddParameterMax(this List<byte> source, int value)
      {
          if (value == Int32.MaxValue)
              source.Add(Constants.EOL);
          else
            source.AddParameter(value);
      }

    }
}
