using BITPay.DBL;
using BITPay.DBL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BITPay
{
    public class Audit
    {
        public static void AuditAction(AppConfig appConfig, string browser, string actionDescr, int moduleid, string MdlFunction, int UserCode, string Ip)
        {
            if (UserCode > 0)
            {
                Task.Run(async () =>
                {
                    SqlConnection sqlconn = new SqlConnection(appConfig.ConnectionString);
                    sqlconn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_AuditAdd", sqlconn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@UsrCode", UserCode));
                        cmd.Parameters.Add(new SqlParameter("@ActDescr", actionDescr));
                        cmd.Parameters.Add(new SqlParameter("@Modid", moduleid));
                        cmd.Parameters.Add(new SqlParameter("@ModFunc", MdlFunction));
                        cmd.Parameters.Add(new SqlParameter("@Browser", browser));
                        cmd.Parameters.Add(new SqlParameter("@ClntIP", Ip));
                        try
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                        catch (Exception ex)
                        {
                            LogUtil.Error(appConfig.LogFile, "Audit.AuditAction", ex);
                        }
                        finally
                        {
                            if (sqlconn != null)
                            {
                                sqlconn.Close();
                            }
                        }
                    }
                });
            }
        }

        private static string GetIPAddress()
        {
            string IP4Address = String.Empty;
            try
            {
                IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    if (curAdd.AddressFamily.ToString() == "InterNetwork")
                    {
                        IP4Address = curAdd.ToString();
                        break;
                    }
                }

                if (IP4Address != String.Empty)
                {
                    return IP4Address;
                }
               

                foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }
            }
            catch (Exception)
            {
                IP4Address = "172.168.0.0";
            }
            return IP4Address;
        }


    }
}
