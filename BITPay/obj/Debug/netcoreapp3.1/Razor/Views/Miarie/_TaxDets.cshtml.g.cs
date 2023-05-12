#pragma checksum "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6312b2aed89724dcc501c53bb8dae49c5fbb3cb1"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Miarie__TaxDets), @"mvc.1.0.view", @"/Views/Miarie/_TaxDets.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6312b2aed89724dcc501c53bb8dae49c5fbb3cb1", @"/Views/Miarie/_TaxDets.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f14dfb8024fdca72d7a4b2bb1b8cf701ebdb915", @"/Views/_ViewImports.cshtml")]
    public class Views_Miarie__TaxDets : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<QueryResp>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("taxForm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "confirmmiariedets", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"
<div class=""row"">
    <div class=""col-md-12"">
        <p class=""alert alert-success text-center"">
            <i class=""fa fa-check-circle-o fa-4x""></i><br />
            <strong>Confirm declaration details to proceed to payment</strong>
        </p>
        <div class=""table-responsive m-t-10"">

");
            WriteLiteral(@"
            <table class=""table table-striped table-hover table-bordered table-responsive-md"" id=""tblData"">
                <thead>
                    <tr>
                        <td>Reference No</td>
                        <td>Declarant Name</td>
                        <td>Declarant Address</td>
                        <td>Description</td>
                        <td>Period</td>
                        <td>Amount</td>
                        <td>Source Amount</td>
                        <td>Paid</td>
                        <td>Late</td>
                        <td></td>
                    </tr>
                </thead>
                <tbody>
");
#nullable restore
#line 46 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                     foreach (var item in Model.Content.TaxNotes)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        <td>");
#nullable restore
#line 49 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                       Write(item.RefNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 50 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                       Write(item.TaxPayerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 51 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                       Write(item.TaxNoteDescr);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 52 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                       Write(item.Period);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 53 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                       Write(item.Amount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 54 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                       Write(item.IsPaid);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 55 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                       Write(item.IsLate);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td>\r\n                        <a data-target=\"#details\" \r\n                           data-toggle=\"modal\"  \r\n                           data-refno=\"");
#nullable restore
#line 59 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                                  Write(item.RefNo);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" \r\n                           data-name=\"");
#nullable restore
#line 60 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                                 Write(item.TaxPayerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" \r\n                           data-descr=\"");
#nullable restore
#line 61 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                                  Write(item.TaxNoteDescr);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" \r\n                           data-period=\"");
#nullable restore
#line 62 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                                   Write(item.Period);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" \r\n                           data-amt=\"");
#nullable restore
#line 63 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                                Write(item.Amount);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" \r\n                           data-paid=\"");
#nullable restore
#line 64 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                                 Write(item.IsPaid);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" \r\n                           data-late=\"");
#nullable restore
#line 65 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                                 Write(item.IsLate);

#line default
#line hidden
#nullable disable
            WriteLiteral("\" \r\n                            data-note=\"");
#nullable restore
#line 66 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                                  Write(item.TaxNoteType);

#line default
#line hidden
#nullable disable
            WriteLiteral("\"\r\n                           class=\"opendetails\"><i class=\"fa fa-cog\"></i> Select</a>\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 70 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Miarie\_TaxDets.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                </tbody>
            </table>
        </div>
    </div>
</div>

<div id=""details"" class=""modal fade"" role=""dialog"" tabindex=""-1"" aria-labelledby=""details"" aria-hidden=""true"">
    <div class=""modal-dialog"" >
        <div class=""modal-content"">
            <div class=""modal-header text-center"">
                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                    <span aria-hidden=""true"">&times;</span>
                </button>
                <h4 class=""modal-title "">Confirm Tax Note Details</h4>
            </div>
            <div class=""modal-body"">
                <div class=""form-group"">
                    <label>RefNo:</label> <input type=""text"" name=""refno"" id=""refno""");
            BeginWriteAttribute("value", " value=\"", 4310, "\"", 4318, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\">\r\n                    <label>Taxpayer Name:</label> <input type=\"text\" name=\"name\" id=\"name\"");
            BeginWriteAttribute("value", " value=\"", 4489, "\"", 4497, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\">\r\n                    <label>Taxpayer Address:</label> <input type=\"text\" name=\"address\" id=\"address\"");
            BeginWriteAttribute("value", " value=\"", 4677, "\"", 4685, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\">\r\n                    <label>Description:</label> <input type=\"text\" name=\"descr\" id=\"descr\"");
            BeginWriteAttribute("value", " value=\"", 4856, "\"", 4864, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\">\r\n                    <label>Period:</label> <input type=\"text\" name=\"period\" id=\"period\"");
            BeginWriteAttribute("value", " value=\"", 5032, "\"", 5040, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\">\r\n                    <label>Amount:</label> <input type=\"text\" name=\"amt\" id=\"amt\"");
            BeginWriteAttribute("value", " value=\"", 5202, "\"", 5210, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\">\r\n                    <label>Amount Source:</label> <input type=\"text\" name=\"amtsrc\" id=\"amtsrc\"");
            BeginWriteAttribute("value", " value=\"", 5385, "\"", 5393, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\">\r\n                    <label>Already Paid:</label> <input type=\"text\" name=\"paid\" id=\"paid\"");
            BeginWriteAttribute("value", " value=\"", 5563, "\"", 5571, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\">\r\n                    <label>Late Payment:</label> <input type=\"text\" name=\"late\" id=\"late\"");
            BeginWriteAttribute("value", " value=\"", 5741, "\"", 5749, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\" hidden>\r\n                    <label>Note Type:</label> <input type=\"text\" name=\"note\" id=\"note\"");
            BeginWriteAttribute("value", " value=\"", 5923, "\"", 5931, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                </div>\r\n                <div class=\"form-group\" hidden>\r\n                    <input type=\"text\" name=\"notetype\" id=\"notetype\"");
            BeginWriteAttribute("value", " value=\"", 6087, "\"", 6095, 0);
            EndWriteAttribute();
            WriteLiteral(" readonly />\r\n                    <input type=\"text\" name=\"noteno\" id=\"noteno\"");
            BeginWriteAttribute("value", " value=\"", 6174, "\"", 6182, 0);
            EndWriteAttribute();
            WriteLiteral(@" readonly />
                </div>
                <div class=""modal-footer"">
                    <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"">Close</button>
                    <button class=""btn btn-primary"" id=""save"" onclick=""func(this);"">Save Taxnote</button>
                </div>
            </div>
        </div>
    </div>
</div>

");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "6312b2aed89724dcc501c53bb8dae49c5fbb3cb115320", async() => {
                WriteLiteral("\r\n    <input id=\"tax_step\" type=\"hidden\" value=\"1\" />\r\n    <input id=\"twende\" type=\"hidden\" value=\"1\" />\r\n");
                WriteLiteral("\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<QueryResp> Html { get; private set; }
    }
}
#pragma warning restore 1591
