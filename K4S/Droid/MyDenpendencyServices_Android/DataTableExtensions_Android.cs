using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MyDenpendencyServices_Android;
using MyDependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(DataTableExtensions_Android))]
namespace MyDenpendencyServices_Android
{
    public class DataTableExtensions_Android : IDataTableExtensions
    {

        public bool IsNull (DataTable dt)
        {
            if (dt == null)
                return true;
            else
                return false;
        }
        public List<T> ToModel<T>(DataTable dt)
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
        public T RowToModel<T>(DataRow row)
        {
            List<string> columns = (from DataColumn dc in row.Table.Columns select dc.ColumnName).ToList();

            var fields = typeof(T).GetFields();
            var properties = typeof(T).GetProperties();

            var lst = new List<T>();

            var ob = Activator.CreateInstance<T>();

            foreach (var fieldInfo in fields.Where(fieldInfo => columns.Contains(fieldInfo.Name)))
            {
                fieldInfo.SetValue(ob, !row.IsNull(fieldInfo.Name) ? row[fieldInfo.Name] : fieldInfo.FieldType.IsValueType ? Activator.CreateInstance(fieldInfo.FieldType) : null);
            }

            foreach (var propertyInfo in properties.Where(propertyInfo => columns.Contains(propertyInfo.Name)))
            {
                propertyInfo.SetValue(ob, !row.IsNull(propertyInfo.Name) ? row[propertyInfo.Name] : propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null);
            }

            lst.Add(ob);

            return lst[0];
        }

        public  int _FindIndex( DataRow[] _rows, string columnName, string value)
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
        public  string _FindValue( DataRow[] _rows, string OutputColumnName, string InputcolumnName, string InputValue)
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

    }
}
