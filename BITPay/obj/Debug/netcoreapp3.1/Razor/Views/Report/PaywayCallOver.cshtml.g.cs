#pragma checksum "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "8c3d36f365dda6c5490d22be4d84949219c8a601"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Report_PaywayCallOver), @"mvc.1.0.view", @"/Views/Report/PaywayCallOver.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\_ViewImports.cshtml"
using BITPay;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\_ViewImports.cshtml"
using BITPay.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\_ViewImports.cshtml"
using BITPay.DBL.Entities;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\_ViewImports.cshtml"
using BITPay.DBL.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8c3d36f365dda6c5490d22be4d84949219c8a601", @"/Views/Report/PaywayCallOver.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f14dfb8024fdca72d7a4b2bb1b8cf701ebdb915", @"/Views/_ViewImports.cshtml")]
    public class Views_Report_PaywayCallOver : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ReportDataModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
  
    Layout = "_LayoutReport";
    var reportData = Model.DataList as IEnumerable<GeneralReportData>;
    var totals = reportData.Sum(x => x.Amount);

    //---- Get groups
    var groups = (from data in reportData
                  group data by data.ModeCode into grp
                  select new ReportGroupData
                  {
                      paymentmode = grp.FirstOrDefault().PaymentMode,
                      GroupCode = grp.FirstOrDefault().ModeCode,
                      GroupAmount = grp.Sum(x => x.Amount)

                  });
    bool authenticated = false;
    string userId = "";
    UserDataModel currentUserData = null;
    int userRole = -1;
    string title = "";
    if (User.Identities.Any(u => u.IsAuthenticated))
    {
        authenticated = true;
        currentUserData = Util.GetCurrentUserData(User.Identities);
        if (currentUserData != null)
        {
            userRole = currentUserData.UserRole;
            title = currentUserData.Title;
        }
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n    <div class=\"table-responsive\">\r\n        <div class=\"text-center\">");
#nullable restore
#line 36 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                            Write(Model.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\r\n        <div style=\"font-size:12px;\">Dates: ");
#nullable restore
#line 37 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                                       Write(Model.DateRange);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>
        <hr />

        <table class=""table table-striped table-bordered table-responsive-sm nowrap"" id=""Data"" style=""width:100%"">
            <thead>
                <tr>
                    <th class=""header"">Txn Ref</th>
                    <th class=""header"">Cr Acc</th>
                    <th class=""header"">Dr Acc</th>
                    <th class=""header"">Amount</th>
                    <th class=""header"">Date</th>
                    <th class=""header"">Maker Id</th>
                   <th class=""header"">Checker Id</th>
                    <th class=""header"">Payway Account</th>
                    <th class=""header"">Payway Contact</th>
                    <th class=""header"">Receipt Number</th>
                    <th class=""header"">Payway ref</th>
                </tr>
            </thead>
            <tbody>
");
#nullable restore
#line 57 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                 foreach (var g in groups)
                {
                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 62 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                     foreach (var rec in reportData.Where(x => x.ModeCode == g.GroupCode))
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <tr>\r\n                            <td>");
#nullable restore
#line 65 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.CbsResp);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 66 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.CR_Account);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 67 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.DR_Account);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 68 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.Amount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 69 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.PaymentDate.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 70 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.MakerID);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                          <td>");
#nullable restore
#line 71 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                         Write(rec.Checker_Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 72 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.PaywayAccount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 73 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.PaywayContact);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 74 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 75 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                           Write(rec.RefNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        </tr>\r\n");
#nullable restore
#line 77 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <tfoot>
                    <tr>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th></th>
                       <th></th>
                        <th></th>
                        <th>General Totals:</th>
                        <th class=""text-right"">");
#nullable restore
#line 94 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                                          Write(totals.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n                    </tr>\r\n                </tfoot>\r\n");
#nullable restore
#line 97 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </tbody>\r\n            </table>\r\n");
            WriteLiteral(@"    </div>
        
    <script>
    $(document).ready(function () {
        var d = new Date();
        var n = d.toString();
        $('#Data').DataTable({
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'excel',
                    messageTop: 'Payway Call Over Report',
                    messageBottom: 'Print Date   :  ' + n,
                    title: '");
#nullable restore
#line 120 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                       Write(title);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                    footer: true
                },
                {
                    extend: 'pdf',
                    messageTop: 'Payway Call Over Report',
                    messageBottom: 'Print Date   :  ' + n ,
                    title: '");
#nullable restore
#line 127 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Report\PaywayCallOver.cshtml"
                       Write(title);

#line default
#line hidden
#nullable disable
            WriteLiteral("\',\r\n                    orientation: \'landscape\',\r\n                    pageSize: \'A4\',\r\n                    footer: true\r\n                },\r\n            ]\r\n        });\r\n    });\r\n    </script>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ReportDataModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
