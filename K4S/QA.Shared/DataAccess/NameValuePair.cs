namespace DataAccess
{
    public class NameValuePair
    {
        public bool IsNull { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }


        public NameValuePair()
        {
            this.IsNull = true;
            this.Name = "";
            this.Value = "";
        }


        public NameValuePair(string name, string value)
        {
            this.Name = name;

            if (value == null)
            {
                this.IsNull = true;
                this.Value = "";
            }
            else
            {
                this.IsNull = false;
                this.Value = value;
            }
        }


        public override string ToString()
        {
            if (IsNull)
            {
                return Name + " = null";
            }
            else
            {
                return Name + " = '" + Value + "'";
            }
        }
    }
}
