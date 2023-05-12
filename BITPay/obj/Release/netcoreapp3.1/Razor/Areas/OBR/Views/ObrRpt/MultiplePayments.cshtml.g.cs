#pragma checksum "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5ff03593f6d080d5bd170f7b1b7a11ee52ab06f7"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_OBR_Views_ObrRpt_MultiplePayments), @"mvc.1.0.view", @"/Areas/OBR/Views/ObrRpt/MultiplePayments.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"5ff03593f6d080d5bd170f7b1b7a11ee52ab06f7", @"/Areas/OBR/Views/ObrRpt/MultiplePayments.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e6e5e55c114ed01159ac00bf2e925b97b6d586bc", @"/Areas/OBR/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_OBR_Views_ObrRpt_MultiplePayments : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ReportDataModel>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
  
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

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    <div class=\"table-responsive\">\n\n        <div class=\"text-center\">");
#nullable restore
#line 21 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                            Write(Model.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\n        <div style=\"font-size:12px;\">Dates: ");
#nullable restore
#line 22 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                                       Write(Model.DateRange);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>
        <hr />

        <table class=""table table-striped table-bordered table-responsive-sm nowrap"" id=""Data"" style=""width:100%"">
            <thead>
                <tr>
                    <th class=""header"">DECLARANT NAME</th>
                    <th class=""header"">DECLARANT CODE</th>
                    <th class=""header"">PAY DATE</th>
                    <th class=""header"">TXN REF</th>
                    <th class=""header"">DR ACCOUNT</th>
                    <th class=""header"">MODE</th>
                    <th class=""header text-right"">AMOUNT</th>
                    <th class=""header text-right"">CheckerId</th>
                    <th class=""header text-right"">Status</th>

                </tr>
            </thead>
            <tbody>
");
#nullable restore
#line 41 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                 foreach (var g in groups)
        {
                

#line default
#line hidden
#nullable disable
#nullable restore
#line 46 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                 foreach (var rec in reportData.Where(x => x.ModeCode == g.GroupCode))
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                    <td>");
#nullable restore
#line 49 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.DeclarantName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 50 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.DeclarantNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 51 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.PaymentDate.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 52 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.RefNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 53 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.DR_Account);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 54 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.PaymentMode);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 55 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.Amount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 56 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.Checker_Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 57 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   Write(rec.Status);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                </tr>\n");
#nullable restore
#line 59 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 63 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                   
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
                    <th>General Totals:</th>
                    <th class=""text-right"">");
#nullable restore
#line 75 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\MultiplePayments.cshtml"
                                      Write(totals.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</th>\n                </tr>\n            </tfoot>\n            </tbody>\n        </table>\n");
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
                    messageTop: 'Multiple Custom Tax Report',
                    messageBottom: 'Print Date   :  ' + n,
                    title: 'Finbank SA SP.',
                    footer: true
                },
                {
                    extend: 'pdf',
                    messageTop: 'Multiple Custom Tax Report',
                    messageBottom: 'Print Date   :  ' + n ,
                    title: 'Finbank SA SP.',
                    orientation: 'landscape',
                    pageSize: 'A4',
                    footer: true
                },
            ]
        });
    });
    </script>
");
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
