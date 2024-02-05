using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMudObjects
{
    [Serializable]
    public class MessageResponse
    {
        public string Name { set; get; }
        private string message;
        public string Message
        {
            set
            {
                message = value.Replace("{target}", "MOB").Replace("{dmg}", @"(\d+)").Replace("{1}", "PLAYER").Replace(".", @"\.").Replace("You ", "PLAYER ").Replace(" you ", " PLAYER ");
            }
            get { return message; }
        }
        private string endswith;
        public string EndsWith
        {
            set
            {
                endswith = value.Replace("{target}", "MOB").Replace("{dmg}", @"(\d+)").Replace("{1}", "PLAYER").Replace(".", @"\.").Replace("You ", "PLAYER ").Replace(" you ", " PLAYER ");
            }
            get { return endswith; }
        }
        private string response;
        public string Response
        {
            set
            {
                response = value.Replace("{target}", "MOB").Replace("{dmg}", @"(\d+)").Replace("{1}", "PLAYER").Replace(".", @"\.").Replace("You ", "PLAYER ").Replace(" you ", " PLAYER ");
            }
            get { return response; }
        }

        public bool Blinded;            //0001 
        public bool Confused;           //0002
        public bool Poisoned;           //0004
        public bool Diseased;           //0040
        public bool LosingHP;           //0008
        public bool HpRgen;             //0080
        public bool MpRgen;             //0200
        public bool NoMove;             //0010
        public bool NoAttack;           //0020
        public bool EndsCombat;         //1000
        public bool LastActionFailed;   //2000

        public bool Ignore;
        public bool CheckRoom;          //0100   
        public bool WaitWearOff;        //Y2
        public bool RestHp;             //Y3
        public bool RestMp;             //Y4
        public bool Run;                //Y5
        public bool Hangup;             //Y6

        public bool FindAnywhere;       //0400
        public bool FindConvo;          //Y1
        public bool UseWhenChasing;     //4000
        public bool Disabled;           //8000
        private string one;

        public MessageResponse(string one)
        {
            var tokens = one.Split(':');
            this.Name = tokens[0];
            if (tokens.Length == 4)
            {
                this.Response = tokens[3].Replace("^M", "\r\n");
            }

            var x = int.Parse(tokens[1]);
            var y = int.Parse(tokens[2]);

            Blinded = (x & 0x0001) != 0;
            Confused = (x & 0x0002) != 0;
            Poisoned = (x & 0x0004) != 0;
            LosingHP = (x & 0x0008) != 0;
            NoMove = (x & 0x0010) != 0;
            NoAttack = (x & 0x0020) != 0;
            Diseased = (x & 0x0040) != 0;
            HpRgen = (x & 0x0080) != 0;
            CheckRoom = (x & 0x0100) != 0;
            MpRgen = (x & 0x0200) != 0;
            FindAnywhere = (x & 0x0400) != 0;

            EndsCombat = (x & 0x1000) != 0;
            LastActionFailed = (x & 0x2000) != 0;
            UseWhenChasing = (x & 0x4000) != 0;
            Disabled = (x & 0x8000) != 0;

            FindConvo = y == 1;
            WaitWearOff = y == 2;
            RestHp = y == 3;
            RestMp = y == 4;
            Run = y == 5;
            Hangup = y == 6;
        }
    }
}
