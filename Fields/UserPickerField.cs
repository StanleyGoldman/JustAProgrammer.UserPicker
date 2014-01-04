using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Orchard.ContentManagement.Utilities;
using Orchard.Security;

namespace JustAProgrammer.UserPicker.Fields
{
    public class UserPickerField : ContentField
    {
        private static readonly char[] Separator = new[] { ',' };
        internal LazyField<IEnumerable<IUser>> users = new LazyField<IEnumerable<IUser>>();

        public string[] Usernames
        {
            get { return SplitUsernames(Storage.Get<string>()); }
            set { Storage.Set(JoinUsernames(value)); }
        }

        public IEnumerable<IUser> ContentItems
        {
            get
            {
                return users.Value;
            }
        }

        private string JoinUsernames(ICollection<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                return string.Empty;
            }

            return string.Join(",", ids.ToArray());
        }

        private string[] SplitUsernames(string ids)
        {
            if (String.IsNullOrWhiteSpace(ids))
            {
                return new string[0];
            }

            return ids.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }
    }
}
