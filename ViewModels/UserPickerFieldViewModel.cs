using System.Collections.Generic;
using System.Web.Mvc;
using JustAProgrammer.UserPicker.Fields;
using Orchard.ContentManagement;
using Orchard.Security;

namespace JustAProgrammer.UserPicker.ViewModels
{
    public class UserPickerFieldViewModel
    {
        public List<SelectListItem> UserSelectionList { get; set; }

        public string[] SelectedUserNames { get; set; }
        public string SelectedUserName { get; set; }

        public UserPickerField Field { get; set; }
        public ContentPart Part { get; set; }
    }
}