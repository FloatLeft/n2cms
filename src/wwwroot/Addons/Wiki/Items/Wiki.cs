using N2.Templates.Items;
using N2.Integrity;
using N2.Details;
using N2.Security.Details;
using System.Collections.Generic;
using N2.Web;

namespace N2.Addons.Wiki.Items
{
    [Definition("Wiki", "Wiki", "A set of pages that can be updated by external users.", "", 460)]
    [RestrictParents(typeof(IStructuralPage))]
    [N2.Web.UI.TabPanel(Wiki.WikiTab, "Wiki", 110)]
    [AllowedChildren(typeof(Subscribe))]
	[Template("search", "~/Addons/Wiki/UI/Views/Search.aspx")]
	[Template("nohits", "~/Addons/Wiki/UI/Views/NoHits.aspx")]
	public class Wiki : WikiArticle, IWiki
    {
        public const string WikiTab = "wiki";

        public Wiki()
        {
            Visible = true;
        }

        public static string DefaultUploadFolder = "/Upload/Wiki/";
        [EditableTextBox("Upload Folder", 100, ContainerName = Wiki.WikiTab)]
        public virtual string UploadFolder
        {
            get { return (string)(GetDetail("UploadFolder") ?? DefaultUploadFolder); }
            set { SetDetail("UploadFolder", value, DefaultUploadFolder); }
        }

        public static string DefaultSubmitText = "<p>Please write some content for the article '{{actionparameter}}'.</p>";
        [WikiText("Submit Text", 110, ContainerName = Wiki.WikiTab)]
        public virtual string SubmitText
        {
            get { return (string)(GetDetail("SubmitText") ?? DefaultSubmitText); }
            set { SetDetail("SubmitText", value, DefaultSubmitText); }
        }

        public static string DefaultModifyText = "<p>Please write the updated content for '{{pagename}}'.</p>";
        [WikiText("Modify Text", 110, ContainerName = Wiki.WikiTab)]
        public virtual string ModifyText
        {
            get { return (string)(GetDetail("ModifyText") ?? DefaultModifyText); }
            set { SetDetail("ModifyText", value, DefaultModifyText); }
        }

        public static string DefaultHistoryText = "<p>These are the current and past revisions of '{{pagename}}'.</p>";
        [WikiText("History Text", 110, ContainerName = Wiki.WikiTab)]
        public virtual string HistoryText
        {
            get { return (string)(GetDetail("HistoryText") ?? DefaultHistoryText); }
            set { SetDetail("HistoryText", value, DefaultHistoryText); }
        }

        public static string DefaultSearchText = "<p>Your search for '{{linkfromparameter}}' yelded the following results:</p>";
        [WikiText("Search Text", 110, ContainerName = Wiki.WikiTab)]
        public virtual string SearchText
        {
            get { return (string)(GetDetail("SearchText") ?? DefaultSearchText); }
            set { SetDetail("SearchText", value, DefaultSearchText); }
        }

        public static string DefaultNoHitsText = "<p>Your search for '{{linkfromparameter}}' yelded no results. You may submit new article with the name '{{linkfromparameter}}'.</p>";
        [WikiText("Search No Hits Text", 111, ContainerName = Wiki.WikiTab)]
        public virtual string NoHitsText
        {
            get { return (string)(GetDetail("NoHitsText") ?? DefaultNoHitsText); }
            set { SetDetail("NoHitsText", value, DefaultNoHitsText); }
        }

        [EditableRoles(Title = "Role required for write access", ContainerName = Wiki.WikiTab)]
        public virtual IEnumerable<string> ModifyRoles
        {
            get { return GetDetailCollection("ModifyRoles", true).Enumerate<string>(); }
        }

        public override string IconUrl
        {
            get { return "~/Addons/Wiki/UI/Img/page_wiki.gif"; }
        }

        public override IWiki WikiRoot
        {
            get { return this; }
        }

		public override TemplateData FindTemplate(string remainingUrl)
		{
			TemplateData data = base.FindTemplate(remainingUrl);
			if(data.CurrentItem == null)
			{
				data = new TemplateData(this, "~/Addons/Wiki/UI/Views/Submit.aspx", "submit", Utility.CapitalizeFirstLetter(remainingUrl));
			}
			return data;
		}
    }
}