using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

namespace Syngenta.AgriCast.Common
{
    public class RewriteContext
    {
        // returns actual RewriteContext instance for
        // current request
        public static RewriteContext Current
        {
            get
            {
                // Look for RewriteContext instance in
                // current HttpContext. If there is no RewriteContextInfo
                // item then this means that rewrite module is turned off
                if (HttpContext.Current.Items.Contains("RewriteContextInfo"))
                    return (RewriteContext)HttpContext.Current.Items["RewriteContextInfo"];
                else
                    return new RewriteContext();
            }
        }

        public RewriteContext()
        {
            _Params = new NameValueCollection();
            _InitialUrl = String.Empty;
        }

        public RewriteContext(NameValueCollection param, string url)
        {
            _InitialUrl = url;
            _Params = new NameValueCollection(param);

        }

        private NameValueCollection _Params;

        public NameValueCollection Params
        {
            get { return _Params; }
            set { _Params = value; }
        }

        private string _InitialUrl;

        public string InitialUrl
        {
            get { return _InitialUrl; }
            set { _InitialUrl = value; }
        }
    }
}