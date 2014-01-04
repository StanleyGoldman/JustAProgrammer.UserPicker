using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace JustAProgrammer.UserPicker.Settings
{
    public class UserPickerFieldEditorEvents : ContentDefinitionEditorEventsBase
    {
        private readonly IContentManager _contentManager;

        public UserPickerFieldEditorEvents(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "UserPickerField")
            {
                var model = definition.Settings.GetModel<UserPickerFieldSettings>();
                model.QueryList = _contentManager.Query("Query").List()
                    .Select(c => new SelectListItem
                    {
                        Text = _contentManager.GetItemMetadata(c).DisplayText,
                        Value = c.Id.ToString(CultureInfo.InvariantCulture),
                        Selected = c.Id == model.QueryId
                    }).ToList();

                model.QueryList.Insert(0, new SelectListItem { Text = "None", Value = "" });

                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.FieldType != "UserPickerField")
            {
                yield break;
            }

            var model = new UserPickerFieldSettings();
            if (updateModel.TryUpdateModel(model, "UserPickerFieldSettings", null, null))
            {
                builder.WithSetting("UserPickerFieldSettings.Hint", model.Hint);
                builder.WithSetting("UserPickerFieldSettings.Required", model.Required.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("UserPickerFieldSettings.Multiple", model.Multiple.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("UserPickerFieldSettings.QueryId", model.QueryId.ToString(CultureInfo.InvariantCulture));
            }

            yield return DefinitionTemplate(model);
        }
    }
}
