#pragma checksum "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5c632581cdb550b709b67987d78333230300cba9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_OBR_Views_ObrRpt_TokenReceipt), @"mvc.1.0.view", @"/Areas/OBR/Views/ObrRpt/TokenReceipt.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"5c632581cdb550b709b67987d78333230300cba9", @"/Areas/OBR/Views/ObrRpt/TokenReceipt.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e6e5e55c114ed01159ac00bf2e925b97b6d586bc", @"/Areas/OBR/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Areas_OBR_Views_ObrRpt_TokenReceipt : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<TokenReportModels>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("logo"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/images/payway.png"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("alt", new global::Microsoft.AspNetCore.Html.HtmlString(".."), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/dist/jquery.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
  
    Layout = "_LayoutPaywayCashPowerReceipt";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<br />\n<table style=\"width:80%\" align=\"center\">\n    <tr>\n        <td></td>\n        <td></td>\n        <td></td>\n        <td>\n            <div style=\"text-align:right;\">");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "5c632581cdb550b709b67987d78333230300cba95408", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@" </div>
        </td>
    </tr>
    <tr>
        <td></td>
        <td><div style=""text-align:center;""><h4 style=""font:bold;"">Cash Power Receipt</h4></div></td>
        <td></td>
    </tr>
</table>
<table style=""width:80%;font-size:12px"" align=""center"">
    <tr>
        <td style=""text-align:left;"">
            <table style=""width:100%;"">
                <tr>
                    <td><strong>Date:</strong> ");
#nullable restore
#line 27 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                          Write(Model.ReceiptDate.ToString("dd/MM/yyyy hh:mm tt"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td><strong>Receipt No:</strong> ");
#nullable restore
#line 28 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                Write(Model.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</td>
                </tr>
            </table>
        </td>
    </tr>


    <tr>
        <td>

            <table class=""table  table-striped"">
                <tr class=""text-nowrap"">
                    <td>
                        <strong> Customer Name: </strong> ");
#nullable restore
#line 41 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                     Write(Model.CustomerName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </br>\n                        ");
#nullable restore
#line 42 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                   Write(Model.AccountDebit);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td></td>\n                    <td></td>\n                    <td>\n                        <strong>Meter No:</strong> ");
#nullable restore
#line 47 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                              Write(Model.MeterNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                </tr>\n                <tr class=\"text-nowrap\">\n                    <td>\n                        <strong>Token:</strong> ");
#nullable restore
#line 52 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                           Write(Model.TokenValue);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Amount Paid(BIF):</strong>  ");
#nullable restore
#line 55 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                       Write(Model.ReceivedAmount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Amount Paid:</strong>  ");
#nullable restore
#line 58 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                  Write(Model.AmountWords);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Payment Received From:</strong>  ");
#nullable restore
#line 61 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                            Write(Model.ReceivedFrom);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                </tr>\n                <tr class=\"text-nowrap\">\n                    <td>\n                        <strong>Amount Accepted(BIF):</strong>  ");
#nullable restore
#line 66 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                           Write(Model.AcceptedAmount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Amount Reimbursed(BIF):</strong>  ");
#nullable restore
#line 69 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                             Write(Model.ReimbursementAmount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Token Pricing(BIF):</strong>  ");
#nullable restore
#line 72 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                         Write(Model.KWh);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Units Purchased:</strong>  ");
#nullable restore
#line 75 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                      Write(Model.UnitsPurchased);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr></tr>
    <tr>
        <table class=""table table-striped"" style=""width:80%; font-size:12px"" align=""center"">
            <tr>
                <td><strong>Moyen de paiement</strong></td>
                <td><strong> Référence </strong></td>
                <td><strong>Banque </strong></td>
                <td><strong>Montant payé </strong></td>
            <tr />
            <tr>
                <td>");
#nullable restore
#line 91 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
               Write(Model.PaymentMode);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                <td>");
