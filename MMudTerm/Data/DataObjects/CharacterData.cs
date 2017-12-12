using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMudTerm.Data.DataObjects
{
    
    [Serializable]
    public class AccountData
    {
        string _acctName, _pw;
    }

    [Serializable]
    public class CharacterData
    {
        string _fname, _lname, _class, _race;
    }
}
