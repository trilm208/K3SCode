using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data;

namespace K3S.Droid.Extensions
{
    public static class DataTableExtensions
    {
        public static IEnumerable<T> Convert<T>(this DataTable table)
        {
            List<T> data = new List<T>();

            var type = typeof(T);
            var fields = type.GetFields();

            foreach (DataRow row in table.Rows)
            {
                var item = (T)type.GetConstructor(Type.EmptyTypes).Invoke(null);

                foreach (var field in fields)
                {
                    var value = row[field.Name];
                    if (value == DBNull.Value)
                        value = null;

                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(item, System.Convert.ToInt32(value));
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(item, System.Convert.ToBoolean(value));
                    }
                    else
                    {
                        field.SetValue(item, value);
                    }
                }

                data.Add(item);
            }

            return data;
        }


        public static List<T> ToModel<T>(this DataTable dt)
        {
            List<string> columns = (from DataColumn dc in dt.Columns select dc.ColumnName).ToList();

            var fields = typeof(T).GetFields();
            var properties = typeof(T).GetProperties();

            List<T> lst = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                var ob = Activator.CreateInstance<T>();

                foreach (var fieldInfo in fields.Where(fieldInfo => columns.Contains(fieldInfo.Name)))
                {
                    fieldInfo.SetValue(ob, !dr.IsNull(fieldInfo.Name) ? dr[fieldInfo.Name] : fieldInfo.FieldType.IsValueType ? Activator.CreateInstance(fieldInfo.FieldType) : null);
                }

                foreach (var propertyInfo in properties.Where(propertyInfo => columns.Contains(propertyInfo.Name)))
                {
                    propertyInfo.SetValue(ob, !dr.IsNull(propertyInfo.Name) ? dr[propertyInfo.Name] : propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null);
                }

                lst.Add(ob);
            }

            return lst;
        }

        public static bool IsContainValue(this DataTable dt, string value, string columnname)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row.Item(columnname) == value)
                {
                    return true;
                }
            }
            return false;
        }



        //To strip control characters:

        //A character that does not represent a printable character but //serves to initiate a particular action.





        //public static int _FindIndex(this DataTable _table, string columnName, string value)
        //{
        //    if (_table == null || _table.Rows.Count == 0)
        //        return -2;
        //    for (int i = 0; i < _table.Rows.Count; i++)
        //    {
        //        if (_table.Rows[i].Item(columnName).Trim() == value.Trim())
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}
        public static string _FindValue(this DataRow[] _rows, string OutputColumnName, string InputcolumnName, string InputValue)
        {
            if (_rows == null)
                return string.Empty;
            foreach (DataRow row in _rows)
            {
                if (row.Item(InputcolumnName) == InputValue)
                {
                    return row.Item(OutputColumnName);
                }
            }
            return String.Empty;
        }
        public static int _FindIndex(this DataRow[] _rows, string columnName, string value)
        {
            if (_rows == null || _rows.Count() == 0)
                return -2;
            for (int i = 0; i < _rows.Count(); i++)
            {
                if (_rows[i].Item(columnName).Trim() == value.Trim())
                {
                    return i;
                }
            }
            return -1;
        }

        public static int CountValue(this DataRow[] _rows)
        {
            return _rows.Count();
        }
        //public static string _FindValue(this DataRow[] _rows, string OutputColumnName, string InputcolumnName, string InputValue)
        //{
        //    if (_rows == null)
        //        return string.Empty;
        //    foreach (DataRow row in _rows)
        //    {
        //        if (row.Item(InputcolumnName) == InputValue)
        //        {
        //            return row.Item(OutputColumnName);
        //        }
        //    }
        //    return String.Empty;
        //}
        public static DataTable ReverseRowsInDataTable(this DataTable inputTable)
        {
            DataTable outputTable = inputTable.Clone();

            for (int i = inputTable.Rows.Count - 1; i >= 0; i--)
            {
                outputTable.ImportRow(inputTable.Rows[i]);
            }

            return outputTable;
        }

        public static string _FindValue(this DataTable _table, string OutputColumnName, string InputcolumnName, string InputValue)
        {
            if (_table == null)
                return string.Empty;

            foreach (DataRow row in _table.Rows)
            {
                if (row.Item(InputcolumnName) == InputValue)
                {
                    return row.Item(OutputColumnName);
                }
            }
            return String.Empty;
        }
    }
}