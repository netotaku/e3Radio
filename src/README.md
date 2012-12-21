e3Radio server
=============================

The contents of the 'src' folder make up the e3Radio server:

- e3Radio.API = REST api no longer used
- e3Radio.Data = entity framework database access and external data access
- e3Radio.Service = main windows service project - websocket server
- e3Radio.WebSocketAPI = test console websocket server

Set up database
--------------------------

e3Radio server needs a MS SQL database. SQL 2005 Express is fine
or any later version.

Create a new database. Run the script to generate the schema.

Create a login with datareader and datawriter to access the db.


Install the server
-----------------------

1. Build e3Radio.Service project in release mode.

2. Copy the e3Radio.Service/bin/release folder elsewhere if you like
   (e.g. program files)
   
3. Make sure .NET Framework 4.0 is installed

4. Configure the settings in e3Radio.Service.exe.config:

- listenOn = IP and port to run socket server on (must be accessible by users)
- fbAppId = facebook app ID used by front end for facebook login
- fbAppSecret = facebook app secret
- E3RadioEntities = update connectionstring to point to the database

5. Grant network service permission to read the folder

6. Create a log.txt file and grant network service modify permission to it

7. Install the service

- open an administrator command prompt to the folder with the files in
- run C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe e3Radio.Service.exe

8. Open up services.msc and start the e3Radio service.


Uninstall the server
------------------------

1. Stop the service via services.msc
2. open an administrator command prompt to the folder with the files in
3. run C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u e3Radio.Service.exe
4. delete the files