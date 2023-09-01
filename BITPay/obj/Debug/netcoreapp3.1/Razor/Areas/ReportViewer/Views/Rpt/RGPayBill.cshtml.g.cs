#pragma checksum "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9a1f39cd529889a4e49dbca26d1ebb97effdded5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_ReportViewer_Views_Rpt_RGPayBill), @"mvc.1.0.view", @"/Areas/ReportViewer/Views/Rpt/RGPayBill.cshtml")]
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
#line 1 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\_ViewImports.cshtml"
using BITPay;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\_ViewImports.cshtml"
using BITPay.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\_ViewImports.cshtml"
using BITPay.DBL.Entities;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\_ViewImports.cshtml"
using BITPay.DBL.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9a1f39cd529889a4e49dbca26d1ebb97effdded5", @"/Areas/ReportViewer/Views/Rpt/RGPayBill.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f14dfb8024fdca72d7a4b2bb1b8cf701ebdb915", @"/Areas/ReportViewer/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_ReportViewer_Views_Rpt_RGPayBill : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ReportDataModel>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
  
    ViewData["Title"] = "DomesticTaxRpt";
    Layout = "_LayoutReport";
    var reportData = Model.DataList as IEnumerable<GeneralReportData>
    ;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 9 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
  
    var totals = reportData.Sum(x => x.Amount);

    //---- Get groups
    var groups = (from data in reportData
                  group data by data.ModeCode into grp
                  select new ReportGroupData
                  {
                      paymentmode = grp.FirstOrDefault().Mode,
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
            WriteLiteral(@"
<div class=""table-responsive"">
    <table class=""table table-striped table-bordered table-responsive-sm nowrap"" id=""Data"" style=""width:100%"">
        <thead>
            <tr>
                <th class=""header"">Ref No</th>
                <th class=""header"">Payer Name</th>
                <th class=""header"">Payment Mode</th>
                <th class=""header"">Acc Credit</th>
                <th class=""header"">Amount</th>
                <th class=""header"">Date</th>
                <th class=""header"">CBS Ref No</th>
                <th class=""header"">Branch Name</th>
                <th class=""header"">Maker</th>
            </tr>
        </thead>
        <tbody>
");
#nullable restore
#line 55 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
             foreach (var g in groups)
            {
                

#line default
#line hidden
#nullable disable
#nullable restore
#line 61 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
                 foreach (var rec in reportData.Where(x => x.ModeCode == g.GroupCode))
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>");
#nullable restore
#line 64 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 65 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.CustomerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 66 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.ModeName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 67 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.AccountCredit);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 68 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.Amount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 69 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.PostDate.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 70 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.ReferenceNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 71 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.BranchName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                <td>");
#nullable restore
#line 72 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
               Write(rec.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n\r\n            </tr>\r\n");
#nullable restore
#line 75 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"

                }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            <tfoot>
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
#line 89 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
                                      Write(totals.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\r\n                </tr>\r\n            </tfoot>\r\n");
#nullable restore
#line 92 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"

             }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
            </tbody>
        </table>

    </div>

<script>
    $(document).ready(function () {
        var d = new Date();
        var n = d.toString();
        $('#Data').DataTable({
            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'excel',
                    messageTop: 'Approved Domestic Tax Report',
                    messageBottom: 'Print Date   :  ' + n,
                    title: '");
#nullable restore
#line 111 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
                       Write(title);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                    footer: true
                },
                {
                    extend: 'pdf',
                    messageTop: 'Approved Domestic Tax Report',
                    messageBottom: 'Print Date   :  ' + n ,
                    title: '");
#nullable restore
#line 118 "C:\Users\Amoh\Desktop\Projects\Bitpay-finbank\Bitpay\BITPay\Areas\ReportViewer\Views\Rpt\RGPayBill.cshtml"
                       Write(title);

#line default
#line hidden
#nullable disable
            WriteLiteral("\',\r\n                    orientation: \'landscape\',\r\n                    pageSize: \'A4\',\r\n                    footer: true\r\n                },\r\n            ]\r\n        });\r\n    });\r\n</script>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ReportDataModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591