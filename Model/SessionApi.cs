using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestFullLocationApi.Model
{
    public class SessionApi
    {
        public SessionApi()
        {
        }

        public void sessionAdd(string label, string content)
        {
            if (string.IsNullOrEmpty(label) || string.IsNullOrEmpty(content))
                return;

            if (HttpContext.Current.Session[label] == null)
            {
                HttpContext.Current.Session.Add(label, content);
            }
        }

        public void sessionSetValue(string label, string content)
        {
            if (string.IsNullOrEmpty(label) || string.IsNullOrEmpty(content))
                return;

            if (HttpContext.Current.Session[label] == null)
            {
                return;
            }
            else
            {
                HttpContext.Current.Session[label] = content;
            }
        }

        public String sessionGetId()
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Session.SessionID.ToString();
            else
                return "0";
        }

        public Object sessionGetValue(string label)
        {
            Object obj = null;
            try
            {
                obj = HttpContext.Current.Session[label];
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return obj;
        }

        public void sessionClear()
        {
            var context = HttpContext.Current;
            if (context.Session != null)
                context.Session.Clear();

            HttpContext.Current = null;
        }
    }
}