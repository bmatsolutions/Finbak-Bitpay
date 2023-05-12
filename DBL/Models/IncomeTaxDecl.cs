using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class IncomeTaxDecl
    {
        [Required]
        [Display(Name = "Nif")]
        public string tin { get; set; }

        [Required]
        [Display(Name = "Tax Period")]
        public string Period { get; set; }

        [Required]
        [Display(Name = "Domestic Tax Group")]
        public string DomesticTax { get; set; }

        [Required]
        [Display(Name = "Domestic Tax Sub Group")]
        public string DomesticTaxType { get; set; }

        [Required]
        [Display(Name = "Domestic Tax Name")]
        public string DomesticTaxName { get; set; }
        [Required]
        [Display(Name = "Tax Adjustment Catergory")]
        public string TaxAdjustmentCat { get; set; }

        [Required]
        [Display(Name = "Declaration Number")]
        public string DeclNo { get; set; }

        [Required]
        [Display(Name = "Commune Name")]
        public string commune { get; set; }

        [Required]
        [Display(Name = "Product Type/Quantity")]
        public string Product { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public string AmountPayable { get; set; }

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Required]
        [Display(Name = "Licence Type")]
        public string LicenceType { get; set; }

        [Required]
        [Display(Name = "Licence Type")]
        public string LicencesType { get; set; }

        [Required]
        [Display(Name = "Wording of right")]
        public string Wording { get; set; }

        [Required]
        [Display(Name = "Service Name")]
        public string Service { get; set; }

        [Required]
        [Display(Name = "Car Owner Name")]
        public string CarOwner { get; set; }

        [Required]
        [Display(Name = "Contraviction Number")]
        public string Contraviction { get; set; }

        [Required]
        [Display(Name = "Driver Full Name")]
        public string DriverName { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomName { get; set; }

        [Required]
        [Display(Name = "Infraction")]
        public string Infraction { get; set; }

        [Required]
        [Display(Name = "Car Chassis")]
        public string CarChassis { get; set; }

        [Required]
        [Display(Name = "Marque/Modele/ numero de plaque/ nombre de CC")]
        public string CarDetails { get; set; }

        [Required]
        [Display(Name = "Car Immatriculation Number")]
        public string CarImma { get; set; }

        [Required]
        [Display(Name = "Document Name/Lost Item")]
        public string Document { get; set; }

        [Required]
        [Display(Name = "Education/Order/Document Type")]
        public string EducationDoc { get; set; }

        [Required]
        [Display(Name = "Total Copies")]
        public string TotCopies { get; set; }

        [Required]
        [Display(Name = "Tax Adjustment Type")]
        public string AdjustMentType { get; set; }

        [Required]
        [Display(Name = "Delay Majoration")]
        public string DelayMajoration { get; set; }

        [Required]
        [Display(Name = "Civil Status")]
        public string CivilStatus { get; set; }

        [Required]
        [Display(Name = "CNI Number")]
        public string CNI { get; set; }

        [Required]
        [Display(Name = "Place and Date of Issue of CNI")]
        public string CNIIssue { get; set; }

        [Required]
        [Display(Name = "Year of Manufacture")]
        public string ManufactureYear { get; set; }

        [Required]
        [Display(Name = "Brand and Type")]
        public string BrandType { get; set; }
    }


    public class QueryPayment
    {
        [Required]
        [Display(Name = "Domestic Tax Reference Number")]
        public string RefNo { get; set; }

    }
}
