#pragma checksum "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Tax\_NotKnown.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c957442b3e46a3d8249d39f63a64ed2dd683a007"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Tax__NotKnown), @"mvc.1.0.view", @"/Views/Tax/_NotKnown.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c957442b3e46a3d8249d39f63a64ed2dd683a007", @"/Views/Tax/_NotKnown.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e6e5e55c114ed01159ac00bf2e925b97b6d586bc", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Tax__NotKnown : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<DeclarationQueryResponse>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<div class=\"row\">\n    <div class=\"col-md-6\">\n        <div class=\"alert alert-danger text-center\">\n            <i class=\"fa fa-warning fa-4x\"></i><br />\n            <h3 class=\"text-danger\">");
#nullable restore
#line 6 "F:\Projects\c#\Finbank-BITPay-master\BITPay\Views\Tax\_NotKnown.cshtml"
                               Write(Model.ErrorDescription);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h3>\n        </div>\n\n        <div class=\"text-center\">\n            <span class=\"text-muted\">Something is not right!<br />Please check.</span>\n        </div>\n    </div>\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<DeclarationQueryResponse> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
