﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Definitions.Static;
using N2.Engine;
using N2.Web.Mvc;

namespace N2.Definitions.Runtime
{
	[Service]
	public class ViewTemplateAnalyzer
	{
		IProvider<ViewEngineCollection> viewEnginesProvider;
		DefinitionMap map;
		DefinitionBuilder builder;

		public ViewTemplateAnalyzer(IProvider<ViewEngineCollection> viewEnginesProvider, DefinitionMap map, DefinitionBuilder builder)
		{
			this.viewEnginesProvider = viewEnginesProvider;
			this.map = map;
			this.builder = builder;
		}

		public virtual IEnumerable<ViewTemplateDescription> FindRegistrations(VirtualPathProvider vpp, HttpContextBase httpContext, IEnumerable<ViewTemplateSource> sources)
		{
			foreach (var source in sources)
			{
				string virtualDir = "~/Views/" + source.ControllerName;
				if (!vpp.DirectoryExists(virtualDir))
					continue;

				List<ViewTemplateDescription> descriptions = new List<ViewTemplateDescription>();
				foreach (var file in vpp.GetDirectory(virtualDir).Files.OfType<VirtualFile>().Where(f => f.Name.EndsWith(source.ViewFileExtension)))
				{
					var description = AnalyzeView(httpContext, file, source.ControllerName, source.ModelType);
					if (description != null)
						descriptions.Add(description);
				}

				foreach (var description in descriptions)
				{
					description.Definition.Add(new TemplateSelectorAttribute 
					{ 
						Name = "TemplateName", 
						Title = "Template", 
						AllTemplates = descriptions.Select(d => new TemplateSelectorAttribute.Info { Name = d.Registration.Template, Title = d.Registration.Title }).ToArray(), 
						ContainerName = source.TemplateSelectorContainerName, 
						Required = true, 
						HelpTitle = "The page must be saved for another template's fields to appear",
						RequiredPermission = Security.Permission.Administer
					});
				}


				foreach (var description in descriptions)
					yield return description;
			}
		}

		private ViewTemplateDescription AnalyzeView(HttpContextBase httpContext, VirtualFile file, string controllerName, Type modelType)
		{
			if (modelType == null || !typeof(ContentItem).IsAssignableFrom(modelType) || modelType.IsAbstract)
				return null;

			var model = Activator.CreateInstance(modelType) as ContentItem;

			var rd = new RouteData();
			bool isPage = model.IsPage;
			rd.ApplyCurrentItem(controllerName, Path.GetFileNameWithoutExtension(file.Name), isPage ? model : new StubItem(), isPage ? null : model);

			var cctx = new ControllerContext(httpContext, rd, new StubController());

			var result = isPage
				? viewEnginesProvider.Get().FindView(cctx, file.VirtualPath, null)
				: viewEnginesProvider.Get().FindPartialView(cctx, file.VirtualPath);


			if (result.View == null)
				return null;

			return RenderViewForRegistration(file, modelType, cctx, result);
		}

		private ViewTemplateDescription RenderViewForRegistration(VirtualFile file, Type modelType, ControllerContext cctx, ViewEngineResult result)
		{
			var re = new ContentRegistration();
			re.ContentType = modelType;
			re.Template = N2.Web.Url.RemoveAnyExtension(file.Name);
			re.IsDefined = false;
			using (StringWriter sw = new StringWriter())
			{
				var vdd = new ViewDataDictionary();
				cctx.Controller.ViewData = vdd;
				vdd["RegistrationExpression"] = re;

				try
				{
					result.View.Render(new ViewContext(cctx, result.View, vdd, new TempDataDictionary(), sw), sw);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
					if (re.IsDefined)
						throw;
					return null;
				}

				if (re.IsDefined)
				{
					return new ViewTemplateDescription
					{
						Registration = re,
						Definition = GetOrCreateDefinition(re),
						TouchedPaths = new[] { file.VirtualPath }.Union(re.TouchedPaths)
					};
				}
				return null;
			}
		}

		private ItemDefinition GetOrCreateDefinition(ContentRegistration re)
		{
			var definition = map.GetOrCreateDefinition(re.ContentType, re.Template);
			return definition;
		}

		class StubController : Controller
		{
		}
		class StubItem : ContentItem
		{
		}
	}
}