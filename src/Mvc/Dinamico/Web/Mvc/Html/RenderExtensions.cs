﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Details;
using System.IO;
using System.Text;
using System.Web.WebPages;

namespace N2.Web.Mvc.Html
{
	public static class RenderExtensions
	{
		class DisplayWrapper : IDisplayRenderer, IHtmlString
		{
			public IDisplayRenderer Wrapped { get; set; }
			public Func<Template<IDisplayRenderer>, HelperResult> Template { get; set; }

			#region IDisplayRenderer Members

			public Rendering.RenderingContext Context { get { return Wrapped.Context; } }
			
			public void Render()
			{
				WriteTo(Context.Html.ViewContext.Writer);
			}

			public void WriteTo(System.IO.TextWriter writer)
			{
				if(Template == null)
				{
					Wrapped.WriteTo(writer);
					return;
				}

				StringBuilder sb = new StringBuilder();
				using (var sw = new StringWriter(sb))
				{
					Wrapped.WriteTo(sw);

					if (sb.Length == 0)
						return;

					Template(new Template<IDisplayRenderer> { ContentRenderer = (w) => w.Write(sb), Data = Wrapped }).WriteTo(writer);
				}
			}

			public override string ToString()
			{
				using (var sw = new StringWriter())
				{
					WriteTo(sw);
					return sw.ToString();
				}
			}

			#endregion

			#region IHtmlString Members

			public string ToHtmlString()
			{
				return ToString();
			}

			#endregion
		}

		public static IDisplayRenderer Wrap(this IDisplayRenderer renderer, Func<Template<IDisplayRenderer>, HelperResult> template)
		{
			return new DisplayWrapper { Wrapped = renderer, Template = template };
		}

		public static DisplayRenderer<DisplayableImageAttribute> Image(this RenderHelper render, string detailName)
		{
			return render.Displayable<DisplayableImageAttribute>(detailName);
		}
		public static DisplayRenderer<DisplayableImageAttribute> Image(this RenderHelper render, string detailName, string preferredSize)
		{
			return render.Displayable(new DisplayableImageAttribute { Name = detailName, PreferredSize = preferredSize });
		}

		public static DisplayRenderer<DisplayableHeadingAttribute> Heading(this RenderHelper render, string detailName)
		{
			return render.Displayable(new DisplayableHeadingAttribute { Name = detailName });
		}
		public static DisplayRenderer<DisplayableHeadingAttribute> Heading(this RenderHelper render, string detailName, int headingLevel)
		{
			return render.Displayable(new DisplayableHeadingAttribute { Name = detailName, HeadingLevel = headingLevel });
		}
	}
}