using System.Collections.Generic;
using System.Linq;

namespace MMudObjects
{
    public class TrackedPlayer : Player
    {
        public List<string> Alignments = new List<string>();
        public List<string> Titles = new List<string>();
        public List<string> GangNames = new List<string>();
        public List<string> LastNames = new List<string>();
        public List<string> LevelRanges = new List<string>();
        public List<TrackedValue<double>> Exps = new List<TrackedValue<double>>();
        public List<int> Ranks = new List<int>();
        public List<EquippedItemsInfo> Equipments = new List<EquippedItemsInfo>();

        public TrackedPlayer(string firstName) : base(firstName)
        {
        }

        public void UpdateTrackedPlayer(Player p)
        {
            if(p.LastName != null && p.LastName != this.LastName)
            {
                this.LastName = p.LastName;
            }

            if (p.Alignment != null && p.Alignment != this.Alignment)
            {
                this.Alignment = p.Alignment;
            }

            if (p.GangName != null && p.GangName != this.GangName)
            {
                this.GangName = p.GangName;
            }

            if (p.Title != null && p.Title != this.Title)
            {
                this.Title = p.Title;
                this.LevelRange = ClassTitles.GetLevelRangeByClassAndTitle(p.Class, p.Title);
            }

            if (p.Equipped != null && p.Equipped.CompareTo(this.Equipped) == 0)
            {
                this.Equipped = p.Equipped;
            }

            if (p.Exp != 0 && p.Exp != this.Exp)
            {
                this.Exp = p.Exp;
            }

            if (p.Rank != 0 && p.Rank != this.Rank)
            {
                this.Rank = p.Rank;
            }
        }

        public override string Alignment {
            get
            {
                if (!this.Alignments.Any()) return "";
                return this.Alignments.Last();
            }
            set => this.Alignments.Add(value);
        }

        public override EquippedItemsInfo Equipped {
            get
            {
                if (!this.Equipments.Any()) return new EquippedItemsInfo();
                return this.Equipments.Last();
            }
            set => this.Equipments.Add(value);
        }

        public override string Title {
            get
            {
                if (!this.Titles.Any()) return "";
                return this.Titles.Last();
            }
            set => this.Titles.Add(value);
        }

        public override string GangName {
            get
            {
                if (!this.GangNames.Any()) return "";
                return this.GangNames.Last();
            }
            set => this.GangNames.Add(value);
        }

        public override string LastName {
            get
            {
                if (!this.LastNames.Any()) return "";
                return this.LastNames.Last();
            }
            set => this.LastNames.Add(value);
        }

        public override string LevelRange {
            get
            {
                if (!this.LevelRanges.Any()) return "";
                return this.LevelRanges.Last();
            }
            set => this.LevelRanges.Add(value);
        }

        public override double Exp {
            get
            {
                if (!this.Exps.Any()) return -1;
                return this.Exps.Last().Value;
            }
            set {
                if (!this.Exps.Any())
                    this.Exps.Add(new TrackedValue<double>(value));
                else
                    this.Exps.Add(new TrackedValue<double>(value, this.Exps.Last()));
            }
        }

        public double InitialExp
        {
            get
            {
                if (!this.Exps.Any()) return -1;
                return this.Exps.First().Value;
            }
        }

        public override int Rank {
            get
            {
                if (!this.Ranks.Any()) return -1;
                return this.Ranks.Last();
            }
            set => this.Ranks.Add(value);
        }

        public double TotalExpGained { get => GetTotalExpGained(); }
        public double LastExpGained { get => GetLastExpGained(); }
        public string TotalExpRate { get => GetTotalExpRate(); }
        public string LastExpRate { get => GetLastExpRate(); }


        //Get the total amount of exp gain that we've monitored
        private double GetTotalExpGained()
        {
            double result = 0;
            if (this.Exps.Count <= 1) return result;

            for (int i = 1; i < this.Exps.Count; i++)
            {
                if (this.Exps[i].Delta == null) return this.Exps[i].Value;

                result += this.Exps[i].Delta.Value;
            }
            return result;
        }

        //get the last exp change that we monitored
        private double GetLastExpGained()
        {
            double result = 0;
            if (this.Exps.Count <= 1) return result;

            TrackedValue<double> LastTrackedXpChange = this.Exps.Last();
            if (LastTrackedXpChange.Delta == null) return LastTrackedXpChange.Value;

            return LastTrackedXpChange.Delta.Value;
        }

        //get the rate of exp gain for all the xp changes we monitored
        private string GetTotalExpRate()
        {
            if (this.Exps.Count <= 1) return "Not Enough Info";

            double TotalSeconds = 0;
            double TotalExp = 0;
            for (int i = 1; i < this.Exps.Count; i++)
            {
                TotalSeconds += this.Exps[i].Delta.timeSpan.TotalSeconds;
                TotalExp += this.Exps[i].Delta.Value;
            }

            double rate = TotalExp / TotalSeconds;

            return string.Format("{0} per {1} seconds - rate {2} per/sec", TotalExp, TotalSeconds, rate);
        }

        //get the xp rate for the last xp change that was monitored
        private string GetLastExpRate()
        {
            if (this.Exps.Count <= 1) return "Not Enough Info";
            TrackedValue<double> LastExp = this.Exps.Last();

            double rate = LastExp.Delta.Value / LastExp.Delta.timeSpan.TotalSeconds;
            return string.Format("{0} per {1} seconds - rate {2} per/sec", LastExp.Delta.Value, LastExp.Delta.timeSpan.TotalSeconds, rate);
        }
    }
}
