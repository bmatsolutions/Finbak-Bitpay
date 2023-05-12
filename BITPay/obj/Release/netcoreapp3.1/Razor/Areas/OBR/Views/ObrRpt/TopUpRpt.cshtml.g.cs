#pragma checksum "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "51e355e6b823473b5c1585f2e438f3235fba5901"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_OBR_Views_ObrRpt_TopUpRpt), @"mvc.1.0.view", @"/Areas/OBR/Views/ObrRpt/TopUpRpt.cshtml")]
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
#line 1 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\_ViewImports.cshtml"
using BITPay;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\_ViewImports.cshtml"
using BITPay.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\_ViewImports.cshtml"
using BITPay.DBL.Entities;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\_ViewImports.cshtml"
using BITPay.DBL.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"51e355e6b823473b5c1585f2e438f3235fba5901", @"/Areas/OBR/Views/ObrRpt/TopUpRpt.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e6e5e55c114ed01159ac00bf2e925b97b6d586bc", @"/Areas/OBR/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_OBR_Views_ObrRpt_TopUpRpt : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ReportDataModel>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
  
    Layout = "_LayoutReport";
    var reportData = Model.DataList as IEnumerable<GeneralReportData>;
    var totals = reportData.Sum(x => x.Amount);

    //---- Get groups
    var groups = (from data in reportData group data by data.ModeCode into grp select new ReportGroupData
    {
        paymentmode = grp.FirstOrDefault().PaymentMode,
        GroupCode = grp.FirstOrDefault().ModeCode,
        GroupAmount = grp.Sum(x => x.Amount)

    });

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    <div class=\"table-responsive\">\n        <div class=\"text-center\">");
#nullable restore
#line 18 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                            Write(Model.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\n        <div style=\"font-size:12px;\">Dates: ");
#nullable restore
#line 19 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
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
                    <th class=""header"">Payway Account</th>
                    <th class=""header"">Payway Contact</th>
                    <th class=""header"">Cr Acc</th>
                    <th class=""header"">Dr Acc</th>
                    <th class=""header"">Amount</th>
                    <th class=""header"">Date</th>
                    <th class=""header"">Maker Id</th>
                    <th class=""header"">Checker Id</th>
                    <th class=""header"">Receipt Number</th>
                    <th class=""header"">Payway ref</th>
                    <th class=""header"">Depositor Name</th>
                    <th class=""header"">Remarks</th>
                </tr>
            </thead>
            <tbody>
");
#nullable restore
#line 41 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                 foreach (var g in groups)
                {
                    

#line default
#line hidden
#nullable disable
#nullable restore
#line 47 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                     foreach (var rec in reportData.Where(x => x.ModeCode == g.GroupCode))
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <tr>\n                            <td>");
#nullable restore
#line 50 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.CbsResp);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 51 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.PaywayAccount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 52 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.PaywayContact);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 53 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.CR_Account);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 54 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.DR_Account);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 55 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.Amount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 56 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.PaymentDate.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 57 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.MakerID);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 58 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.Checker_Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 59 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 60 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.RefNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 61 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.DepositorName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td>");
#nullable restore
#line 62 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                           Write(rec.Remarks);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                        </tr>\n");
#nullable restore
#line 64 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                    }

#line default
#line hidden
#nullable disable
#nullable restore
#line 70 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                       

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
                    <th></th>
                    <th></th>
                    <th>General Totals:</th>
                    <th class=""text-right"">");
#nullable restore
#line 87 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TopUpRpt.cshtml"
                                      Write(totals.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\n                </tr>\n            </tfoot>\n            </tbody>\n        </table>\n");
            WriteLiteral(@"
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
                    messageTop: 'E-Value Top Up Report',
                    messageBottom: 'Print Date   :  ' + n,
                    title: 'Finbank SA SP.',
                    footer: true
                },
                {
                    extend: 'pdf',
                    messageTop: 'E-Value Top Up Report',
                    messageBottom: 'Print Date   :  ' + n ,
                    title: 'Finbank SA SP.',
                    orientation: 'landscape',
                    pageSize: 'A4',
                    footer: true
                },
            ]
        });
    });
    </script>");
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
