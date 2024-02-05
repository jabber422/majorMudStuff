using MMudTerm.Session;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace MMudTerm.Session
{
    public partial class GotoLoopSelectForm : Form
    {
        private bool is_loop;

        public string Answer { get; set; }


        public GotoLoopSelectForm(bool is_loop)
        {
            this.is_loop = is_loop;
            InitializeComponent();
        }

        private void GotoLoopSelectForm_Load(object sender, EventArgs e)
        {
            var lst = PathsCache.Load();

            Dictionary<string, TreeNode> zone_hash = new Dictionary<string, TreeNode>();
            foreach (var item in lst)
            {
                if(item.StartRoomHashCode != item.EndRoomHashCode && is_loop)
                {
                    continue;
                }
                TreeNode node = null;
                if (item.Start != null)
                {
                    if (zone_hash.ContainsKey(item.Start.RoomZone))
                    {
                        node = zone_hash[item.Start.RoomZone];
                    }
                    else
                    {
                        node = new TreeNode(item.Start.RoomZone);
                        zone_hash.Add(item.Start.RoomZone, node);
                        this.treeView1.Nodes.Add(node);
                    }
                    
                    
                    bool found = false;
                    foreach(TreeNode n in node.Nodes)
                    {
                        if(n.Text.Trim() == item.Start.RoomName.Trim())
                        {
                            found = true;break;
                        }
                    }
                    if (!found)
                    {
                        node.Nodes.Add(new TreeNode(item.Start.RoomName));
                    }
                    
                }

                
            }

            SortTreeView(treeView1);          
        }

        private int CompareTreeNodes(TreeNode node1, TreeNode node2)
        {
            return string.Compare(node1.Text, node2.Text);
        }

        private void SortTreeView(TreeView treeView)
        {
            List<TreeNode> sortedNodes = new List<TreeNode>(treeView.Nodes.Cast<TreeNode>());
            sortedNodes.Sort(CompareTreeNodes);
            foreach(TreeNode node in sortedNodes)
            {
                List<TreeNode> sortedNodes2 = new List<TreeNode>(node.Nodes.Cast<TreeNode>());
                sortedNodes2.Sort(CompareTreeNodes);
            }

            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.Nodes.AddRange(sortedNodes.ToArray());
            treeView.EndUpdate();
        }

        MudPath Selected { get; set; }

        private void button_walk_Click(object sender, EventArgs e)
        {
            this.Answer = this.treeView1.SelectedNode.Text;
            if(this.Answer != null)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }

    public static class PathsCache
    {
        //This is a list of MudPath objects, basically *.mp file
        static public List<MudPath> MudPaths { get; set; }
        static public Dictionary<string, long> Rooms { get; set; }

        public static Graph Graph { get; set; }
        
        public static List<MudPath> Load()
        {
            
            if (MudPaths == null)
            {
                MudPaths = new List<MudPath>();
            }

            if( Rooms == null)
            {
                Rooms = new Dictionary<string, long>();
            }

            if (MudPaths.Count > 0)
            {
                return MudPaths;
            }

            var d = Directory.GetCurrentDirectory();
            var p = Path.Combine(d, "res", "Paths");
            var files = Directory.GetFiles(p, "*.mp");
            foreach (string file in files)
            {
                MudPath path = new MudPath(file);
                MudPaths.Add(path);
                if (path.Start == null) continue;
                if (!Rooms.ContainsKey(path.Start.RoomName))
                {
                    Rooms.Add(path.Start.RoomName, path.StartRoomHashCode);
                }
            }

            // Usage
            Graph = new Graph();
            foreach (var mudPath in MudPaths) // Assuming mudPaths is your list of 2000 MudPath objects
            {
                Graph.AddStep(mudPath);
            }

            return MudPaths;
        }

        internal static List<MudPath> GetLoop(long to_room_hash)
        {
            foreach(var path in Load())
            {
                if(path.StartRoomHashCode == path.EndRoomHashCode && path.StartRoomHashCode == to_room_hash)
                {
                    return new List<MudPath>() { path };
                }
            }
            return null;
        }
    }

    public class Graph
    {
        private Dictionary<long, List<long>> _adjacencyList;

        public Graph()
        {
            _adjacencyList = new Dictionary<long, List<long>>();
        }

        public void AddStep(MudPath pathobj)
        {
            if(pathobj.StartRoomHashCode == 0xABC10002)
            {

            }
            if (!_adjacencyList.ContainsKey(pathobj.StartRoomHashCode))
            {
                _adjacencyList[pathobj.StartRoomHashCode] = new List<long>();
            }
            if (_adjacencyList[pathobj.StartRoomHashCode].Contains(pathobj.EndRoomHashCode)) return;
            _adjacencyList[pathobj.StartRoomHashCode].Add(pathobj.EndRoomHashCode);
        }

        public List<MudPath> GetShortestPath(long startHashCode, long endHashCode)
        {
            if (!_adjacencyList.ContainsKey(startHashCode))
            {
                throw new ArgumentException("Start node not found in graph.");
            }

            var queue = new Queue<long>();
            var visited = new HashSet<long>();
            var previous = new Dictionary<long, long>();
            queue.Enqueue(startHashCode);
            visited.Add(startHashCode);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == endHashCode)
                {
                    return ConstructPath(previous, startHashCode, endHashCode);
                }

                foreach (var neighbor in _adjacencyList[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        previous[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return new List<MudPath>(); // No path found
        }

        private List<MudPath> ConstructPath(Dictionary<long, long> previous, long startHashCode, long endHashCode)
        {
            var path = new Stack<long>();
            var current = endHashCode;
            while (current != startHashCode)
            {
                path.Push(current);
                current = previous[current];
            }
            path.Push(startHashCode);

            var result = new List<MudPath>();
            while (path.Count > 1)
            {
                var from = path.Pop();
                var to = path.Peek();
                // Assuming you have a method to get MudPath from hash codes
                result.Add(GetMudPath(from, to));
            }

            return result;
        }

        private MudPath GetMudPath(long fromHashCode, long toHashCode)
        {
            // Implement logic to retrieve the MudPath object that represents the path from 'fromHashCode' to 'toHashCode'
            // This might involve searching through a collection of MudPath objects to find the one that matches these hash codes
            foreach(var path in PathsCache.Load())
            {
                if (path.StartRoomHashCode == fromHashCode &&
                    path.EndRoomHashCode == toHashCode)
                    return path;
            }

            return null;
        }
    }

    public class MudPathRoom
    {
        public MudPathRoom(string line)
        {
            string[] tokens = line.Split(':');
            this.RoomShortName = tokens[0];
            this.RoomZone = tokens[1];
            this.RoomName = tokens[2];
        }

        public string RoomShortName { get; set; }
        public string RoomZone { get; set; }
        public string RoomName { get; set; }
    }

    public class MudPathStep
    {
        public MudPathStep(string line)
        {
            string[] tokens = line.Split(':');
            this.RoomHashCode = Convert.ToInt64(tokens[0],16);
            this.Extras = tokens[1];
            this.Direction = tokens[2];
        }

        public long RoomHashCode { get; set; }
        public string Extras {  get; set; }
        public string Direction { get; set; }
        
    }

    public class MudPath
    {
        public MudPath(string file)
        {
            this.Name = Path.GetFileName(file);
            this.Extras = new List<string>();
            this.Steps = new List<MudPathStep>();
            int idx = 0;
            bool not_done = true;
            using (StreamReader sr = new StreamReader(file))
            {
                //Console.WriteLine(this.Name);
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if(line == "\u001a") { continue; } 
                    if(line.Trim() == "") { continue; }
                    
                    if(idx == 0 && line.StartsWith("["))
                    {
                        this.Author = line.Trim('[', ']');
                    }
                    else if(idx == 1 && line.StartsWith("["))
                    {
                        this.Start = new MudPathRoom(line.Trim('[', ']'));
                        //Console.WriteLine(this.Start.RoomZone + " " + this.Start.RoomName);
                    }
                    else if (idx == 2 && line.StartsWith("["))
                    {
                        this.End = new MudPathRoom(line.Trim('[', ']'));
                    }
                    else if (idx >= 1 && not_done)
                    {
                        string[] tokens = line.Split(':');
                        this.StartRoomHashCode = Convert.ToInt64(tokens[0], 16);
                        if (StartRoomHashCode == 0xABC10002)
                        {

                        }
                        if(Name == "Narnnewh.mp")
                        {

                        }
                        this.EndRoomHashCode = Convert.ToInt64(tokens[1], 16);
                        for (int i = 2; i < tokens.Length; i++)
                        {
                            if (tokens[i] != "") this.Extras.Add(tokens[i]);
                        }
                        not_done = false;
                    }
                    else
                    {
                        this.Steps.Add(new MudPathStep(line));
                    }
                        
                    idx++;
                }
            }
            
        }

        public string Author { get; set; }
        public MudPathRoom Start { get; set; }
        public MudPathRoom End { get; set; }

        public long StartRoomHashCode { get; set; }
        public long EndRoomHashCode { get; set; }
        public List<string> Extras { get; set; }

        public List<MudPathStep> Steps { get; set; }
        public string Name { get; private set; }
    }
}
