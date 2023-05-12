#pragma checksum "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "883d28acf0291a45f805487fadcb06b08012df08"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Report_MiarieTaxReceipt), @"mvc.1.0.view", @"/Views/Report/MiarieTaxReceipt.cshtml")]
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
#line 1 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\_ViewImports.cshtml"
using BITPay;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\_ViewImports.cshtml"
using BITPay.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\_ViewImports.cshtml"
using BITPay.DBL.Entities;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\_ViewImports.cshtml"
using BITPay.DBL.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"883d28acf0291a45f805487fadcb06b08012df08", @"/Views/Report/MiarieTaxReceipt.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e6e5e55c114ed01159ac00bf2e925b97b6d586bc", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Report_MiarieTaxReceipt : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MiarieReportModels>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery/dist/jquery.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral("\n");
#nullable restore
#line 3 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
  
    Layout = "_LayoutReceipt";
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
            WriteLiteral("\n<br />\n<table style=\"width:100%\">\n    <tr>\n        <td><div style=\"text-align:right;\"><h4 style=\"font:bold;\">RECU DE PAIEMENT</h4></div></td>\n");
            WriteLiteral("    </tr>\n</table>\n<table style=\"width:100%;font-size:12px\">\n    <tr>\n        <td style=\"text-align:left;\">\n            <table style=\"width:100%;\">\n                <tr>\n                    <td><strong>Numèro  D\'Enregistrement de paiement:</strong> ");
#nullable restore
#line 36 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                                          Write(Model.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td style=\"text-align: center;\"><strong>Date D\'Enregistrement du paiement:</strong> ");
#nullable restore
#line 37 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                                                                   Write(Model.ReceiptDate.ToString("dd/MM/yyyy hh:mm tt"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                </tr>\n            </table>\n        </td>\n    </tr>\n");
            WriteLiteral("    <tr>\n        <td style=\"text-align:left;\">\n            <strong> Détails du contribuable: </strong> ");
