#pragma checksum "F:\OFFICEWORK\PROJECTS\SOURCECODE\Burundi\BITPAY\Finbank\Bitpay\Bitpay\BITPay\Views\Domestic\_QueryError.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "83bc1642044ff93a5e00ccdec16521c7cf540e30"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Domestic__QueryError), @"mvc.1.0.view", @"/Views/Domestic/_QueryError.cshtml")]
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
#line 1 "F:\OFFICEWORK\PROJECTS\SOURCECODE\Burundi\BITPAY\Finbank\Bitpay\Bitpay\BITPay\Views\_ViewImports.cshtml"
using BITPay;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "F:\OFFICEWORK\PROJECTS\SOURCECODE\Burundi\BITPAY\Finbank\Bitpay\Bitpay\BITPay\Views\_ViewImports.cshtml"
using BITPay.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "F:\OFFICEWORK\PROJECTS\SOURCECODE\Burundi\BITPAY\Finbank\Bitpay\Bitpay\BITPay\Views\_ViewImports.cshtml"
using BITPay.DBL.Entities;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "F:\OFFICEWORK\PROJECTS\SOURCECODE\Burundi\BITPAY\Finbank\Bitpay\Bitpay\BITPay\Views\_ViewImports.cshtml"
using BITPay.DBL.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"83bc1642044ff93a5e00ccdec16521c7cf540e30", @"/Views/Domestic/_QueryError.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f14dfb8024fdca72d7a4b2bb1b8cf701ebdb915", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Domestic__QueryError : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ErrorModel>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-6\">\r\n        <p class=\"text-center alert alert-danger\">\r\n            <i class=\"fa fa-warning fa-4x\"></i><br />\r\n            <span class=\"font-bold\">Failed!</span><br />\r\n            <strong>");
#nullable restore
#line 8 "F:\OFFICEWORK\PROJECTS\SOURCECODE\Burundi\BITPAY\Finbank\Bitpay\Bitpay\BITPay\Views\Domestic\_QueryError.cshtml"
               Write(Model.ErrorMessage);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong>  <br />\r\n            Try again!\r\n        </p>\r\n    </div>\r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ErrorModel> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
