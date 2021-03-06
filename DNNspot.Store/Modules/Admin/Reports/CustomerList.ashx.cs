/*
* This software is licensed under the GNU General Public License, version 2
* You may copy, distribute and modify the software as long as you track changes/dates of in source files and keep all modifications under GPL. You can distribute your application using a GPL library commercially, but you must also provide the source code.

* DNNspot Software (http://www.dnnspot.com)
* Copyright (C) 2013 Atriage Software LLC
* Authors: Kevin Southworth, Matthew Hall, Ryan Doom

* This program is free software; you can redistribute it and/or
* modify it under the terms of the GNU General Public License
* as published by the Free Software Foundation; either version 2
* of the License, or (at your option) any later version.

* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.

* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

* Full license viewable here: http://www.gnu.org/licenses/gpl-2.0.txt
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using WA.FileHelpers.Csv;

namespace DNNspot.Store.Modules.Admin.Reports
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class CustomerList : DnnIHttpHandler //IHttpHandler
    {
        public override void HandleRequest(HttpRequest request, HttpResponse response)
        {
            DataModel.DataModel.Initialize();

            const string nameForFile = "CustomerList";           

            response.Buffer = true; //make sure that the entire output is rendered simultaneously                        
            //response.ContentType = "application/vnd.ms-excel";
            response.ContentType = "text/csv";
            response.AddHeader("Content-Disposition", "inline;filename=" + nameForFile + ".csv");

            if (IsUserAuthenticated)
            {
                UserInfo userInfo = DnnUserInfo;
                if (userInfo != null && (userInfo.IsSuperUser || userInfo.IsInRole(DnnPortalSettings.AdministratorRoleName)))
                {
                    int? storeId = WA.Parser.ToInt(request.Params["storeId"]);
                    if (storeId.HasValue)
                    {
                        List<CustomerInfo> customerInfos = DataModel.Store.GetCustomers(storeId.Value);

                        using (var csvWriter = new CsvWriter(new StreamWriter(response.OutputStream), new CsvWriterOptions() { HasHeaderRecord = true }))
                        {                            
                            csvWriter.WriteRecords(customerInfos);
                        }
                    }
                }
            }
            

            response.Flush();
        }
    }
}