#nullable restore
#line 49 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                   Write(Model.PayerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n        </td>\n    </tr>\n\n");
            WriteLiteral(@"</table>
<table style=""width:100%"">
    <tr>
        <td><div style=""text-align:center;""><h5 style=""font:bold;"">Dètails de Paiement</h5></div></td>
    </tr>
</table>
<table class=""table table-striped"" style=""width:100%;font-size:12px"">
    <tr>
        <td> <strong>Détails de la taxe intérieure</strong></td>
        <td> <strong>Détails du déclarant fiscal</strong></td>
        <td> <strong>Période Fiscale </strong></td>
        <td> <strong></strong></td>
    </tr>
    <tr>
        <td>");
#nullable restore
#line 86 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.Descr);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 87 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.PayerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 88 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.Period);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td></td>\n\n");
            WriteLiteral("    <tr>\n        <td><strong>Moyen de paiement</strong></td>\n        <td><strong>Banque Référence </strong></td>\n        <td><strong>Banque </strong></td>\n        <td><strong>Montant payé </strong></td>\n    <tr />\n    <tr>\n\n        <td>");
#nullable restore
#line 100 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.PaymentMode);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 101 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.ReferenceNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 102 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(title);

#line default
#line hidden
#nullable disable
            WriteLiteral(" - ");
#nullable restore
#line 102 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                Write(Model.BranchName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 103 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.ReceivedAmount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n    </tr>\n");
            WriteLiteral("\n    <tr><td><strong>Payé par:</strong></td><td> ");
#nullable restore
#line 120 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                           Write(Model.ReceivedFrom);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td> <td> <strong>Remarques:</strong></td><td>");
#nullable restore
#line 120 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                                                                             Write(Model.Remarks);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td></tr>\n    <tr><td><strong>Date de réception:</strong></td><td> ");
#nullable restore
#line 121 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                    Write(DateTime.Now.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td><td><strong>Signature du client: </strong></td><td></td></tr>\n    <tr><td><strong>Code de recherche:</strong></td><td>");
#nullable restore
#line 122 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                   Write(Model.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral(@" (Utilisez ce numéro pour demander l'état de votre bulletin de versement)</td></tr>
    <tr><td><strong>Finbank SA Reconnait avoir encaisse la somme ci-dessus.</strong></td></tr>
    <tr>
        <td><div>Signature et Cachet:  </div></td>
        <td></td>
        <td><div style=""text-align:right;"">................................................................</div></td>
    </tr>
</table>
");
            WriteLiteral("\n");
            WriteLiteral("<br />\n<br />\n<br />\n<br />\n<br />\n<br />\n<br />\n<br />\n<table style=\"width:100%\">\n    <tr>\n        <td><div style=\"text-align:right;\"><h4 style=\"font:bold;\">RECU DE PAIEMENT</h4></div></td>\n");
            WriteLiteral("    </tr>\n</table>\n<table style=\"width:100%;font-size:12px\">\n    <tr>\n        <td style=\"text-align:left;\">\n            <table style=\"width:100%;\">\n                <tr>\n                    <td><strong>Numèro  D\'Enregistrement de paiement:</strong> ");
#nullable restore
#line 158 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                                          Write(Model.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td style=\"text-align: center;\"><strong>Date D\'Enregistrement du paiement:</strong> ");
#nullable restore
#line 159 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                                                                   Write(Model.ReceiptDate.ToString("dd/MM/yyyy hh:mm tt"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                </tr>\n            </table>\n        </td>\n    </tr>\n");
            WriteLiteral("    <tr>\n        <td style=\"text-align:left;\">\n            <strong> Détails du contribuable: </strong> ");
#nullable restore
#line 171 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                   Write(Model.PayerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\n        </td>\n    </tr>\n\n");
            WriteLiteral(@"</table>
<table style=""width:100%"">
    <tr>
        <td><div style=""text-align:center;""><h5 style=""font:bold;"">Dètails de Paiement</h5></div></td>
    </tr>
</table>
<table class=""table table-striped"" style=""width:100%;font-size:12px"">
    <tr>
        <td> <strong>Détails de la taxe intérieure</strong></td>
        <td> <strong>Détails du déclarant fiscal</strong></td>
        <td> <strong>Période Fiscale </strong></td>
        <td> <strong></strong></td>
    </tr>
    <tr>
        <td>");
#nullable restore
#line 208 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.Descr);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 209 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.PayerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 210 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.Period);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td></td>\n\n");
            WriteLiteral("    <tr>\n        <td><strong>Moyen de paiement</strong></td>\n        <td><strong>Banque Référence </strong></td>\n        <td><strong>Banque </strong></td>\n        <td><strong>Montant payé </strong></td>\n    <tr />\n    <tr>\n\n        <td>");
#nullable restore
#line 222 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.PaymentMode);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 223 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.ReferenceNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 224 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(title);

#line default
#line hidden
#nullable disable
            WriteLiteral(" - ");
#nullable restore
#line 224 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                Write(Model.BranchName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n        <td>");
#nullable restore
#line 225 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
       Write(Model.ReceivedAmount.ToString("#,##0.00"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n    </tr>\n");
            WriteLiteral("\n    <tr><td><strong>Payé par:</strong></td><td> ");
#nullable restore
#line 242 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                           Write(Model.ReceivedFrom);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td> <td> <strong>Remarques:</strong></td><td>");
#nullable restore
#line 242 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                                                                             Write(Model.Remarks);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td></tr>\n    <tr><td><strong>Date de réception:</strong></td><td> ");
#nullable restore
#line 243 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                    Write(DateTime.Now.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td><td><strong>Signature du client: </strong></td><td></td></tr>\n    <tr><td><strong>Code de recherche:</strong></td><td>");
#nullable restore
#line 244 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Report\MiarieTaxReceipt.cshtml"
                                                   Write(Model.ReceiptNo);

#line default
#line hidden
#nullable disable
            WriteLiteral(@" (Utilisez ce numéro pour demander l'état de votre bulletin de versement)</td></tr>
    <tr><td><strong>Finbank SA Reconnait avoir encaisse la somme ci-dessus.</strong></td></tr>
    <tr>
        <td><div>Signature et Cachet:  </div></td>
        <td></td>
        <td><div style=""text-align:right;"">................................................................</div></td>
    </tr>
</table>
");
            WriteLiteral("\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "883d28acf0291a45f805487fadcb06b08012df0817571", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MiarieReportModels> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
