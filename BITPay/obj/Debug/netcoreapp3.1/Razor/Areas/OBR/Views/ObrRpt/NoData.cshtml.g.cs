#pragma checksum "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Areas\OBR\Views\ObrRpt\NoData.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "58434f18903721c9b0620356ae39b335dd66cd07"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Areas_OBR_Views_ObrRpt_NoData), @"mvc.1.0.view", @"/Areas/OBR/Views/ObrRpt/NoData.cshtml")]
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
#line 1 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Areas\OBR\Views\_ViewImports.cshtml"
using BITPay;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Areas\OBR\Views\_ViewImports.cshtml"
using BITPay.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Areas\OBR\Views\_ViewImports.cshtml"
using BITPay.DBL.Entities;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Areas\OBR\Views\_ViewImports.cshtml"
using BITPay.DBL.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"58434f18903721c9b0620356ae39b335dd66cd07", @"/Areas/OBR/Views/ObrRpt/NoData.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f14dfb8024fdca72d7a4b2bb1b8cf701ebdb915", @"/Areas/OBR/Views/_ViewImports.cshtml")]
    public class Areas_OBR_Views_ObrRpt_NoData : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ReceiptReportModels>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Areas\OBR\Views\ObrRpt\NoData.cshtml"
  
    ViewData["Title"] = "NoData";
    Layout = "_LayoutReport";

#line default
#line hidden
#nullable disable
            WriteLiteral("<hr />\r\n<h1 style=\"color:#ff0000\">No Data!</h1>\r\n<h3 style=\"color:#ff0000\">No data found to display for this report</h3>\r\n<p style=\"color:#ff0000\">\r\n    ");
#nullable restore
#line 10 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Areas\OBR\Views\ObrRpt\NoData.cshtml"
Write(Model.CustomMessage);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    <br/>\r\n    Try to print another report or change report filters.\r\n\r\n</p>\r\n\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ReceiptReportModels> Html { get; private set; }
    }
}
#pragma warning restore 1591
