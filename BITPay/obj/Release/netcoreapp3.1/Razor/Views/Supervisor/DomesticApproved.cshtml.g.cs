#pragma checksum "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a6f9ca08370f53eb6097142b4e7ed9046f957b86"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Supervisor_DomesticApproved), @"mvc.1.0.view", @"/Views/Supervisor/DomesticApproved.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a6f9ca08370f53eb6097142b4e7ed9046f957b86", @"/Views/Supervisor/DomesticApproved.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e6e5e55c114ed01159ac00bf2e925b97b6d586bc", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Supervisor_DomesticApproved : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<FinishModel>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("target", new global::Microsoft.AspNetCore.Html.HtmlString("_blank"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "domestictaxreceipt", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#nullable restore
#line 2 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
  
    ViewData["Title"] = "Approval Status";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<h2 class=\"header-title\">Approval Status</h2>\n<div class=\"row\">\n    <div class=\"col-md-6\">\n        <div class=\"card-box\">\n");
#nullable restore
#line 10 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
             if (Model.Status == 1)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"alert alert-warning text-center\">\n                <i class=\"fa fa-times fa-5x\"></i><br />\n                <h2 class=\"text-warning\">");
#nullable restore
#line 14 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
                                    Write(Model.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\n                <strong class=\"text-danger\">");
#nullable restore
#line 15 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
                                       Write(Model.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\n            </div>\n");
#nullable restore
#line 17 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
            }
            else if (Model.Status == 2)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"alert alert-success text-center\">\n                <i class=\"fa fa-check fa-5x\"></i><br />\n                <h2 class=\"text-success\">");
#nullable restore
#line 22 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
                                    Write(Model.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\n                <strong></strong>\n            </div>\n");
#nullable restore
#line 25 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
            }
            else
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"alert alert-success text-center\">\n                <i class=\"fa fa-check fa-5x\"></i><br />\n                <h2 class=\"text-success\">");
#nullable restore
#line 30 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
                                    Write(Model.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\n                <strong>");
#nullable restore
#line 31 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
                   Write(Model.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>\n            </div>\n            <div class=\"text-center text-muted\">\n                <span>Click on the button below to view transaction receipt</span><br />\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "a6f9ca08370f53eb6097142b4e7ed9046f957b868057", async() => {
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
#line 35 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
                                                                                               WriteLiteral(Model.Code);

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
            WriteLiteral("\n            </div>\n");
#nullable restore
#line 37 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Supervisor\DomesticApproved.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\n        </div>\n    </div>\n</div>\n\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<FinishModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
