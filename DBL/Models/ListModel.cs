using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class ListModel
    {
        public ListModel()
        {
        }

        public ListModel(string text, string value)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; set; }
        public string Value { get; set; }
        public int TypeCode { get; set; }

    }
}
