using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMStaticModels.UserModels;

namespace PlanMateWebApp.Controllers
{
    public class PublicController : Controller
    {
        public void DeleteUserInfo()
        {
            PMPublicFunctions.PMPublicFunc.PMPublicFuncs.DeleteUser();
        }

    }
}