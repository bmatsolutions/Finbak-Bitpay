#pragma checksum "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Regideso\PayBill.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "45308c051a38d940bc744c9383e798dca4e20ce6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Regideso_PayBill), @"mvc.1.0.view", @"/Views/Regideso/PayBill.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"45308c051a38d940bc744c9383e798dca4e20ce6", @"/Views/Regideso/PayBill.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f14dfb8024fdca72d7a4b2bb1b8cf701ebdb915", @"/Views/_ViewImports.cshtml")]
    public class Views_Regideso_PayBill : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery.steps/demo/css/jquery.steps.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/jquery.steps/build/jquery.steps.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral("\r\n");
#nullable restore
#line 2 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Regideso\PayBill.cshtml"
  
    ViewData["Title"] = "Regideso Pay Bill";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h2 class=\"header-title\">Pay Bill</h2>\r\n<div id=\"mywizard\">\r\n    <h3>Query Bills</h3>\r\n    <section data-mode=\"async\" data-url=\"");
#nullable restore
#line 9 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Regideso\PayBill.cshtml"
                                    Write(Url.Action("QueryPostPay"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"""></section>
    <h3>Make Payment</h3>
    <section>
        <div id=""paymentContent"">
            <h3><i class=""fa fa-spin fa-spinner m-r-5""></i> <strong>Please wait........</strong></h3>
        </div>
    </section>
    <h3>Confirm</h3>
    <section>
        <div id=""confirmContent"">
            <h3><i class=""fa fa-spin fa-spinner m-r-5""></i> <strong>Please wait........</strong></h3>
        </div>
    </section>
    <h3>Finish</h3>
    <section>
        <div id=""finishContent"">
            <h3><i class=""fa fa-spin fa-spinner m-r-5""></i> <strong>Please wait........</strong></h3>
        </div>
    </section>
</div>

");
            DefineSection("css", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "45308c051a38d940bc744c9383e798dca4e20ce65939", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
            }
            );
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n    ");
#nullable restore
#line 34 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Regideso\PayBill.cshtml"
Write(await Html.PartialAsync("_ValidationScriptsPartial"));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "45308c051a38d940bc744c9383e798dca4e20ce67498", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"

    <script>
        var ur =  '/regideso/BillConfirm';
         var dt = {};
       

        var settings = {
            headerTag: ""h3"",
            bodyTag: ""section"",
            transitionEffect: ""slideLeft"",
            transitionEffectSpeed: 400,
            /* Events */
            onContentLoaded: function (event, currentIndex) {
                if (currentIndex == 0) {
                    listenPayModeChange();
                    //--- Manage types
                    $('#myTaxType').on(""change"", function () {
                        var type = $(this).val();
                        loadTaxTypes(type);
                    });
                    $('#myTaxType').change();

                    $('#myTypeCode').on(""change"", function () {
                        var type = $(this).val();
                        loadTypeFields(type);
                    });  
                     
                }
                //////listenPayModeChange();
            },
           ");
                WriteLiteral(@" onStepChanging: function (event, currentIndex, newIndex) {
                if ($('.frm-validate').length) {
                    $('.frm-validate').validate();
                    if ($('.frm-validate').valid() == false) {
                        console.log('Validation failed!');
                        return false;
                    }
                    console.log('Validation passed.');
                }
                return changingStep(event, currentIndex, newIndex);
            },
            onStepChanged: function (event, currentIndex, priorIndex) {
                stepChanged(event, currentIndex, priorIndex);
            },
            onFinishing: function (event, currentIndex) {
                return true;
            },
            onFinished: function (event, currentIndex) {
                location.reload();
            },
            labels: {
                finish: ""Finish <i class='fa fa-check'></i>"",
                next: ""Next <i class='fa fa-arrow-right'></i>""");
                WriteLiteral(@",
                previous: ""<i class='fa fa-arrow-left'></i> Previous"",
                loading: ""Loading ...""
            },
            loadingTemplate: '<h3><i class=""fa fa-spin fa-spinner""></i> <strong>#text#</strong></h3>'
        };

        $(""#mywizard"").steps(settings);

        function changingStep(event, currentIndex, newIndex) {
            console.log(currentIndex + "", "" + newIndex);
            var progress = '<h3><i class=""fa fa-spin fa-spinner""></i> <strong>Please wait........</strong></h3>';
            if (currentIndex == 0 && newIndex == 1) {
              var step = $(""#tax_step"").val();
                if (step === ""0"") {
                    if(Object.keys(dt).length > 0)
                    {
                    var myForm = $('#frmQuey');
                    $(""#taxDetsContent"").html(progress); 
                        $.ajax({
                            type: 'Post',
                            url: myForm.attr('action'),
                            data: dt
 ");
                WriteLiteral(@"                       })
                    .done(function (resp) {
                        $(""#paymentContent"").html(resp);
                                listenPayModeChange();
                         });
                       return true;
                    }else{
                    listenPayModeChange();
                    return false;
                    }
                } else {
                    listenPayModeChange();
                    return false;
                }
            }

            if (currentIndex == 1 && newIndex == 2) {
                //--- Payment screen
                if ($(""#twende"").val() === ""1"") {
                    //----- Post form
                    var myForm = $('#taxForm');
                    var frmData = myForm.serialize();
                    $.ajax({
                        type: myForm.attr('method'),
                        url: myForm.attr('action'),
                        data: frmData
                    })
             ");
                WriteLiteral(@"           .done(function (resp) {
                           
                            $(""#confirmContent"").html(resp);
                            listenPayModeChange();
                        });

                    return true;
                } else {
                    return false;
                }
            }
            if (currentIndex == 2 && newIndex == 3) {
                //--- Confirmation screen
                if ($(""#twende"").val() === ""1"") {
                    //----- Post form
                    var myForm = $('#confirmForm');
                    var frmData = myForm.serialize();
                    $.ajax({
                        type: myForm.attr('method'),
                        url: myForm.attr('action'),
                        data: frmData
                    })
                        .done(function (resp) {
                            $(""#finishContent"").html(resp);
                            listenPayModeChange();
                        });");
                WriteLiteral(@"

                    return true;
                } else {
                    return false;
                }
            }
            if (currentIndex == 3) {
                //--- Never go back the process was a success
                if ($(""#twende"").val() === ""1"") {
                    return false;
                }
            }
            if (currentIndex == 1 && newIndex == 0) {
                return false;
            }
            return true;
        }

        function stepChanged(event, currentIndex, newIndex) {


        }
        
       
        function listenPayModeChange() {
            $(""#pay_mode"").on(""change"", function () {
                var mode = $(this).val();
                console.log('Pay Mode: ' + mode);
                $(""#account_no"").hide();
                $(""#cheque_no"").hide();
                $(""#sort_code"").hide();

                if (mode === ""1"") {
                    $(""#account_no"").show();
                } else if (mode =");
                WriteLiteral(@"== ""2"") {
                    $(""#account_no"").show();
                    $(""#cheque_no"").show();
                } else if (mode === ""3"") {
                    $(""#cheque_no"").show();
                    $(""#sort_code"").show();
                    loadBanks();
                } else if (mode === ""4"") {
                    $(""#cheque_no"").show();
                    $(""#sort_code"").show();
                    loadBanks();
                }

                setCharge(mode);

            });
            $(""#pay_mode"").change();
        }

        function setCharge(mode) {
            var url = """);
#nullable restore
#line 218 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Regideso\PayBill.cshtml"
                  Write(Url.Action("GetPayMode"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""";
            $.ajax({
                type: 'get',
                url: url,
                data: { payMode: mode}
            })
                .done(function (resp) {
                    $(""#chargeVal"").val(resp);
                });

        }

        function listenPaymentStatus() {
            $(""#btnRefreshStat"").click(function (e) {
                e.preventDefault();
                $('#payStatView').html('<h3><i class=""fa fa-spin fa-spinner m-r-5""></i> <strong>Loading..............</strong></h3>');
                var url = $(this).attr('href');
                $.get(url, function (data) {
                    $('#payStatView').html(data);
                });
            });
        }

        function loadBanks() {
            $(""#banks"").empty();

            var url = """);
#nullable restore
#line 244 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Regideso\PayBill.cshtml"
                  Write(Url.Action("getbanks"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""";
            $.getJSON(url, function (data) {
                $.each(data, function (i, item) {
                    $(""#banks"").append('<option value=""' + item.value + '"">' + item.text + '</option>');
                });

            })
        }

        $(document).on('click', "".opendetails"", function () {
          
            var amnt = $(this).attr('data-amnt');
            var accnt_no = $(this).attr('data-accnt_no');
            var cust_no = $(this).attr('data-cust_no');
            var year = $(this).attr('data-year');
            var month = $(this).attr('data-month');
            var invoice_no = $(this).attr('data-invoice_no');
            var cust_name = $(this).attr('data-cust_name');
            var activity = $(this).attr('data-activity');
            $('.modal-body #amnt').val(amnt);
            $('.modal-body #accnt_no').val(accnt_no);
            $('.modal-body #cust_no').val(cust_no);
            $('.modal-body #year').val(year);
            $('.modal-body #month'");
                WriteLiteral(@").val(month);
            $('.modal-body #invoice_no').val(invoice_no);
            $('.modal-body #cust_name').val(cust_name);
            $('.modal-body #activity').val(activity);
        });

        //function func(x) {
           
        //    var amnt_paid = document.getElementById(""amnt_paid"").value;
        //    var amnt = document.getElementById(""amnt"").value;
        //    var accnt_no = document.getElementById(""accnt_no"").value;
        //    var cust_no = document.getElementById(""cust_no"").value;
        //    var year = document.getElementById(""year"").value;
        //    var month = document.getElementById(""month"").value;
        //    var invoice_no = document.getElementById(""invoice_no"").value;
        //    var cust_name = document.getElementById(""cust_name"").value;
        //    var activity = document.getElementById(""activity"").value;
        //     console.log(""good new"")
        //    $.ajax({
        //        url: '/regideso/BillConfirm',
        //        type: 'P");
                WriteLiteral(@"ost',
        //        dataType: 'application/json',
        //        data: { 'amnt_paid': amnt_paid, 'amnt': amnt, 'accnt_no': accnt_no, 'cust_no': cust_no, 'year': year, 'month': month, 'invoice_no': invoice_no,'cust_name':cust_name,'activity':activity },
        //        success: function (data) {
        //           var msg =JSON.parse(data.responseText);
        //            var success = msg.success;
        //            var response = msg.responseText;

        //            alert(response);
        //        },
        //        error: function (data) {
        //            var msg =JSON.parse(data.responseText);
        //            var success = msg.success;
        //            var response = msg.responseText;

        //            alert(response);
        //        }
        //    });
        //}
        function func(x) {
             var progress = '<h3><i class=""fa fa-spin fa-spinner""></i> <strong>Please wait........</strong></h3>';
            var amnt = document");
                WriteLiteral(@".getElementById(""amnt"").value;
            var accnt_no = document.getElementById(""accnt_no"").value;
            var cust_no = document.getElementById(""cust_no"").value;
            var year = document.getElementById(""year"").value;
            var month = document.getElementById(""month"").value;
            var invoice_no = document.getElementById(""invoice_no"").value;
            var cust_name = document.getElementById(""cust_name"").value;
            var activity = document.getElementById(""activity"").value;
             dt =  {'amnt': amnt, 'accnt_no': accnt_no, 'cust_no': cust_no, 'year': year, 'month': month, 'invoice_no': invoice_no,'cust_name':cust_name,'activity':activity }
             $('#details').modal('hide');
              $(""#frmQuey"").show();
              //$(""#mywizard"").steps(settings);
        }

        function resetValidation (form) {
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form");
                WriteLiteral(@");
            form.addClass('frm-validate');
        }

        function loadTaxTypes(type) {
            console.log(""Type change -> "", type);

            $('#myTypeCode').empty();
            $('#myTypeCode').append(new Option('Loading........'));
            var url = """);
#nullable restore
#line 334 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Regideso\PayBill.cshtml"
                  Write(Url.Action("gettaxtypes"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"/"" + type;
            $.get(url, function (data) {
                $('#myTypeCode').empty();
                $.each(data, function () {
                    $('#myTypeCode').append(new Option(this.name, this.code));
                });
                $('#myTypeCode').change();
            })
        }

        function loadTypeFields(type) {
            console.log(""Fields -> "", type);
            $('#typeFields').html('<div class=""row""><div class=""col-md-offset-5 col-md-7""><p><i class=""fa fa-spin fa-spinner""></i> Loading....</p></div></div>');
            var url = """);
#nullable restore
#line 347 "D:\Projects\Work\Finbank Bitpay 2023\BITPay\Views\Regideso\PayBill.cshtml"
                  Write(Url.Action("gettypefields"));

#line default
#line hidden
#nullable disable
                WriteLiteral("/\" + type;\r\n            $.get(url, function (data) {\r\n                $(\'#typeFields\').html(data);\r\n\r\n                resetValidation($(\'#frmQuey\'));\r\n            })           \r\n        }\r\n    </script>\r\n    ");
            }
            );
            WriteLiteral("\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
