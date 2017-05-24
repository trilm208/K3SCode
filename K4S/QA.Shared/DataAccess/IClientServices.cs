using DataAccess;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IClientServices
    {
        string LastError { get; }

        Task<DataSet> ExecuteAsync(RequestCollection requests);
        DataSet Execute(RequestCollection requests);

        object this[string key] { get; set; }

        void SetInformation(string key, object value);

        object GetInformation(string key);

        string Localize(string name);

        string Localize(string category, string name);

        string GetSetting(string name);

        string GetSetting(string category, string name);

        void LoadSettings(string FacID);

        bool HasPermission(string type);
        
    }
}