#nullable restore
#line 92 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
               Write(Model.ReferenceNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                <td>Finbank SA</td>\n                <td>");
#nullable restore
#line 94 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
               Write(Model.ReceivedAmount);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</td>
            </tr>
        </table>
    </tr>
    <tr>
        <table>
            <tr>
                <td> <div style=""text-align:right;"">  Signature et Cachet:  </div></td>
                <td></td>
                <td><div style=""text-align:right;"">................................................................</div></td>
            </tr>
        </table>
    </tr>
</table>

");
            WriteLiteral("\n\n<br />\n<br />\n<br />\n<br/>\n<br/>\n<br />\n<br/>\n<br/>\n<br/>\n<br/>\n<br />\n<br />\n<table style=\"width:80%\" align=\"center\">\n    <tr>\n        <td></td>\n        <td></td>\n        <td></td>\n        <td>\n            <div style=\"text-align:right;\">");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "5c632581cdb550b709b67987d78333230300cba914314", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@" </div>
        </td>
    </tr>
    <tr>
        <td></td>
        <td><div style=""text-align:center;""><h4 style=""font:bold;"">Cash Power Receipt</h4></div></td>
        <td></td>
    </tr>


</table>

<table style=""width:80%;font-size:12px"" align=""center"">
    <tr>
        <td style=""text-align:left;"">
            <table style=""width:100%;"">
                <tr>
                    <td><strong>Date:</strong> ");
#nullable restore
#line 149 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                          Write(Model.ReceiptDate.ToString("dd/MM/yyyy hh:mm tt"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td><strong>Receipt No:</strong> ");
#nullable restore
#line 150 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                Write(Model.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</td>
                </tr>
            </table>
        </td>
    </tr>


    <tr>
        <td>

            <table class=""table table-striped"">
                <tr class=""text-nowrap"">
                    <td>
                        <strong> Customer Name: </strong> ");
#nullable restore
#line 163 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                     Write(Model.CustomerName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </br>\n                        ");
#nullable restore
#line 164 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                   Write(Model.AccountDebit);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td></td>\n                    <td></td>\n                    <td>\n                        <strong>Meter No:</strong> ");
#nullable restore
#line 169 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                              Write(Model.MeterNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n\n                </tr>\n                <tr class=\"text-nowrap\">\n                    <td>\n                        <strong>Token:</strong> ");
#nullable restore
#line 175 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                           Write(Model.TokenValue);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Amount Paid(BIF):</strong>  ");
#nullable restore
#line 178 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                       Write(Model.ReceivedAmount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Amount Paid:</strong>  ");
#nullable restore
#line 181 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                  Write(Model.AmountWords);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Payment Received From:</strong>  ");
#nullable restore
#line 184 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                            Write(Model.ReceivedFrom);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                </tr>\n                <tr class=\"text-nowrap\">\n                    <td>\n                        <strong>Amount Accepted(BIF):</strong>  ");
#nullable restore
#line 189 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                           Write(Model.AcceptedAmount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Amount Reimbursed(BIF):</strong>  ");
#nullable restore
#line 192 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                             Write(Model.ReimbursementAmount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Token Pricing(BIF):</strong>  ");
#nullable restore
#line 195 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                         Write(Model.KWh);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n                    </td>\n                    <td>\n                        <strong>Units Purchased:</strong>  ");
#nullable restore
#line 198 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
                                                      Write(Model.UnitsPurchased);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr></tr>

    <tr>
        <table class=""table table-striped"" style=""width:80%; font-size:12px"" align=""center"">
            <tr>
                <td><strong>Moyen de paiement</strong></td>
                <td><strong> Référence </strong></td>
                <td><strong>Banque </strong></td>
                <td><strong>Montant payé </strong></td>
            <tr />
            <tr>
                <td>");
#nullable restore
#line 215 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
               Write(Model.PaymentMode);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                <td>");
#nullable restore
#line 216 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
               Write(Model.ReferenceNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                <td>Finbank SA</td>\n                <td>");
#nullable restore
#line 218 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Areas\OBR\Views\ObrRpt\TokenReceipt.cshtml"
               Write(Model.ReceivedAmount);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</td>
            </tr>
        </table>
    </tr>
    <tr>
        <table>
            <tr>
                <td> <div style=""text-align:right;"">  Signature et Cachet:  </div></td>
                <td></td>
                <td><div style=""text-align:right;"">................................................................</div></td>
            </tr>
        </table>
    </tr>
</table>

");
            WriteLiteral("\n\n\n\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5c632581cdb550b709b67987d78333230300cba923080", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\n\n");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<TokenReportModels> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
