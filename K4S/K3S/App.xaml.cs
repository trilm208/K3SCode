using Extensions;

using MyDependencyServices;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using K3S.Model;
using Realms;
using System.Linq;

namespace K3S
{
    public partial class App : Application
    {
        #region DB VARIABLES
     
        static Realms.Realm _realm;
        internal static List<RmGenericFormQuestionAndroidItem> _listOriginalQuestion
        {
            get;
            set;
        }
        #endregion

        public App()
        {
            Resources = new ResourceDictionary();
            InitializeComponent();

          

            var con = RealmConfiguration.DefaultConfiguration;
            con.SchemaVersion = 1;  
            _realm = Realms.Realm.GetInstance(con);
            var homepage = new HomePage();
            homepage.Process();
            MainPage = new NavigationPage(homepage);
        }

        #region DATABASE
        //public static ProfleImageDatabase dbProfileImage
        //{
        //    get
        //    {
        //        if (DBProfleImage == null)
        //        {
        //            DBProfleImage = new ProfleImageDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBProfleImage;
        //    }
        //}

        public static Realms.Realm dbRealm
        {
            get
            {

                return _realm;
            }
        }

        public static List<Model.RmWardDistrictCityItem> _listHanhChanh
        {
            get;
            set;         
        }
          

        //public static GPSItemDatabase dbGPS
        //{
        //    get
        //    {
        //        if (DBGPS == null)
        //        {
        //            DBGPS = new GPSItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBGPS;
        //    }
        //}

        //public static InterviewTimeDatabase dbInterviewTime
        //{
        //    get
        //    {
        //        if (DBInterviewTime == null)
        //        {
        //            DBInterviewTime = new InterviewTimeDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBInterviewTime;
        //    }
        //}

        //public static GenericFormValueDatabase dbGenericFormValue
        //{
        //    get
        //    {
        //        if (DBGenericFormValue == null)
        //        {
        //            DBGenericFormValue = new GenericFormValueDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBGenericFormValue;
        //    }
        //}

        //public static AnswerItemDatabase dbAnswer
        //{
        //    get
        //    {
        //        if (DBAnswer == null)
        //        {
        //            DBAnswer = new AnswerItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBAnswer;
        //    }
        //}

        //public static ProjectItemDatabase dbProject
        //{
        //    get
        //    {
        //        if (DBProject == null)
        //        {
        //            DBProject = new ProjectItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBProject;
        //    }
        //}

        //public static GenericFormQuestionAndroidItemDatabase dbGenericFormQuestionAndroid
        //{
        //    get
        //    {
        //        if (DBGenericFormQuestionAndroid == null)
        //        {
        //            DBGenericFormQuestionAndroid = new GenericFormQuestionAndroidItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBGenericFormQuestionAndroid;
        //    }
        //}

        //public static WardDistrictCityItemDatabase dbWardDistrictCity
        //{
        //	get
	       // {
        //        if (DBWardDistrictCity == null)
    		  //  {
        //            DBWardDistrictCity = new WardDistrictCityItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBWardDistrictCity;
        //    }
        //}

        //public static FilterConditionItemDatabase dbFilterCondition
        //{
        //    get
        //    {
        //        if (DBFilterCondition == null)
        //        {
        //            DBFilterCondition = new FilterConditionItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBFilterCondition;
        //    }
        //}

        //public static LogicCheckItemDatabase dbLogicCheck
        //{
        //    get
        //    {
        //        if (DBLogicCheck == null)
        //        {
        //            DBLogicCheck = new LogicCheckItemDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("ToDoSQLite.db3"));
        //        }
        //        return DBLogicCheck;
        //    }
        //}

        #endregion

        protected override void OnStart()
        {
            if (App.Current.MainPage.GetType() == typeof(InterviewPage))
            {
                Debug.WriteLine("OnStart on interview page");
            }
            else
            {
                Debug.WriteLine("OnStart");
            }
        }
        protected override void OnSleep()
        {
            if (App.Current.MainPage.GetType() == typeof(InterviewPage))
            {
                Debug.WriteLine("OnSleep on interview page");
            }
            else
            {
                Debug.WriteLine("OnSleep");
            }
           
        }
        protected override void OnResume()
        {
            if (App.Current.MainPage.GetType() == typeof(InterviewPage))
            {
                Debug.WriteLine("OnResume on interview page");
            }
            else
            {
                Debug.WriteLine("OnResume");
            }
         
        }
    }
}
