﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace DataAccess
{
	public interface IDataProvider
	{
		DataSet Excute(RequestCollection requests);
        DataSet ExcuteAsync(RequestCollection requests);
    }
}
