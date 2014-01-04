using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustAProgrammer.UserPicker.Settings;
using JustAProgrammer.UserPicker.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Projections.Services;
using Orchard.Security;
using Orchard.Utility.Extensions;

namespace JustAProgrammer.UserPicker.Drivers
{
    public class UserPickerFieldDriver : ContentFieldDriver<Fields.UserPickerField>
    {
        private readonly IMembershipService _membershipService;
        private readonly IProjectionManager _projectionManager;
        private readonly IContentManager _contentManager;

        public UserPickerFieldDriver(IMembershipService membershipService, IProjectionManager projectionManager, IContentManager contentManager)
        {
            _membershipService = membershipService;
            _projectionManager = projectionManager;
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        private static string GetPrefix(Fields.UserPickerField field, ContentPart part)
        {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(Fields.UserPickerField field, ContentPart part)
        {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, Fields.UserPickerField field, string displayType, dynamic shapeHelper)
        {
            return Combined(
                ContentShape("Fields_UserPicker", GetDifferentiator(field, part), () => shapeHelper.Fields_UserPicker()),
                ContentShape("Fields_UserPicker_SummaryAdmin", GetDifferentiator(field, part), () => shapeHelper.Fields_UserPicker_SummaryAdmin())
            );
        }

        protected override DriverResult Editor(ContentPart part, Fields.UserPickerField field, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<UserPickerFieldSettings>();
            var queryId = settings.QueryId;

            List<SelectListItem> usersSelectList = null;
            if (queryId != 0)
            {
                var query = (dynamic)_contentManager.Get(queryId);

                usersSelectList = _projectionManager.GetContentItems(queryId)
                                                    .Select(c =>
                                                        {
                                                            if (c.ContentType != "User")
                                                            {
                                                                string message = "Query '" + (query.TitlePart.Title) +
                                                                                 "' returned a '" + c.ContentType +
                                                                                 "' ContentItem Id:" + c.Id;
                                                                Logger.Warning(message);

                                                                return null;
                                                            }

                                                            var contentItemMetadata =
                                                                _contentManager.GetItemMetadata(c);
                                                            var displayText = contentItemMetadata.DisplayText;
                                                            var identity = contentItemMetadata.Identity.ToString();

                                                            return new SelectListItem
                                                                {
                                                                    Text = displayText,
                                                                    Value = identity,
                                                                    Selected = field.Usernames.Contains(identity)
                                                                };
                                                        })
                                                    .Where(item => item != null)
                                                    .ToList();
            }

            return ContentShape("Fields_UserPicker_Edit", GetDifferentiator(field, part),
                () =>
                {
                    var model = new UserPickerFieldViewModel
                        {
                            Field = field,
                            Part = part,
                            SelectedUserNames = field.Usernames,
                            SelectedUserName = field.Usernames.FirstOrDefault(),
                            UserSelectionList = usersSelectList
                        };

                    return shapeHelper.EditorTemplate(TemplateName: "Fields/UserPicker.Edit", Model: model, Prefix: GetPrefix(field, part));
                });
        }

        protected override DriverResult Editor(ContentPart part, Fields.UserPickerField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var model = new UserPickerFieldViewModel();

            updater.TryUpdateModel(model, GetPrefix(field, part), null, null);

            var settings = field.PartFieldDefinition.Settings.GetModel<UserPickerFieldSettings>();

            if (!model.SelectedUserNames.Any())
            {
                field.Usernames = !string.IsNullOrEmpty(model.SelectedUserName) 
                    ? new[] { model.SelectedUserName } 
                    : new string[0];
            }
            else
            {
                field.Usernames = model.SelectedUserNames;
            }

            if (settings.Required && field.Usernames.Length == 0)
            {
                updater.AddModelError("Id", T("The field {0} is mandatory", field.Name.CamelFriendly()));
            }

            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, Fields.UserPickerField field, ImportContentContext context)
        {
            var contentItemIds = context.Attribute(field.FieldDefinition.Name + "." + field.Name, "UserNames");
            if (contentItemIds != null)
            {
            }
            else
            {
            }
        }

        protected override void Exporting(ContentPart part, Fields.UserPickerField field, ExportContentContext context)
        {
            if (field.Usernames.Any())
            {
                context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("UserNames", "");
            }
        }

        protected override void Describe(DescribeMembersContext context)
        {
            context
                .Member(null, typeof(string), T("UserNames"), T("A formatted list of the UserNames, e.g., {admin},{user1}"));
        }
    }
}