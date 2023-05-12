using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Consts
{
    public class ConstData
    {
        public static List<ListModel> GetUserRoles()
        {
            return new List<ListModel>
            {
                new ListModel("MAKER","0"),
                new ListModel("CHECKER","1"),
                new ListModel("ADMIN","2"),
                new ListModel("ReportViewer","3"),
                new ListModel("GUEST","4")
            };
        }

        public static List<ListModel> GetVehicleTypes()
        {
            return new List<ListModel>
            {
                new ListModel("Vehicule","1"),
                new ListModel("Moto","2"),
                new ListModel("Touk Touk","3")
            };
        }

        public static List<ListModel> GetAnnualFees()
        {
            return new List<ListModel>
            {
                new ListModel("Permis Industrielle","1"),
                new ListModel("Permis Artisanal","2")
            };
        }

        public static List<ListModel> GetTaxeValorem()
        {
            return new List<ListModel>
            {
                new ListModel("Exploitation Industrielle","1"),
                new ListModel("Exploitation Artisanal","2")
            };
        }

        public static List<ListModel> GetCivilStat()
        {
            return new List<ListModel>
            {
                new ListModel("Marie","1"),
                new ListModel("Celibataire","2"),
                new ListModel("Veuf (veuve)","3"),
                new ListModel("Divorsé(é)","4")
            };
        }
    }
}
