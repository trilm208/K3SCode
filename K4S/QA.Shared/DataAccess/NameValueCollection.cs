using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace DataAccess
{
    public class NameValueCollection : IEnumerable<NameValuePair>
    {
        private List<NameValuePair> items = new List<NameValuePair>();


        public NameValuePair this[string name]
        {
            get
            {
                foreach (var item in items)
                {
                    if (item.Name == name)
                        return item;
                }

                return new NameValuePair();
            }
        }


        public void Add(NameValuePair item)
        {
            items.Add(item);
        }


        public void AddRange(IEnumerable<NameValuePair> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }


        public void Add(string name, string value)
        {
            this.Add(new NameValuePair(name, value));
        }


        public void Add(object obj)
        {
            if (obj is IDictionary<string, string>)
            {
                var items = obj as IDictionary<string, string>;
                foreach (var item in items)
                {
                    this.Add(item.Key, item.Value);
                }
            }
            else if (IsAnonymousType(obj.GetType()))
            {
                this.AddRange(GetObjectProperties(obj));
            }
        }


        private IEnumerable<NameValuePair> GetObjectProperties(object obj)
        {
            var type = obj.GetType();

            TypeInfo typeInfo = type.GetTypeInfo();

            foreach (PropertyInfo propInfo in typeInfo.DeclaredProperties)
            {
                var name = propInfo.Name;
                var value = propInfo.GetValue(obj, null);

                if (value == null)
                    value = "";
                yield return new NameValuePair(name, value.ToString());
            }

            //var properties = type.GetProperties();

            //foreach (var property in properties)
            //{
            //    var name = property.Name;
            //    var value = property.GetValue(obj, null);

            //    if (value == null)
            //        value = "";
            //    yield return new NameValuePair(name, value.ToString());
            //}
        }


        private bool IsAnonymousType(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            bool hasCompilerGeneratedAttribute = typeInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            bool nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            bool isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }


        public IEnumerator<NameValuePair> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
