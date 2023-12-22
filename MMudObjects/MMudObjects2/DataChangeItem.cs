using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MMudObjects
{
    public class DataChangeItem
    {
        public string StartPattern;
        public List<Group> StartGroups;
        public string EndPattern;
        public List<Group> EndGroups;
        public List<List<Group>> Middle;
        public string TargetProperty;

        //public DataChangeItem(string startPattern, GroupCollection startGroups, string endPattern)
        //{
        //    this.targetProperty = targetProperty;
        //    this.groups = groups;
        //}

        public DataChangeItem(string startPattern, GroupCollection startGroups, string endPattern, GroupCollection endGroups, List<GroupCollection> middle, string targetProperty)
        {
            this.StartPattern = startPattern;

            Group[] _startGroups = new Group[startGroups.Count];
            startGroups.CopyTo(_startGroups, 0);
            this.StartGroups = new List<Group>(_startGroups);

            if (endPattern != null)
            {
                this.EndPattern = endPattern;
                Group[] _endGroups = new Group[endGroups.Count];
                endGroups.CopyTo(_endGroups, 0);
                this.EndGroups = new List<Group>(_endGroups);
            }

            this.Middle = new List<List<Group>>();
            foreach (GroupCollection gc in middle)
            {
                Group[] _gc = new Group[gc.Count];
                startGroups.CopyTo(_gc, 0);
                this.Middle.Add(new List<Group>(_gc));
            }
            this.TargetProperty = targetProperty;
        }
    }
}