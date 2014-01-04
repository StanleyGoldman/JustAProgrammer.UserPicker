using System.Linq;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Security;

namespace JustAProgrammer.UserPicker.Handlers
{
    public class UserPickerFieldHandler : ContentHandler
    {
        private readonly IMembershipService _membershipService;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public UserPickerFieldHandler(IMembershipService membershipService, IContentDefinitionManager contentDefinitionManager)
        {
            _membershipService = membershipService;
            _contentDefinitionManager = contentDefinitionManager;
        }

        protected override void Loading(LoadContentContext context)
        {
            base.Loading(context);

            var fields = context.ContentItem.Parts
                .SelectMany(x => x.Fields.Where(f => f.FieldDefinition.Name == typeof(Fields.UserPickerField).Name))
                .Cast<Fields.UserPickerField>();

            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(context.ContentType);
            if (contentTypeDefinition == null)
            {
                return;
            }

            foreach (var field in fields)
            {
                var localField = field;
                field.users.Loader(x => localField.Usernames.Select(username => _membershipService.GetUser(username)));
            }
        }
    }
}