#pragma checksum "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a2532d15e3a39f04a60ab8432ddd07128f8246f5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_RegidesoPrePay__Finish), @"mvc.1.0.view", @"/Views/RegidesoPrePay/_Finish.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a2532d15e3a39f04a60ab8432ddd07128f8246f5", @"/Views/RegidesoPrePay/_Finish.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f14dfb8024fdca72d7a4b2bb1b8cf701ebdb915", @"/Views/_ViewImports.cshtml")]
    public class Views_RegidesoPrePay__Finish : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<PrePaidModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("target", new global::Microsoft.AspNetCore.Html.HtmlString("_blank"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "PrePayReceipt", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "report", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-default btn-sm m-t-5"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-6\">\r\n        <div class=\"alert alert-success text-center\">\r\n            <i class=\"fa fa-check fa-5x\"></i><br />\r\n            <h2 class=\"text-success\">Successful.</h2>\r\n            <strong>");
#nullable restore
#line 8 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
               Write(Model.Msg);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n        </div>\r\n\r\n        <div class=\"row\">\r\n            <div class=\"col-md-4\">Meter No:</div>\r\n            <div class=\"col-md-8 control-label\">\r\n                <strong>");
#nullable restore
#line 14 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
                   Write(Model.Meter_No);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n            </div>\r\n        </div>\r\n        <div class=\"row\">\r\n            <div class=\"col-md-4\">Token:</div>\r\n            <div class=\"col-md-8 control-label\">\r\n                <strong>");
#nullable restore
#line 20 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
                   Write(Model.Token3);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n            </div>\r\n        </div>\r\n         <div class=\"row\">\r\n            <div class=\"col-md-4\">Units:</div>\r\n            <div class=\"col-md-8 control-label\">\r\n                <strong>");
#nullable restore
#line 26 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
                   Write(Model.units);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\r\n            </div>\r\n        </div>\r\n\r\n");
#nullable restore
#line 30 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
         if (!Model.NeedApproval)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"text-center text-muted \">\r\n                <span>Click on the button below to view transaction receipt</span><br />\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a2532d15e3a39f04a60ab8432ddd07128f8246f57031", async() => {
                WriteLiteral("<i class=\"fa fa-print\"></i> Print Receipt");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-code", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 34 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
                                                                                          WriteLiteral(Model.BillCode);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["code"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-code", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["code"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n            </div>\r\n");
#nullable restore
#line 36 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\r\n    <div class=\"col-md-6\">\r\n");
#nullable restore
#line 39 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
         if (Model.NeedApproval == true)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            <div class=""panel panel-default"">
                <div class=""panel-body"">
                    <h3><i class=""fa fa-spin fa-spinner m-r-5""></i> <strong>Loading..............</strong></h3>
                    <button class=""btn btn-success"" id=""paymentStatus""");
            BeginWriteAttribute("value", " value=\"", 1712, "\"", 1735, 1);
#nullable restore
#line 44 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
WriteAttributeValue("", 1720, Model.BillCode, 1720, 15, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n                        <i class=\"fa fa-refresh\" aria-hidden=\"true\"></i> Refresh\r\n                    </button>\r\n\r\n                </div>\r\n            </div>\r\n");
#nullable restore
#line 50 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\RegidesoPrePay\_Finish.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\r\n</div>\r\n<input id=\"twende\" type=\"hidden\" value=\"1\" />\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<PrePaidModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
