# SimpliSafeMonitorService
a windows service which executes a specific .exe file when your SimpliSafe alarm status changes.  this service polls
the simplisafe servers once every 5 minutes to get the status of the alarm.  when the status changes, the service
executes a specified file along with any arguments specific to the new status.

how to use:
first, install the SimpliSafeMonitorService.exe as a windows service.  for instructions on how to install it, search for
"how to install a windows service".  

next, paste the SimpliSafeMonitorService.conf file into your C:/Windows/SysWOW64 folder.
edit this file using some text editor like notepad.  you should enter your account username, password, filePath 
including the file name and extension, the arguments to be supplied when the system changes mode, and the address of 
the location, exactly as it appears in your online account.  for more details please see the comments in the file.

for debugging purposes, a file called SimpliSafeMonitorService.log is created if it does not exist whenever the service
is started.  this file stores a time and  date stamp, along with a brief message whenever certain events or an error
occurs.  this info should help identify any errors with the service.

notes:
i have recently been having issues with the SimpliSafe server sending a status of unknown.  this issue revolves around
when i use the mobile application or website to set the status of the alarm.  i believe this issue to be with SimpliSafe
and not with my service.  the issue results when i try to change the status of the alarm from the mobile application or 
the website.  sometimes the alarm will not change status and the service will receive a status of "unknown" or 
"pending ..." and the service fails.  sometimes, if i try to set the alarm status back, this will fix the issue.  i 
believe this occurs because SimpliSafe is upgrading either their servers or communication protocol in preparation for 
the new mobile app, scheduled to be released in december 2015.