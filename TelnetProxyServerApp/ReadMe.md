This is a Telnet Proxy Tap you can use it to tap into a telnet session and r/w to the stream

When you connect this will prompt with:
ID?

There are two modes.  Connect to a Remote server
You send - ID,1,42  ID,<1 = Connect to Remote Server>,<42 = Session Token>

The proxy server will then ask for the remote connection info
CON?
You send - CON,BBS.com,23

You don't need carriage returns on the response... i should change that..


The Second mode is to tap into a session stream
ID?
ID,2,42 - ID, <2 = Tamp into session>,<42 = Session Token>
