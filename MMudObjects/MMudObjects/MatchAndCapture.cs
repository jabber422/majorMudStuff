using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MMudObjects
{
    public class MatchAndCapture
    {
        //in a color state, we get a new TermStringDataCmd, the string
        //check the context, 
        //  if there is an open Match And Capture
        //      if string matches that capture criteria
        //          if MatchAndCapture is complete
        //                MatchAndCapture invokes the change handler for this object, the model is updated, the world is updated
        //              set the context request to null;
        //  else
        //      Check all of the regex's for this state vs string
        //      if - not a match, log warning and ignore
        //      if a match
        //          set the context reqest to the found MatchAndCapture
        //
        //once the capture is complete, the MatchAndCapture object is given to the model,
        //the model updates it self and fires off event about data changed
        string StartPattern;
        Regex Start;
        public GroupCollection StartMatch;
        string EndPattern;
        Regex End;
        public GroupCollection EndMatch;

        List<GroupCollection> Middle;

        public string TargetProperty;
        MatchAndCaptureProp Properties;

        public bool IsComplete { get; set; }
        bool IsCapturing = false;
        private string v;
        public Type type;

        public MatchAndCapture(string key, string value, string targetProperty, MatchAndCaptureProp properties = 0b0)
        {
            this.StartPattern = key;
            this.Start = new Regex(this.StartPattern);
            this.EndPattern = value;
            this.End = new Regex(this.EndPattern);
            this.Middle = new List<GroupCollection>();
            this.TargetProperty = targetProperty;
            this.Properties = properties;
        }

        public MatchAndCapture(string key, string targetProperty, MatchAndCaptureProp properties = 0b0)
        {
            this.StartPattern = key;
            this.Start = new Regex(this.StartPattern);
            this.Middle = new List<GroupCollection>();
            this.TargetProperty = targetProperty;
            this.Properties = properties;
        }

        public MatchAndCapture(string v, Type type)
        {
            this.StartPattern = v;
            this.Start = new Regex(this.StartPattern);
            this.type = type;
        }

        public bool IsMatch(string text)
        {
            return this.Start.IsMatch(text);
        }

        //Color has an open matchAndCapture request (this), checking if current string will close the capture
        public ConsumeResults ConsumeCapturing(string stringCmd)
        {
            Log.Tag("MatchAndCapture", "Called on - " + stringCmd + " IsCapturing: " + this.IsCapturing);
            Match m = this.End.Match(stringCmd);
            if (m.Success)
            {
                this.EndMatch = m.Groups;
                this.IsCapturing = false;
                return ConsumeResults.CaptureComplete;


                //this.IsComplete = true;
                //return true;
            }
            else if ((this.Properties & MatchAndCaptureProp.MatchUntilEnd) == MatchAndCaptureProp.MatchUntilEnd)
            {
                m = Regex.Match(stringCmd, "^(.*)$");
                if (m.Success)
                {
                    Middle.Add(m.Groups);
                    return ConsumeResults.Capturing;
                }
                throw new Exception("This should never happen");
            }
            else if((this.Properties & MatchAndCaptureProp.IgnoreMatchFailure) == MatchAndCaptureProp.IgnoreMatchFailure)
            {
                this.IsCapturing = false;
                return ConsumeResults.CaptureFailed_Ignored;
            }
            else
            {
                throw new Exception("MatchAndCapture pattern failed to Capture");
            }
            
        }

        //Color is searching it's list of regex for a starting match item
        public ConsumeResults ConsumeMatching(string stringCmd)
        {
            Log.Tag("MatchAndCapture", "Called on - " + stringCmd + " IsCapturing: " + this.IsCapturing);
            Match m = this.Start.Match(stringCmd);
            if (m.Success)
            {
                this.StartMatch = m.Groups;
                if (this.End == null)
                {
                    //this is a one line cmd not a key/value pair
                    this.IsCapturing = false;
                    return ConsumeResults.MatchComplete & ConsumeResults.CaptureComplete;
                }
                else
                {
                    this.IsCapturing = true;
                    return ConsumeResults.Capturing;
                }
            }
            return ConsumeResults.NotAMatch;
        }
        //consume a string based on the state of the object.
        //return true if complete, false to persist the match
        public ConsumeResults Consume(string stringCmd)
        {
            Log.Tag("MatchAndCapture", "Called on - " + stringCmd);
            if (IsCapturing)
            {
                return ConsumeCapturing(stringCmd);
            }
            return ConsumeMatching(stringCmd);
        }

        public DataChangeItem CreateDataChangeItem()
        {
            //if (!this.IsComplete) throw new Exception("Can't get DCI object on incomplete MatchAndCapture");

            DataChangeItem dci = new DataChangeItem(StartPattern,StartMatch,EndPattern,EndMatch,Middle,TargetProperty);
            //reset the state since we reuse the same object
            this.StartMatch = null;
            this.EndMatch = null;
            this.Middle.Clear();
            this.IsCapturing = false;
            this.IsComplete = false;
            return dci;
        }
    }


    [Flags]
    public enum MatchAndCaptureProp
    {
        //when set, the second regex will only use the next string cmd or fail loudly, on success the obj is used and set back to null;
        MatchNextString = 0b0001,
        //when set, the second regex will fail quietly, the failed string will be match ^(.*)$ and the MatchAndCapture will trying again on the next line
        MatchUntilEnd = 0b0010,
        MatchUntilColorChange = 0b0100,

        //Override a loud failure
        IgnoreMatchFailure = 0b1000,
        
    }

    [Flags]
    public enum ConsumeResults
    {
        CaptureFailed_Loud = 0b00000, //this is an exception currently
        CaptureFailed_Ignored   = 0b10000,
        NotAMatch               = 0b01000,
        Capturing               = 0b00100,
        CaptureComplete         = 0b00010,
        MatchComplete           = 0b00001,
    }

}