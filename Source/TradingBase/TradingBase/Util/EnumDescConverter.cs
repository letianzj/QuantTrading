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
using System.ComponentModel;
using System.Reflection;


namespace TradingBase
{
    /// <summary>
    /// EnumConverter supporting System.ComponentModel.DescriptionAttribute
    /// Usage:  get string description:     EnumDescConverter.GetEnumDescription(sectype)
    ///         get enum type from string:  contract.SecurityType = (string.IsNullOrEmpty(sectypestr) || sectypestr.Equals("?")
    ///                                     ? SecurityType.Undefined
    ///                                     : (SecurityType) EnumDescConverter.GetEnumValue(typeof (SecurityType), sectypestr));
    /// </summary>
    public static class EnumDescConverter
    {
        #region Private Caches

        /// <summary>
        /// Cache of enumerables
        /// </summary>
        private static Dictionary<Type, EnumTuple> enumLookup = new Dictionary<Type, EnumTuple>();

        /// <summary>
        /// Enum Tuple
        /// </summary>
        private class EnumTuple
        {
            /// <summary>
            /// Enum Type
            /// </summary>
            public Type EnumType;

            /// <summary>
            /// Enumeration to String
            /// </summary>
            public Dictionary<Enum, string> EnumToString = new Dictionary<Enum, string>();

            /// <summary>
            /// String to Enumeration
            /// </summary>
            public Dictionary<string, Enum> DescriptionToEnum = new Dictionary<string, Enum>();

            /// <summary>
            /// Name to enum
            /// </summary>
            public Dictionary<string, Enum> NameToEnum = new Dictionary<string, Enum>();
        }

        #endregion

        /// <summary>
        /// Constructor to create caches
        /// </summary>
        static EnumDescConverter()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            List<Type> enums = new List<Type>();

            foreach (var type in types)
            {
                if (type.IsEnum)
                {
                    //Add to cache
                    enums.Add(type);
                }
            }

            foreach (var e in enums)
            {
                EnumTuple et = new EnumTuple();
                et.EnumType = e;
                FieldInfo[] fields = e.GetFields();

                foreach (var fi in fields)
                {
                    if (fi.FieldType == e)
                    {
                        DescriptionAttribute[] attributes =
                            (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        Enum val = (Enum)fi.GetValue(fi.Name);
                        string strVal = attributes.Length > 0 ? attributes[0].Description : val.ToString();
                        et.EnumToString.Add(val, strVal);
                        if (attributes.Length > 0)
                            et.DescriptionToEnum.Add(attributes[0].Description, val);
                        et.NameToEnum.Add(val.ToString(), val);
                    }
                }
                enumLookup.Add(e, et);
            }
        }

        /// <summary>
        /// Gets Enum Value's Description Attribute
        /// </summary>
        /// <param name="value">The value you want the description attribute for</param>
        /// <returns>The description, if any, else it's .ToString()</returns>
        public static string GetEnumDescription(Enum value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Type t = value.GetType();

            if (enumLookup.ContainsKey(t))
            {
                EnumTuple et = enumLookup[t];
                if (et.EnumToString.ContainsKey(value))
                    return et.EnumToString[value];
            }

            FieldInfo fi = t.GetField(value.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Gets the value of an Enum, based on it's Description Attribute or named value
        /// </summary>
        /// <param name="value">The Enum type</param>
        /// <param name="description">The description or name of the element</param>
        /// <returns>The value, or the passed in description, if it was not found</returns>
        public static object GetEnumValue(Type value, string description)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (description == null)
                description = string.Empty;

            if (enumLookup.ContainsKey(value))
            {
                EnumTuple et = enumLookup[value];
                if (et.DescriptionToEnum.ContainsKey(description))
                    return (object)et.DescriptionToEnum[description];
                if (et.NameToEnum.ContainsKey(description))
                    return (object)et.NameToEnum[description];
            }

            FieldInfo[] fis = value.GetFields();
            foreach (FieldInfo fi in fis)
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                        typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    if (attributes[0].Description == description)
                    {
                        return fi.GetValue(fi.Name);
                    }
                }
                if (fi.Name == description)
                {
                    return fi.GetValue(fi.Name);
                }
            }

            throw new InvalidCastException(string.Concat("The received value ", description, " was unrecognized as an ", value.Name, " enum value."));
        }
    }
}
