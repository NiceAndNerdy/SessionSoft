Please read and make the necessary changes before hosting.

**General**
Please add a DNS alias named SSServer where SSServer is the machine where the server and database are hosted.   

**SessionSoftTimer**
This is the client application to be hosted on the local, public machines.  Session.cs and LogStats.cs need the site specific connection string.

**SessionSoftWebClient**
The "splash" screen that the local applcation will use.  The web.config file requires the site specific connection string.  Controller.aspx.cs requires the SIP2 parameters.  That same file consumes a webservice avaiable via the Koha ILS.  If you are not running Koha, disable that function.

**SS2**
The administration server.  *VERY IMPORTANT* - Login.aspx.cs uses the database parameters to log in.  Never do this.  Likewise, the admin password for changing client settings is hardcoded into Controller.aspx.cs.  This is also a bad idea.

**General notes**
SS allows you to toggle between printers (e.g. in case on public printer goes down).  Please modify the printer code (SessionSoftTimer/Printers.cs *and* the printer database table - printer names are the table values).  Full database schema to be posted later.
