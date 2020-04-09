Please read and make the necessary changes before hosting.

**General**<br>
Please add a DNS alias named SSServer where SSServer is the machine where the server and database are hosted.  SS allows you to toggle between printers (e.g. in case your public printer goes down).  Please modify the printer code (SessionSoftTimer/Printers.cs *and* the printer database table - printer names are the table values) to reflect your local environment.  Full MSSQL database schema to be posted later.  The SS timer uses an optional sentinel program to prevent users from killing the process.  If you not blocking taskmanager, it would be a good idea to use this.      

**SessionSoftTimer**<br>
This is the client application to be hosted on the local, public machines.  Session.cs and LogStats.cs need the site specific connection string.

**SessionSoftWebClient**<br>
The "splash" screen that the local applcation will use.  The web.config file requires the site specific connection string.  Controller.aspx.cs requires the SIP2 parameters.  That same file consumes a webservice avaiable via the Koha ILS that allows you to use a patron's username and pin to log into a public PC.  If you are not running Koha, disable that function.  Local logging in (i.e. from a randomly generated password) and logging into the public PC by library barcode is supported by all ILSs running a SIP2 server...so basically all of them.  SIP2 code is available at <a href="" />NiceAndNerdy/SIP2.NET</a>

**SS2**<br>
The administration server.  *VERY IMPORTANT* - Login.aspx.cs uses the database parameters to log in.  Never do this.  Likewise, the admin password for changing client settings is hardcoded into Controller.aspx.cs.  This is also a bad idea.  Web.Config requires the local database connection string.

