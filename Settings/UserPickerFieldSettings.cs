using System.Collections.Generic;
using System.Web.Mvc;

namespace JustAProgrammer.UserPicker.Settings
{
    public class UserPickerFieldSettings
    {
        public string Hint { get; set; }
        public bool Required { get; set; }
        public bool Multiple { get; set; }

        public int QueryId { get; set; }

        public IList<SelectListItem> QueryList { get; set; }
    }
}
