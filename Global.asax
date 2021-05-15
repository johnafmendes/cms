<%@ Application Language="C#" %>

<script runat="server">

    //this method is the first to run for every request. It mean that for each file (css, images, media in general) 
    // will be asked to do download passing by this function, so, we can catch the friendly url that is
    // being requested and go to database to search for that and bring back the absolute url and rewrite them.
    void Application_BeginRequest(object sender, EventArgs e)
    {
        //request url
        HttpContext incoming = HttpContext.Current;
        string LastFriendlyURL = incoming.Request.Path.ToLower();
        
        
        //if (LastFriendlyURL.EndsWith(".html"))
        //if url has not dot into, then search into database to retrieve the absolute url stored.
        if (LastFriendlyURL.IndexOf(".") == -1)
        {
            FriendlyURL f = new FriendlyURL();

            string OriginalURL = f.retrieveOriginalURL(LastFriendlyURL, "1");

            if (OriginalURL.Length > 0) incoming.RewritePath(OriginalURL);
            else incoming.RewritePath(LastFriendlyURL);
        }
    }

    
    void Application_AuthorizeRequest(Object sender, EventArgs e)
    {
        
        /*if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
        { }
        if(System.Web.HttpContext.Current.Session != null)
        {
            General g = Session["app"] as General;

            HttpContext incoming = HttpContext.Current;
            string FriendlyURL = incoming.Request.Path.ToLower();

            string OriginalURL = g.FUrl.retrieveOriginalURL(FriendlyURL);

            if (OriginalURL.Length > 0) incoming.RewritePath(OriginalURL);
            else incoming.RewritePath(FriendlyURL);
        }*/
    }

    void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
    {
        /*if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
        {
            General g = Session["app"] as General;

            HttpContext incoming = HttpContext.Current;
            string FriendlyURL = incoming.Request.Path.ToLower();

            string OriginalURL = g.FUrl.retrieveOriginalURL(FriendlyURL);

            if (OriginalURL.Length > 0) incoming.RewritePath(OriginalURL);
            else incoming.RewritePath(FriendlyURL);
        }*/
    }

    void Application_EndRequest(object sender, EventArgs e)
    {
    }

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown
        
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    //when the session start, create inside the session called app the main class that is General.
    //The General has access to others class.
    void Session_Start(object sender, EventArgs e) 
    {
        General app = new General();
        Session["app"] = app;
    }

    //when the session came to end, the status authentication receive false and after do abandon.
    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
        General g = Session["app"] as General;
        g.Auth.Status = false;
        Session.Abandon();
    }
       
</script>
