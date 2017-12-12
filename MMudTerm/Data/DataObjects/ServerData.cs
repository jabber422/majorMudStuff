using System;
using System.Collections.Generic;
using System.Text;

namespace MMudTerm.Data.DataObjects
{
    [Serializable]
    public class ServerData
    {
        string _name, _ip, _dbName = "NONE", _dbVer = "NONE";
        int _port;

        #region accesors
        public string Name
        { get { return this._name; } }
        public string IpAddress
        { get { return this._ip; } }
        public string DbName
        { get { return this._dbName; } }
        public string DbVer
        { get { return this._dbVer; } }
        public int Port
        { get { return this._port; } }
        #endregion

        public void CreateNewServer(string name, string ip, int port)
        {
            this._name = name;
            this._ip = ip;
            this._port = port;
        }
    }

    /// <summary>
    /// We can store data in a DB or in files...
    /// </summary>
    public abstract class TermDataAdapter
    {
        const bool sourceIsFile = true;
        bool source = sourceIsFile;
    }

    public class ServerDataAdapter : TermDataAdapter
    {
    }

    public class CharacterSessionDataAdapter : TermDataAdapter
    {
    }
}
