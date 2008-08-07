﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;
using System.Collections.Specialized;

namespace N2.Web
{
    /// <summary>
    /// A web context wrapper that maps to the web request context for calls in 
    /// web application scope and thread context when no request has been made 
    /// (e.g. when executing code in scheduled action).
    /// </summary>
    public class AdaptiveContext : IWebContext
    {
        IWebContext thread = new ThreadContext();
        IWebContext web = new WebRequestContext();

        public AdaptiveContext()
        {
        }

        /// <summary>Gets wether there is a web context availabe.</summary>
        public bool IsWeb
        {
            get { return HttpContext.Current != null; }
        }

        /// <summary>Returns either the web or the thread context depending on <see cref="IsWeb"/>.</summary>
        protected IWebContext CurrentContext 
        {
            get { return IsWeb ? web : thread; }
        }

        /// <summary>Gets a dictionary of request scoped items.</summary>
        public IDictionary RequestItems
        {
            get { return CurrentContext.RequestItems; }
        }

        /// <summary>Gets the current user principal (may be null).</summary>
        public System.Security.Principal.IPrincipal User
        {
            get { return CurrentContext.User; }
        }

        /// <summary>The current request object.</summary>
        public System.Web.HttpRequest Request
        {
            get { return CurrentContext.Request; }
        }

        /// <summary>The current response object.</summary>
        public System.Web.HttpResponse Response
        {
            get { return CurrentContext.Response; }
        }

        /// <summary>The handler associated with this request.</summary>
        public System.Web.IHttpHandler Handler
        {
            get { return CurrentContext.Handler; }
        }

        /// <summary>A page instance stored in the request context.</summary>
        public ContentItem CurrentPage
        {
            get { return CurrentContext.CurrentPage; }
            set { CurrentContext.CurrentPage = value; }
        }

        /// <summary>The physical path on disk to the requested page.</summary>
        public string PhysicalPath
        {
            get { return CurrentContext.PhysicalPath; }
        }

        /// <summary>The local part of the requested path, e.g. /path/to/a/page.aspx?some=query.</summary>
        public Url LocalUrl
        {
            get { return CurrentContext.LocalUrl; }
        }

        /// <summary>The host part of the requested url, e.g. http://n2cms.com/.</summary>
        public Url HostUrl
        {
            get { return CurrentContext.HostUrl; }
        }

        /// <summary>Converts a virtual path to an an absolute path. E.g. ~/hello.aspx -> /MyVirtualDirectory/hello.aspx.</summary>
        /// <param name="virtualPath">The virtual url to make absolute.</param>
        /// <returns>The absolute url.</returns>
        public string ToAbsolute(string virtualPath)
        {
            return CurrentContext.ToAbsolute(virtualPath);
        }

        /// <summary>Converts an absolute url to an app relative path. E.g. /MyVirtualDirectory/hello.aspx -> ~/hello.aspx.</summary>
        /// <param name="virtualPath">The absolute url to convert.</param>
        /// <returns>An app relative url.</returns>
        public string ToAppRelative(string virtualPath)
        {
            return CurrentContext.ToAppRelative(virtualPath);
        }

        /// <summary>Maps a virtual path to a physical disk path.</summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public string MapPath(string path)
        {
            return CurrentContext.MapPath(path);
        }

        /// <summary>Rewrites the request to the given path.</summary>
        /// <param name="path">The path to the template that will handle the request.</param>
        public void RewritePath(string path)
        {
            CurrentContext.RewritePath(path);
        }

        /// <summary>Disposes request items that needs disposing. This method should be called at the end of each request.</summary>
        public void Dispose()
        {
            CurrentContext.Dispose();
        }
    }
}