Simple wrapper around the .net socket classes

See IConnection

bool Connect();
bool Disconnect();

bool Connected {get;}

event RcvMsgCallback Rcvr;
event EventHandler Disconnected;
int Send(byte[] buffer);
